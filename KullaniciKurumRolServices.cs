using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gokhan.Core.Services;
using Ishop.Core.EkOdeme.Entity;
using Ishop.Core.Finance.Entity;
using Ishop.Core.Finance.Repository;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
    public class KullaniciKurumRolServices : IKullaniciKurumRolServices
    {
        
        KullaniciKurumRolWebRepository _kullaniciKurumRolWebRepository;
        public KullaniciKurumRolServices(FinanceAppSettings financeAppSettings){
            _kullaniciKurumRolWebRepository = new KullaniciKurumRolWebRepository(financeAppSettings);
        }
        
        public async Task<List<KullaniciKurumRolAtamaEntity>> getByPersonelId(int personelId)
        {
            return await _kullaniciKurumRolWebRepository.get<KullaniciKurumRolAtamaEntity>(String.Format("KullaniciKurumRolAtama?query=PersonelID={0}&Page=-1&include=kurum,rol", personelId));
        }
    }
}
