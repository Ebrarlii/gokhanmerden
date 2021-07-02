using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.EkOdeme.Entity;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface IResourceTreeServices{
        Task<IEnumerable<ResourceTreeEntity>> getByPersonelId(int personelId);
        List<int> GetResourceTreeParents(int ChildUnitID);
    }
}