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
            service.AddScoped<IBaseEntryServices<MaturityEntryEntity>,MaturityEntryServices>(s=> new MaturityEntryServices(config));
            service.AddScoped<ICompanyServices,CompanyServices>(s=> new CompanyServices(config, financeAppSettings));
            service.AddScoped<IMaturityEntryReportServices,MaturityEntryReportServices>(s=> new MaturityEntryReportServices(config));
            service.AddScoped<IMaturityEntryServices, MaturityEntryServices>(s=> new MaturityEntryServices(config));
            service.AddScoped<It300donemServices, t300donemServices>(s=> new t300donemServices(config));
            service.AddScoped<ISequencesServices, SequenceServices>(s=> new SequenceServices(config));   
        }
    }
}