using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IVoucherServices {
        Task<IEnumerable<VoucherExpensesEntity>> getVouchers(int YearNo,byte MonthNo,int UnitNo); 
    } 
}