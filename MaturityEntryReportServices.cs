using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
    public class MaturityEntryReportServices : IMaturityEntryReportServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        public MaturityEntryReportServices(IConfiguration config){
           _financeUnitOfWork = new FinanceUnitOfWork(config);
        }

        public async Task<Gokhan.Core.Services.PaginatedList<maturityEntryReportResultModel>> getReport(maturityEntryReportRequestModel model)
        {
            IQueryable<Voucher> voucherQueryable = _financeUnitOfWork.VoucherRepository.GetManyQueryable();
            FinanceDbContext context = _financeUnitOfWork.GetContext();

            IQueryable<maturityEntryReportResultModel> result =    from    table in voucherQueryable
                            join paySum in context.PaymentSummary on table.voucherNo equals paySum.voucherNo into paySumInto
                            from paySum in paySumInto.DefaultIfEmpty()
                            join maturity in context.MaturityEntry on table.voucherNo equals maturity.voucherNo into maturitySumInto
                            from maturity in maturitySumInto.DefaultIfEmpty()
                            join vouchers in context.Voucher on new { voucherNo= table.voucherNo, rowno = 0 }  
                                                                equals 
                                                                new { voucherNo= vouchers.voucherNo, rowno = vouchers.rowNo}  
                                                                into vouchersInto
                            from vouchers in vouchersInto.DefaultIfEmpty()
                            join paymentVoucher in context.PaymentVoucher on table.voucherNo equals paymentVoucher.voucherNo into paymentVoucherInto
                            from paymentVoucher in paymentVoucherInto.DefaultIfEmpty()
                            join firma in context.Company on paymentVoucher.companyNo equals firma.id into firmaInto
                            from firma in firmaInto.DefaultIfEmpty()
                            where   paySum.balance > 0 &&
                                    table.rowNo == 0
                            select ( new maturityEntryReportResultModel(){
                                vergiNo     = firma.vergiNo, 
                                firmaAdi    = firma.firmaAd,
                                voucherNo   = table.voucherNo,
                                faturaTarihi= context.P300INVOICE_INVOICE_DATE(table.voucherNo),
                                yevmiyeTarihi = maturity.maturityDate,
                                vadeGunu = maturity.maturityDay,
                                feragatDurumu = maturity.renunciationStatus,
                                giderTuru = paymentVoucher.accountName,
                                odenecekTutar = table.dmisNetTutar == null ? 0 : table.dmisNetTutar.Value
                            } );

            var voucherList =  await _financeUnitOfWork.GetQueryableToList(result);
            var paginatedList = new Gokhan.Core.Services.PaginatedList<maturityEntryReportResultModel>(voucherList,voucherList.Count,1,50);

            return paginatedList; 
        }
    }
}