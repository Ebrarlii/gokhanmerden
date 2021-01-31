using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using AutoMapper;
using System.Linq.Expressions;


namespace Ishop.Core.Finance.Services
{
    public class VoucherServices : IVoucherServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        IMapper _mapper;
        public VoucherServices(IConfiguration config){
            _financeUnitOfWork = new FinanceUnitOfWork(config);
            _mapper =Gokhan.Core.Services.MappingConfiguration.CreateMapping(typeof(VoucherServices));

        } 
        public async Task<Gokhan.Core.Services.PaginatedList<VoucherExpensesEntity>> getVouchers(int YearNo, byte MonthNo, int UnitNo)
        {
            var argument = Expression.Parameter(typeof(Voucher));
            IQueryable<Voucher> voucherQueryable = _financeUnitOfWork.VoucherRepository.GetManyQueryable();
            IQueryable<PaymentSummary> paymentSummaryQueryable = _financeUnitOfWork.PaymentSummaryRepository.GetManyQueryable();
            FinanceDbContext context = _financeUnitOfWork.GetContext();
            //Expression<Func<Voucher, bool>> query = null;
            if (YearNo > 0) {
                //var expressYear = Expression.Lambda<Func<Voucher, bool>>(Expression.Equal(queryYear,Expression.Constant(YearNo)), new[] {argument});
                voucherQueryable = voucherQueryable.Where(p=>p.yearNo == YearNo);
                
            }
            if (MonthNo > 0) {
                voucherQueryable = voucherQueryable.Where(p=>p.monthNo == MonthNo);
            }
            if (UnitNo > 0 ) {
                voucherQueryable = voucherQueryable.Where(p=> p.unitNo == UnitNo);
            }
            List<int> voucherTypes = new List<int>();
            voucherTypes.Add(3);
            voucherTypes.Add(4);
            voucherTypes.Add(5);
            voucherTypes.Add(6);
            List<int> voucherTypesInVoucherTable = new List<int>();
            voucherTypesInVoucherTable.AddRange(voucherTypes);
            voucherTypesInVoucherTable.Add(7);
            voucherTypesInVoucherTable.Add(10);
            voucherTypesInVoucherTable.Add(11);
            voucherTypesInVoucherTable.Add(16);
            IQueryable<VoucherExpensesEntity> result =    from    table in voucherQueryable
                            join paySum in context.PaymentSummary on table.voucherNo equals paySum.voucherNo into paySumInto
                            from paySum in paySumInto.DefaultIfEmpty()
                            join resTreeU1 in context.ResourceTree on table.unitNo equals resTreeU1.id into resTreeU1Into
                            from resTreeU11 in resTreeU1Into.DefaultIfEmpty()
                            join resTreeU3 in context.ResourceTree on 
                                    context.getResourceTreeParent(table.voucherNo,2) equals resTreeU3.id  into resTreeU3Into
                            from resTreeU3 in resTreeU3Into.DefaultIfEmpty()
                            join resTreeU2 in context.ResourceTree on table.relatedUnitNo equals resTreeU2.id into resTreeU2Into
                            from resTreeU2 in resTreeU2Into.DefaultIfEmpty()
                            join voucherType in context.VoucherType on table.voucherType equals voucherType.typeNo into voucherTypeInto
                            from voucherType in voucherTypeInto.DefaultIfEmpty()
                            join monthF in context.MonthlyField on table.account.fieldNo equals monthF.fieldNo into monthFeildInto
                            from monthF in monthFeildInto.DefaultIfEmpty()
                            join paymentVoucher in context.PaymentVoucher on 
                                                new { voucherNo = table.voucherNo, voucherTypes = voucherTypes.Contains(table.voucherType)}
                                         equals new { voucherNo = paymentVoucher.voucherNo, voucherTypes = true} into paymentVoucherInto
                            from paymentVoucher in paymentVoucherInto.DefaultIfEmpty()
                            join firma in context.Firma on paymentVoucher.companyNo equals firma.id into firmaInto
                            from firma in firmaInto.DefaultIfEmpty()
                            where   table.account.debitOrCredit == "C" &&
                                    table.rowNo == 0 &&
                                    context.getResourceTreeChilds(UnitNo,1,3,true).Select(p=>p.ITEM_ID).Contains(table.unitNo) &&
                                    voucherTypesInVoucherTable.Contains(table.voucherType)
                            select( new VoucherExpensesEntity() { voucherNo = table.voucherNo, rowNo = table.rowNo, rowDate = table.rowDate,
                                         yearNo = table.yearNo, monthNo = table.monthNo,unitNo = table.unitNo, 
                                         voucherStatus = table.voucherStatus,voucherType = table.voucherType,accountNo = table.accountNo,
                                         prodOrCancel = table.prodOrCancel,isCancelled = table.isCancelled,
                                         comments = table.comments,netAmount = table.netAmount,grossAmount = table.grossAmount,
                                         kdvAmount = table.kdvAmount,kdvRate = table.kdvRate,typeName = voucherType.typeName,
                                         yearMonthNo = ((System.Convert.ToInt32(table.yearNo) * 100) + System.Convert.ToInt32(table.monthNo)),
                                         InvoiceNoList = context.P300INVOICE_INVOICE_NO(table.voucherStatus),
                                         InvoiceDateList = context.P300INVOICE_INVOICE_DATE(table.voucherNo),
                                         dmisFisNo = table.dmisFisNo,dmisNetTutar = table.dmisNetTutar,localeStatusDate = table.localeStatusDate, 
                                         localeStatusText = table.localeStatusText,
                                         paymentDate = paymentVoucher.paymentDate,paymentNo = paymentVoucher.paymentNo,
                                         DebtLedgerNoListBirim = context.P300DEBT_LEDGER_NO_LIST(table.voucherNo, 2),
                                         DebtLedgerNoList = context.P300DEBT_LEDGER_NO_LIST(table.voucherNo, 1),
                                         DebtLedgerSendDateListBirim = context.P300DEBT_LEDGER_SEND_DATE_LIST(table.voucherNo, 2),
                                         DebtLedgerSendDateList = context.P300DEBT_LEDGER_SEND_DATE_LIST(table.voucherNo, 1),
                                         accountCode = table.account.accountCode,accountName = table.account.accountName, 
                                         fieldName = monthF.fieldName,fieldNo = monthF.fieldNo,
                                         MainUnitName = resTreeU3.text, UnitName = resTreeU11.text, RelatedUnitName = resTreeU2.text,
                                         accrumentNo = paymentVoucher.accrumentNo,accrumentDate = paymentVoucher.accrumentDate, 
                                         TenderConfirmDate = paymentVoucher.TenderConfirmDate,
                                         TenderConfirmNo = paymentVoucher.TenderConfirmNo,tenderDate = paymentVoucher.tenderDate, 
                                         TenderMethodName = paymentVoucher.TenderMethodName,
                                         TenderMethodNo = paymentVoucher.TenderMethodNo,companyNo = paymentVoucher.companyNo, 
                                         firmaAd = firma.firmaAd,debitAmount = paySum.debitAmount,
                                         creditAmount = paySum.creditAmount,balance = paySum.balance } );
                            // CASE VO.VOUCHER_STATUS 
                                //WHEN 0 THEN '�ptal' 
                                //WHEN 1 THEN 'Tahakkuk Onaylama' 
                                //WHEN 2 THEN '�deme ��lemleri' WHEN 3 
                                //THEN '�demeler' ELSE CONVERT(NVARCHAR, VO.VOUCHER_STATUS) END AS VoucherStatusTitle, 
                                // ISNULL(PS.CREDIT_AMOUNT, 0) AS CreditAmount, ISNULL(PS.BALANCE, 0) AS Balance

            var voucherList =  await _financeUnitOfWork.GetQueryableToList(result);
            var paginatedList = new Gokhan.Core.Services.PaginatedList<VoucherExpensesEntity>(voucherList,voucherList.Count,1,50);
            //var viewModel = _mapper.Map<IEnumerable<Voucher>,IEnumerable<VoucherEntity>>(voucherList);

            return paginatedList;       
        }
    }
}


/*

 INNER JOIN T300ACCOUNT AC ON AC.ACCOUNT_NO = VO.ACCOUNT_NO AND AC.DEBIT_OR_CREDIT = 'C'
 INNER JOIN T300MONTHLY_FIELD MF ON MF.FIELD_NO = AC.FIELD_NO
 LEFT JOIN T300VOUCHER_TYPE VT ON VT.TYPE_NO = VO.VOUCHER_TYPE
 LEFT JOIN RESOURCE_TREE U3 ON U3.ITEM_ID = DBO.GET_RESOURCE_TREE_PARENT(VO.UNIT_NO, 2)
 LEFT JOIN RESOURCE_TREE U1 ON U1.ITEM_ID = VO.UNIT_NO
 LEFT JOIN RESOURCE_TREE U2 ON U2.ITEM_ID = VO.RELATED_UNIT_NO
 LEFT JOIN T300PAYMENT_VOUCHER PV ON PV.VOUCHER_NO = VO.VOUCHER_NO AND PV.ACCOUNT_NO <> 0 AND VO.VOUCHER_TYPE IN (3,4,5,6)
 LEFT JOIN GG_FIRMA CP ON CP.FIRMA_ID = PV.COMPANY_NO
 LEFT JOIN V300PAYMENT_SUMMARY PS ON PS.VOUCHER_NO = VO.VOUCHER_NO
 WHERE VO.YEAR_NO = 2021 AND VO.MONTH_NO = 1 AND VO.ROW_NO = 0 
 AND VO.VOUCHER_TYPE IN (3,4,5,6,7,10,11,16) AND VO.VOUCHER_STATUS <> 0 
 AND VO.UNIT_NO IN (SELECT 
UC.ITEM_ID
 FROM dbo.GET_RESOURCE_TREE_CHILDS(1598, 1, 3, 1) UC
)

*/