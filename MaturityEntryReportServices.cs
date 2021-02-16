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
            IQueryable<MaturityEntry> maturityEntryQueryable = _financeUnitOfWork.MaturityEntryRepository.GetManyQueryable();
            FinanceDbContext context = _financeUnitOfWork.GetContext();

            IQueryable<maturityEntryReportResultModel> result =    from    table in maturityEntryQueryable
                            join paySum in context.PaymentSummary on table.voucherNo equals paySum.voucherNo into paySumInto
                            from paySum in paySumInto.DefaultIfEmpty()
                            join paymentVoucher in context.PaymentVoucher on table.voucherNo equals paymentVoucher.voucherNo into paymentVoucherInto
                            from paymentVoucher in paymentVoucherInto.DefaultIfEmpty()
                            join firma in context.Company on paymentVoucher.companyNo equals firma.id into firmaInto
                            from firma in firmaInto.DefaultIfEmpty()
                            where table.maturityDate>=model.startDate && table.maturityDate <= model.endDate 
                            select ( new maturityEntryReportResultModel(){
                                vergiNo     = firma.vergiNo, 
                                firmaAdi    = firma.firmaAd,
                                voucherNo   = table.voucherNo,
                                faturaTarihi= context.P300INVOICE_INVOICE_DATE(table.voucherNo),
                                yevmiyeTarihi = table.maturityDate,
                                vadeGunu = table.maturityDay,
                                feragatDurumu = table.renunciationStatus,
                                giderTuru = paymentVoucher.accountName,
                                odenecekTutar = 1
                            } );

            var voucherList =  await _financeUnitOfWork.GetQueryableToList(result);
            var paginatedList = new Gokhan.Core.Services.PaginatedList<maturityEntryReportResultModel>(voucherList,voucherList.Count,1,50);

            return paginatedList; 
        }
    }
}