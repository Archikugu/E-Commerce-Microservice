using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using MultiShop.Images.Utils.ConfigOptions;

namespace MultiShop.Images.Services;

public class CloudStorageService : ICloudStorageService
{
    private readonly GCSConfigOptions _options;
    private readonly ILogger<CloudStorageService> _logger;
    private readonly GoogleCredential _googleCredential;

    public CloudStorageService(IOptions<GCSConfigOptions> options, ILogger<CloudStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Production)
            {
                // Store the json file in Secrets.
                _googleCredential = GoogleCredential.FromJson(_options.GCPStorageAuthFile);
            }
            else
            {
                _googleCredential = GoogleCredential.FromFile(_options.GCPStorageAuthFile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
            throw;
        }
    }
    public async Task DeleteFileAsync(string fileNameToDelete)
    {
        try
        {
            using (var storageClient = StorageClient.Create(_googleCredential))
            {
                await storageClient.DeleteObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToDelete);
            }
            _logger.LogInformation($"File {fileNameToDelete} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while deleting file {fileNameToDelete}: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetSignedUrlAsync(string fileNameToRead, int timeOutInMinutes = 30)
    {
        try
        {
            var sac = _googleCredential.UnderlyingCredential as ServiceAccountCredential;
            var urlSigner = UrlSigner.FromServiceAccountCredential(sac);
            // provides limited permission and time to make a request: time here is mentioned for 30 minutes.
            var signedUrl = await urlSigner.SignAsync(_options.GoogleCloudStorageBucketName, fileNameToRead, TimeSpan.FromMinutes(timeOutInMinutes));
            _logger.LogInformation($"Signed url obtained for file {fileNameToRead}");
            return signedUrl.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while obtaining signed url for file {fileNameToRead}: {ex.Message}");
            throw;
        }
    }

    public async Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave)
    {
        try
        {
            _logger.LogInformation($"Uploading: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
            using (var memoryStream = new MemoryStream())
            {
                await fileToUpload.CopyToAsync(memoryStream);
                // Create Storage Client from Google Credential
                using (var storageClient = StorageClient.Create(_googleCredential))
                {
                    // upload file stream
                    var uploadedFile = await storageClient.UploadObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToSave, fileToUpload.ContentType, memoryStream);
                    _logger.LogInformation($"Uploaded: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                    return uploadedFile.MediaLink;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while uploading file {fileNameToSave}: {ex.Message}");
            // DEV FALLBACK: write to local wwwroot when GCS not reachable (e.g., DNS outage)
            try
            {
                var root = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads");
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                var localPath = Path.Combine(root, fileNameToSave);
                using (var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await fileToUpload.CopyToAsync(fs);
                }
                var baseUrl = _options.FallbackBaseUrl?.TrimEnd('/') ?? string.Empty; // e.g., https://localhost:7008
                var publicUrl = string.IsNullOrEmpty(baseUrl)
                    ? $"/uploads/{fileNameToSave}"
                    : $"{baseUrl}/uploads/{fileNameToSave}";
                _logger.LogWarning($"GCS unavailable. Saved locally: {publicUrl}");
                return publicUrl;
            }
            catch (Exception inner)
            {
                _logger.LogError($"Local fallback failed for {fileNameToSave}: {inner.Message}");
                throw;
            }
        }
    }
}
