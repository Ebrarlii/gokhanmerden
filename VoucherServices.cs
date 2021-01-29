using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using AutoMapper;

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
        public async Task<IEnumerable<VoucherEntity>> getVouchers(int YearNo, int MonthNo, int UnitNo)
        {
            var voucherList =  await _financeUnitOfWork.VoucherRepository.GetListAsync<Voucher>(selector: s => s,
            predicate: p => p.yearNo == YearNo && p.monthNo == MonthNo && p.unitNo == UnitNo,
            orderBy: o => o.OrderBy(ob=> ob.voucherNo));

            var viewModel = _mapper.Map<IEnumerable<Voucher>,IEnumerable<VoucherEntity>>(voucherList);

            return viewModel;       
        }
    }
}