using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Dashboard;
using BusinessLogic.Dtos.Function;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            response.CountEmployee = await dbContext.Users.Where(x => !x.IsDeleted).CountAsync();
            var result =  await Result<DashboardResponse>.SuccessAsync(response);
            return result.Data;
        }
    }
}
