using System.Composition;
using Gokhan.Core.Services;
using Ishop.Core.EkOdeme.Entity;
using Ishop.Core.Finance.Entity;
using Ishop.Core.Finance.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ishop.Core.Finance.Services
{
    [Export(typeof(Gokhan.Core.DependencyResolver.IComponent))]
    public class DependencyResolver : Gokhan.Core.DependencyResolver.IComponent
    {
        public void SetUp(IConfiguration config,IServiceCollection service)
        {
            FinanceAppSettings financeAppSettings= config.GetSection("AppSettings").Get<FinanceAppSettings>();
            service.AddScoped<IKullaniciKurumRolServices,KullaniciKurumRolServices>(s=> new KullaniciKurumRolServices(financeAppSettings));
            service.AddScoped<IResourceTreeServices,ResourceTreeServices>(s=> new ResourceTreeServices(config,financeAppSettings));
            service.AddScoped<IVoucherServices,VoucherServices>(s=> new VoucherServices(config));
        
        }
    }
}