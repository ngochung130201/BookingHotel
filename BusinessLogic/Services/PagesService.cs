using System.Linq.Dynamic.Core;
using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Pages;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IPagesService
    {
        Task<PaginatedResult<PagesResponse>> GetPagination(PagesRequest request);

        Task<Result<PagesDetailDto>> GetById(int id);

        Task<IResult> Add(PagesDetailDto request);

        Task<IResult> Update(PagesDetailDto request);

        Task<IResult> Delete(int id);
    }

    public class PagesService(ApplicationDbContext dbContext, IMapper mapper) : IPagesService
    {
        public async Task<PaginatedResult<PagesResponse>> GetPagination(PagesRequest request)
        {
            var query = dbContext.Pages.Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword) 
                                                                   || x.Title.ToLower().Contains(request.Keyword.ToLower()) 
                                                            || x.TitleVi.ToLower().Contains(request.Keyword.ToLower())));

            var totalRecord = query.Count();

            var pagesList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = mapper.Map<List<PagesResponse>>(pagesList);

            return PaginatedResult<PagesResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<PagesDetailDto>> GetById(int id)
        {
            var user = await dbContext.Pages.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = mapper.Map<PagesDetailDto>(user);

            return await Result<PagesDetailDto>.SuccessAsync(result ?? new PagesDetailDto());
        }

        public async Task<IResult> Add(PagesDetailDto request)
        {
            var pages = mapper.Map<Pages>(request);
            await dbContext.Pages.AddAsync(pages);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }

        public async Task<IResult> Update(PagesDetailDto request)
        {
            var pages = await dbContext.Pages.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (pages == null) return await Result.FailAsync(MessageConstants.NotFound);

            var updatePages = mapper.Map(request, pages);

            dbContext.Pages.Update(updatePages);
            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> Delete(int id)
        {
            var pages = await dbContext.Pages.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (pages == null) return await Result.FailAsync(MessageConstants.NotFound);

            pages.IsDeleted = true;
            dbContext.Pages.Update(pages);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }
    }
}