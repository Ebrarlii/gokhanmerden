using System.Threading.Tasks;
using Gokhan.Core.Services;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace Ishop.Core.Finance.Services
{
    public class CompanyServices : ICompanyServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        public CompanyServices(IConfiguration config, FinanceAppSettings financeAppSettings){
            _financeUnitOfWork = new FinanceUnitOfWork(config);
        }

        public async Task<CombineCompanyResultModel> combineCompanies(CombineCompanyRequestModel model)
        {
            CombineCompanyResultModel resultModel = new CombineCompanyResultModel();
            try
            {
                foreach (var companyId in model.sources)
            {
                var paymentVochers = await _financeUnitOfWork.PaymentVoucherRepository.GetListAsync(p=>p.companyNo == companyId, true);
                foreach (var paymentVocher in paymentVochers)
                {
                    paymentVocher.companyNo = model.target;
                    _financeUnitOfWork.PaymentVoucherRepository.Update(paymentVocher);
                }
                var company = _financeUnitOfWork.CompanyRepository.GetByID(companyId);
                company.isDeleted = true;
                _financeUnitOfWork.CompanyRepository.Update(company);
                await _financeUnitOfWork.SaveAync();
                resultModel.result = true;
                resultModel.message = "Firma birleştirme işlemi tamamlandı";
            }
            }
            catch (System.Exception ex)
            {
                resultModel.result = false;
                resultModel.message = ex.Message;
            }
            return resultModel;
        }

        public async Task<PaginatedList<CompanyEntity>> getCompanies(CompanyRequestModel model)
        {
           IEnumerable<CompanyEntity> result = await _financeUnitOfWork.CompanyRepository.GetListAsync<CompanyEntity>(selector: s=> new CompanyEntity(s.id,s.firmaNo,s.firmaAd,s.vergiNo),predicate: p=> (p.firmaAd.Contains(model.name) || p.vergiNo.Contains(model.vergiNo)) && p.isDeleted != true);
           
           return new PaginatedList<CompanyEntity>(result.Take(10),10,1,50);
        }
    }
}