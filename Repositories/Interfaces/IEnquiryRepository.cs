using com.zameen.Models;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Repositories.Interfaces;

public interface IEnquiryRepository : IGenericRepository<Enquiry, int>
{
    Task<PagedResult<Enquiry>> GetByPropertyIdAsync(int propertyId, int page, int size);
    Task<PagedResult<Enquiry>> GetAllEnquiryByAgentAsync(string propertyId, int page, int size);
}
