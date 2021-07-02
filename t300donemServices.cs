using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
    public class t300donemServices : It300donemServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        ResourceTreeServices _resourceTreeServices;
        public t300donemServices(IConfiguration config){
            FinanceAppSettings financeAppSettings= config.GetSection("AppSettings").Get<FinanceAppSettings>();
            _financeUnitOfWork = new FinanceUnitOfWork(config);
            _resourceTreeServices = new ResourceTreeServices(config,financeAppSettings);
        }
        public async Task<List<int?>> menuEnabled(menuEnabledPostModel model)
        {
                return await Task.Run(async () => {
                List<int> m_parentList = _resourceTreeServices.GetResourceTreeParents(model.MainUnitNo);
                t300donem m_donem =
                 await _financeUnitOfWork.T300donemRepository.GetFirstOrDefaultAsync(selector: s => s, predicate:
                p => p.Yil == model.YearNo && p.Ay == model.MonthNo &&
                p.t300preaccountresourcetreeassignments.Where(c => m_parentList.Contains(c.resource_tree.HasValue ? c.resource_tree.Value : 0)).Count() > 0,
                include: i => i.Include(r => r.t300preaccountresourcetreeassignments).Include(f => f.FisTurAtamalari).ThenInclude(t=>t.t300preaccountref));
                List<int?> result = new List<int?>();
                if (m_donem != null)
                {
                    result = (from tablo in m_donem.FisTurAtamalari
                            select tablo.t300preaccountref.id).ToList();
                }
                else
                {
                    result.Add(0);
                }
                return result;
            });
        }
    }
}