using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gokhan.Core.Entity;
using Gokhan.Core.Services;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
        public class MaturityEntryServices : BaseEntryServices<MaturityEntryEntity>, IMaturityEntryServices
        {
        public MaturityEntryServices(IConfiguration config):base(config)
        {
            var _unitOfWork = new Finance.Data.FinanceUnitOfWork(config);
            var Mapper =Gokhan.Core.Services.MappingConfiguration.CreateMapping(typeof(MaturityEntryServices));
            Initialize(Mapper,_unitOfWork.MaturityEntryRepository,_unitOfWork);
        }
        public  override MaturityEntryEntity Add(MaturityEntryEntity rol)
        {
            return this.GenericAdd<MaturityEntry>(rol);
        }
        public override void Delete(int id)
        {
            this.GenericDelete<MaturityEntry>(id);
        }
        public override PaginatedList<MaturityEntryEntity> GetAll(QueryParameters queryParameters)
        {
            return this.GenericGetAll<MaturityEntry>(queryParameters);
        }
        public override MaturityEntryEntity GetSingle(int id)
        {
            return this.GenericGetSingle<MaturityEntry>(id);
        }

        public async Task<PaginatedList<maturityEntryListModel>> getVouchers(VoucherPostRequestModel model)
        {
            //var argument = Expression.Parameter(typeof(Voucher));
            IQueryable<Voucher> voucherQueryable = (this.GetUow() as Ishop.Core.Finance.Data.FinanceUnitOfWork).VoucherRepository.GetManyQueryable();
            // IQueryable<PaymentSummary> paymentSummaryQueryable = _financeUnitOfWork.PaymentSummaryRepository.GetManyQueryable();
            FinanceDbContext context = (this.GetUow() as Ishop.Core.Finance.Data.FinanceUnitOfWork).GetContext();
            if (model.startDate > DateTime.MinValue) {
                voucherQueryable = voucherQueryable.Where(p=> (p.yearNo >= model.startDate.Year && p.monthNo >= model.startDate.Month) 
                                                          ||  (p.yearNo <= model.endDate.Year && p.monthNo <= model.endDate.Month)
                                                          );
            }
            
/*             if (model.unitNo > 0 ) {
                voucherQueryable = voucherQueryable.Where(p=> p.unitNo == model.unitNo);
            } */

            List<int> voucherTypes = new List<int>();
            voucherTypes.Add(3);
            voucherTypes.Add(4);
            voucherTypes.Add(1);
            List<int> voucherTypesInVoucherTable = new List<int>();
            voucherTypesInVoucherTable.AddRange(voucherTypes);

            IQueryable<maturityEntryListModel> query =    from    table in voucherQueryable
                            join paySum in context.PaymentSummary on table.voucherNo equals paySum.voucherNo into paySumInto
                            from paySum in paySumInto.DefaultIfEmpty()
                            join paymentVoucher in context.PaymentVoucher on 
                                                table.voucherNo equals paymentVoucher.voucherNo
                                                into paymentVoucherInto
                            from paymentVoucher in paymentVoucherInto.DefaultIfEmpty()
                            join firma in context.Company on paymentVoucher.companyNo equals firma.id into firmaInto
                            from firma in firmaInto.DefaultIfEmpty()
                            where   ( model.onlyNotDues == true ? !(from maturity in context.MaturityEntry
                                     select maturity.voucherNo).Contains(table.voucherNo) : true) &&
                                    table.account.debitOrCredit == "C" &&
                                    table.rowNo == 0 &&
                                    (model.companyId > 0 ? firma.id == model.companyId : true) &&
                                    (model.unitNo > 0 ? context.getResourceTreeChilds(model.unitNo,1,3,true).Select(p=>p.ITEM_ID)
                                                       .Contains(table.unitNo) : true) &&
                                    voucherTypesInVoucherTable.Contains(table.voucherType) &&
                                    table.voucherStatus != VoucherStatus.Paid
                            select( new maturityEntryListModel() { voucherNo = table.voucherNo,
                                         yearNo = table.yearNo, monthNo = table.monthNo,
                                         isCancelled = table.isCancelled,
                                         grossAmount = table.grossAmount,
                                         dmisFisNo = table.dmisFisNo,dmisNetTutar = table.dmisNetTutar,
                                         accountName = table.account.accountName
                                         ,firmaAdi = firma.firmaAd 
                                         } );
            var pageQuery = query.GroupBy(g => g.voucherNo).Count();
            var pageSize = 50;
            var skip = (model.page - 1) * pageSize;  
            var totalCount = pageQuery;
            var voucherList =  query.OrderBy(o=> o.voucherNo).Skip(skip).Take(pageSize).ToList();
            var paginatedList = new Gokhan.Core.Services.PaginatedList<maturityEntryListModel>(voucherList,totalCount,model.page,pageSize);

            return paginatedList;
        }

        public override MaturityEntryEntity Update(MaturityEntryEntity item)
        {
            return this.GenericUpdate<MaturityEntry>(item);
        }
    }
}