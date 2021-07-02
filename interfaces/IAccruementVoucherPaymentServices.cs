using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    //////////////////////////
    // Tahakkuk Ödeme İşlemleri Arayüzü
    //////////////////////////
    public interface IAccruementVoucherPaymentServices {
           List<PayVoucherResultModel> PayVoucher(List<PayVoucherRequestModel> model);
    }
}