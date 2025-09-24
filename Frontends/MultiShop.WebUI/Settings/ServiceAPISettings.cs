namespace MultiShop.WebUI.Settings;

public class ServiceAPISettings
{
    public string OcelotUrl { get; set; }
    public string IdentityServerUrl { get; set; }
    public ServiceAPI Catalog { get; set; }
    public ServiceAPI Basket { get; set; }
    public ServiceAPI Cargo { get; set; }
    public ServiceAPI Comment { get; set; }
    public ServiceAPI Discount { get; set; }
    public ServiceAPI Order { get; set; }
    public ServiceAPI Payment { get; set; }
    public ServiceAPI Images { get; set; }
}

public class ServiceAPI
{
    public string Path { get; set; }
}
