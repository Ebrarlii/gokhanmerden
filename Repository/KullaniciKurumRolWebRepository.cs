using System.Collections.Generic;
using System.Threading.Tasks;
using Gokhan.Core.Repository;
using Ishop.Core.EkOdeme.Entity;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Repository
{
    public class KullaniciKurumRolWebRepository: HttpClientRepository {

        FinanceAppSettings _financeAppSettings;
        public KullaniciKurumRolWebRepository(FinanceAppSettings financeAppSettings) {
            
            _financeAppSettings = financeAppSettings;
            this.setRequestUri(_financeAppSettings.EkOdemeServiceUrl);
        }
        public override async Task<List<KullaniciKurumRolAtamaEntity>> get<KullaniciKurumRolAtamaEntity>(string query)
        {
            return await base.get<KullaniciKurumRolAtamaEntity>(query);
        }
    }
}