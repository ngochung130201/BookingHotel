using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Function;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IFunctionService
    {
        Task<Result<List<FunctionResponse>>> GetAll();

        Task<Result<FunctionDetailDto>> GetById(string functionId);

        Task<IResult> Add(FunctionDetailDto request);

        Task<IResult> Update(FunctionDetailDto request);

        Task<IResult> Delete(string id);

        Task<IResult> UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items);

        Task<Result<bool>> CheckExistedId(string id);

        Task<IResult> ReOrder(string sourceId, string targetId);

        Task<Result<List<FunctionResponse>>> GetAllWithRole(string roleName);
    }

    public class FunctionService(ApplicationDbContext dbContext, IMapper mapper) : IFunctionService
    {
        public async Task<Result<List<FunctionResponse>>> GetAll()
        {
            var query = await dbContext.Functions.Where(x => !x.IsDeleted).OrderBy(x => x.Id).ToListAsync();

            var result = mapper.Map<List<FunctionResponse>>(query);

            return await Result<List<FunctionResponse>>.SuccessAsync(result);
        }

        public async Task<Result<FunctionDetailDto>> GetById(string functionId)
        {
            var user = await dbContext.Functions.Where(x => !x.IsDeleted && x.FunctionId == functionId).FirstOrDefaultAsync();

            var result = mapper.Map<FunctionDetailDto>(user);

            return await Result<FunctionDetailDto>.SuccessAsync(result ?? new FunctionDetailDto());
        }

        public async Task<IResult> Add(FunctionDetailDto request)
        {
            var function = mapper.Map<Function>(request);
            await dbContext.Functions.AddAsync(function);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }

        public async Task<IResult> Update(FunctionDetailDto request)
        {
            var function = await dbContext.Functions.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (function == null) return await Result.FailAsync(MessageConstants.NotFound);

            var updateFunction = mapper.Map(request, function);

            dbContext.Functions.Update(updateFunction);
            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> Delete(string id)
        {
            var function = await dbContext.Functions.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.FunctionId == id);

            if (function == null) return await Result.FailAsync(MessageConstants.NotFound);

            function.IsDeleted = true;
            dbContext.Functions.Update(function);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }

        public async Task<IResult> UpdateParentId(string sourceId, string targetId, Dictionary<string, int> items)
        {
            try
            {
                //Update parent id for source
                var category = await dbContext.Functions.FirstOrDefaultAsync(x => x.FunctionId.Equals(sourceId));
                if (category == null) return await Result.FailAsync(MessageConstants.NotFound);
                category.ParentId = targetId;
                dbContext.Functions.Update(category);
                await dbContext.SaveChangesAsync();

                //Get all sibling
                var sibling = await dbContext.Functions.Where(x => items.ContainsKey(x.FunctionId)).ToListAsync();
                foreach (var child in sibling)
                {
                    child.SortOrder = items[child.FunctionId];
                    dbContext.Functions.Update(child);
                }
                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Result<bool>> CheckExistedId(string id)
        {
            var existed = await dbContext.Functions.AnyAsync(x => x.FunctionId.Equals(id));
            return await Result<bool>.SuccessAsync(existed); ;
        }

        public async Task<IResult> ReOrder(string sourceId, string targetId)
        {
            var functions = await dbContext.Functions
                .Where(x => x.FunctionId.Equals(sourceId) || x.FunctionId.Equals(targetId))
                .ToListAsync();

            if (functions.Count != 2)
            {
                return await Result.FailAsync(MessageConstants.NotFound);
            }

            var source = functions.Find(x => x.FunctionId.Equals(sourceId));
            var target = functions.Find(x => x.FunctionId.Equals(targetId));

            if (source != null && target != null)
            {
                (source.SortOrder, target.SortOrder) = (target.SortOrder, source.SortOrder);
                // Update the records in memory
                dbContext.Functions.UpdateRange(functions);
                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
            }

            return await Result.FailAsync(MessageConstants.NotFound);
        }

        public async Task<Result<List<FunctionResponse>>> GetAllWithRole(string roleName)
        {
            var result = await (from f in dbContext.Functions
                                join p in dbContext.Permissions
                                    on f.FunctionId equals p.FunctionId
                                join r in dbContext.Roles
                                    on p.RoleId equals r.Id
                                where !string.IsNullOrEmpty(r.Name) && r.Name.Contains(roleName)
                                    && ((p.CanCreate == true)
                                        || (p.CanUpdate == true)
                                        || (p.CanDelete == true)
                                        || (p.CanRead == true))
                                select new FunctionResponse
                                {
                                    FunctionId = p.FunctionId,
                                    IconCss = f.IconCss,
                                    Name = f.Name,
                                    ParentId = f.ParentId,
                                    SortOrder = f.SortOrder,
                                    URL = f.URL,
                                    CreatedOn = f.CreatedOn
                                }).OrderBy(x => x.SortOrder).ToListAsync();
            return await Result<List<FunctionResponse>>.SuccessAsync(result);
        }
    }
}