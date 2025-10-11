using System.Threading.Tasks;

namespace MultiShop.WebUI.Services.StatisticsServices.UserStatistic;

public interface IUserStatisticsService
{
    Task<long> GetUserCountAsync();
}


