using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.EkOdeme.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IKullaniciKurumRolServices{
        public Task<List<KullaniciKurumRolAtamaEntity>> getByPersonelId(int personelId);
    }
}