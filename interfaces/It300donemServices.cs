using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public interface It300donemServices {
        Task<List<int?>> menuEnabled(menuEnabledPostModel mode);
    }
}