using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface ICompanyServices {
        Task<Gokhan.Core.Services.PaginatedList<CompanyEntity>> getCompanies(CompanyRequestModel model); 
        Task<CombineCompanyResultModel> combineCompanies(CombineCompanyRequestModel model);
    } 
}