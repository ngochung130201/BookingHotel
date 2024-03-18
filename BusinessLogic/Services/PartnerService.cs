using BusinessLogic.Constants.Messages;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using AutoMapper;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Partners;

namespace BusinessLogic.Services
{
    public interface IPartnerService
    {
        Task<PaginatedResult<PartnerResponse>> GetPagination(PartnerRequest request);

        Task<Result<PartnerDetailDto>> GetById(int id);

        Task<IResult> Add(PartnerDetailDto request);

        Task<IResult> Update(PartnerDetailDto request);

        Task<IResult> Delete(int id);
    }

    public class PartnerService(ApplicationDbContext dbContext, IMapper mapper) : IPartnerService
    {
        public async Task<PaginatedResult<PartnerResponse>> GetPagination(PartnerRequest request)
        {
            var query = dbContext.Partners.Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                                                                   || x.Title.ToLower().Contains(request.Keyword.ToLower())
                                                            || x.TitleVi.ToLower().Contains(request.Keyword.ToLower()))
                                                            && (!request.Status.HasValue || request.Status.Value == x.Status));

            var totalRecord = query.Count();

            var partnersList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = mapper.Map<List<PartnerResponse>>(partnersList);

            return PaginatedResult<PartnerResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<PartnerDetailDto>> GetById(int id)
        {
            var partner = await dbContext.Partners.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = mapper.Map<PartnerDetailDto>(partner);

            return await Result<PartnerDetailDto>.SuccessAsync(result ?? new PartnerDetailDto());
        }

        public async Task<IResult> Add(PartnerDetailDto request)
        {
            var partner = mapper.Map<Partners>(request);
            await dbContext.Partners.AddAsync(partner);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }

        public async Task<IResult> Update(PartnerDetailDto request)
        {
            var partner = await dbContext.Partners.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (partner == null) return await Result.FailAsync(MessageConstants.NotFound);

            var updatePartner = mapper.Map(request, partner);

            dbContext.Partners.Update(updatePartner);
            await dbContext.SaveChangesAsync();

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> Delete(int id)
        {
            var partner = await dbContext.Partners.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (partner == null) return await Result.FailAsync(MessageConstants.NotFound);

            partner.IsDeleted = true;
            dbContext.Partners.Update(partner);
            await dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }
    }

}
