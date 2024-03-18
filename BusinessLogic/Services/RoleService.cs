using System.Linq.Dynamic.Core;
using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Role;
using BusinessLogic.Entities;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IRoleService
    {
        Task<PaginatedResult<RoleResponse>> GetAllPaginationAsync(RoleRequest request);

        Task<Result<RoleDetailDto>> GetByIdAsync(string id);

        Task<IResult> AddAsync(RoleDetailDto request);

        Task<IResult> UpdateAsync(RoleDetailDto request);

        Task<IResult> DeleteAsync(string id);

        Task<Result<List<FunctionWithRoleDto>>> GetListFunctionWithRole(string roleId);

        Task<IResult> SavePermission(List<FunctionWithRoleDto> request, string roleId);

        public Task<bool> CheckPermission(string functionId, string action, string[] roles);
    }

    public class RoleService(ApplicationDbContext dbContext, RoleManager<AppRole> roleManager, IMapper mapper) : IRoleService
    {
        public async Task<PaginatedResult<RoleResponse>> GetAllPaginationAsync(RoleRequest request)
        {
            var query = from r in dbContext.Roles.Where(x => !x.IsDeleted)
                        where string.IsNullOrEmpty(request.Keyword)
                              || r.Name.ToLower().Contains(request.Keyword.ToLower())
                        select new RoleResponse
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            CreatedOn = r.CreatedOn,
                            CreatedBy = r.CreatedBy
                        };

            var totalRecord = query.Count();

            var result = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            return PaginatedResult<RoleResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<RoleDetailDto>> GetByIdAsync(string id)
        {
            var role = await dbContext.Roles.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = mapper.Map<RoleDetailDto>(role);

            return await Result<RoleDetailDto>.SuccessAsync(result ?? new RoleDetailDto());
        }

        public async Task<IResult> AddAsync(RoleDetailDto request)
        {
            var role = new AppRole(request.Name, request.Description ?? "");

            await roleManager.CreateAsync(role);
            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }

        public async Task<IResult> UpdateAsync(RoleDetailDto request)
        {
            var role = await dbContext.Roles.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (role == null) return await Result.FailAsync(MessageConstants.NotFound);

            role.Name = request.Name;
            role.Description = request.Description;

            await roleManager.UpdateAsync(role);
            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> DeleteAsync(string id)
        {
            var role = await dbContext.Roles.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (role == null) return await Result.FailAsync(MessageConstants.NotFound);

            role.IsDeleted = true;
            dbContext.Roles.Update(role);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }

        public async Task<Result<List<FunctionWithRoleDto>>> GetListFunctionWithRole(string roleId)
        {
            var query = await (from f in dbContext.Functions.Where(x => !x.IsDeleted)
                               join p in dbContext.Permissions.Where(x => !x.IsDeleted && x.RoleId == roleId)
                                   on f.FunctionId equals p.FunctionId into fp
                               from p in fp.DefaultIfEmpty()
                               select new FunctionWithRoleDto()
                               {
                                   RoleId = roleId,
                                   FunctionId = f.FunctionId,
                                   CanCreate = p != null && p.CanCreate,
                                   CanDelete = p != null && p.CanDelete,
                                   CanRead = p != null && p.CanRead,
                                   CanUpdate = p != null && p.CanUpdate
                               }).ToListAsync();
            return await Result<List<FunctionWithRoleDto>>.SuccessAsync(query);
        }

        public async Task<IResult> SavePermission(List<FunctionWithRoleDto> request, string roleId)
        {
            var permissions = mapper.Map<List<Permission>>(request);
            var oldPermission = dbContext.Permissions.Where(x => x.RoleId == roleId).ToList();
            if (oldPermission.Count > 0)
            {
                dbContext.Permissions.RemoveRange(oldPermission);
                await dbContext.SaveChangesAsync();
            }
            // Add new permission
            await dbContext.Permissions.AddRangeAsync(permissions);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        /// <summary>
        /// Check permission
        /// </summary>
        /// <param name="functionId"></param>
        /// <param name="action"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(string functionId, string action, string[] roles)
        {
            var result = await (from f in dbContext.Functions.Where(x => !x.IsDeleted)
                                join p in dbContext.Permissions.Where(x => !x.IsDeleted)
                                    on f.FunctionId equals p.FunctionId
                                join r in roleManager.Roles on p.RoleId equals r.Id
                                where roles.Contains(r.Name) && f.FunctionId == functionId
                                                             && ((p.CanCreate && action == "Create")
                                                                 || (p.CanUpdate && action == "Update")
                                                                 || (p.CanDelete && action == "Delete")
                                                                 || (p.CanRead && action == "Read"))
                                select p).AnyAsync();
            return result;
        }
    }
}