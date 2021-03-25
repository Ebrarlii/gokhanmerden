using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gokhan.Core.Services;
using Ishop.Core.EkOdeme.Entity;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Ishop.Core.Finance.Repository;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
    public class ResourceTreeServices : IResourceTreeServices
    {
        
        KullaniciKurumRolServices _kullaniciKurumServices;
        FinanceUnitOfWork _financeUnitOfWork;
        public ResourceTreeServices(IConfiguration config, FinanceAppSettings financeAppSettings){
            _kullaniciKurumServices = new KullaniciKurumRolServices(financeAppSettings);
            _financeUnitOfWork = new FinanceUnitOfWork(config);
        }

        public async Task<IEnumerable<ResourceTreeEntity>> getByPersonelId(int personelId)
        {
            List<KullaniciKurumRolAtamaEntity> _kullaniciKurumRols = await _kullaniciKurumServices.getByPersonelId(personelId);
            List<int> _kaynakList = _kullaniciKurumRols.Select(p=> p.kurum.KurumsalKaynakItemID).ToList();
            List<int> _atamaListId = _kullaniciKurumRols.Select(p=> p.KurumID).ToList();
            IEnumerable<ResourceTreeEntity> _resourceTrees;
            if (_kullaniciKurumRols.Where(predicate => predicate.rol.YoneticiMi == true).Count() > 0) {
                _resourceTrees = await _financeUnitOfWork.ResourceTreeRepository
                                        .GetListAsync<ResourceTreeEntity>(selector: s=> new ResourceTreeEntity(s.id,s.text),
                                        predicate: p =>  p.isDeleted != true && p.organizationId == 3 && p.enterpriseId == 1 && p.objectId > 0,orderBy: o=> o.OrderBy(ob=> ob.text));
            } else {
                _resourceTrees = await _financeUnitOfWork.ResourceTreeRepository
                                        .GetListAsync<ResourceTreeEntity>(selector: s=> new ResourceTreeEntity(s.id,s.text),
                                        predicate: p=> (_kaynakList.Contains(p.id) || _atamaListId.Contains(p.objectId)) && p.isDeleted != true && p.organizationId == 3,orderBy: o=> o.OrderBy(ob=> ob.text));

            }
            return _resourceTrees;
        }

        /*public async Task<List<KullaniciKurumRolAtamaEntity>> getByPersonelId(int personelId)
        {
            return await _kullaniciKurumRolWebRepository.get<KullaniciKurumRolAtamaEntity>(String.Format("KullaniciKurumRolAtama?query=PersonelID={0}&Page=-1&include=kurum", personelId));
        }*/
    }
}
