using System.Threading.Tasks;

namespace MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;

public interface ICommentStatisticsService
{
    Task<int> GetActiveCommentCountAsync();
    Task<int> GetPassiveCommentCountAsync();
    Task<int> GetTotalCommentCountAsync();
}


