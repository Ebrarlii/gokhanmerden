using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IVoucherServices {
        Task<Gokhan.Core.Services.PaginatedList<VoucherExpensesEntity>> getVouchers(VoucherPostRequestModel model); 
    } 
}