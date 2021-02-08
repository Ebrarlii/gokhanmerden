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
        public async Task<PaginatedList<CompanyEntity>> getCompanies(CompanyRequestModel model)
        {
           IEnumerable<CompanyEntity> result = await _financeUnitOfWork.CompanyRepository.GetListAsync<CompanyEntity>(selector: s=> new CompanyEntity(s.id,s.firmaNo,s.firmaAd,s.vergiNo),predicate: p=> p.firmaAd.Contains(model.name));
           
           return new PaginatedList<CompanyEntity>(result,result.Count(),1,50);
        }
    }
}