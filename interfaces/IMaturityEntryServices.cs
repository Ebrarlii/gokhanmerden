using System.Threading.Tasks;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IMaturityEntryServices{
        Task<Gokhan.Core.Services.PaginatedList<maturityEntryListModel>> getVouchers(VoucherPostRequestModel model); 
    } 
}