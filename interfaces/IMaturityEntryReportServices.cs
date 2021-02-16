using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IMaturityEntryReportServices{
        public Task<Gokhan.Core.Services.PaginatedList<maturityEntryReportResultModel>> getReport(maturityEntryReportRequestModel model);
    } 
}