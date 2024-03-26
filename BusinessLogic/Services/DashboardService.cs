using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Dashboard;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IDashboardService 
    {
        Task<DashboardResponse> GetDashboard();
    }
    public class DashboardService(ApplicationDbContext dbContext) : IDashboardService
    {
        public async Task<DashboardResponse> GetDashboard()
        {
            var response = new DashboardResponse();
            response.CountUser = await dbContext.Users.Where(x => !x.IsDeleted).CountAsync();
            response.CountEmployee = await dbContext.Bookings.Where(x => !x.IsDeleted).CountAsync();
            var result =  await Result<DashboardResponse>.SuccessAsync(response);
            return result.Data;
        }
    }
}
