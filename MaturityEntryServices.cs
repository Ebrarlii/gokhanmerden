using Gokhan.Core.Entity;
using Gokhan.Core.Services;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
        public class MaturityEntryServices : BaseEntryServices<MaturityEntryEntity>
        {
        public MaturityEntryServices(IConfiguration config):base(config)
        {
            var _unitOfWork = new Finance.Data.FinanceUnitOfWork(config);
            var Mapper =Gokhan.Core.Services.MappingConfiguration.CreateMapping(typeof(MaturityEntryServices));
            Initialize(Mapper,_unitOfWork.MaturityEntryRepository,_unitOfWork);
        }
        public  override MaturityEntryEntity Add(MaturityEntryEntity rol)
        {
            return this.GenericAdd<MaturityEntry>(rol);
        }
        public override void Delete(int id)
        {
            this.GenericDelete<MaturityEntry>(id);
        }
        public override PaginatedList<MaturityEntryEntity> GetAll(QueryParameters queryParameters)
        {
            return this.GenericGetAll<MaturityEntry>(queryParameters);
        }
        public override MaturityEntryEntity GetSingle(int id)
        {
            return this.GenericGetSingle<MaturityEntry>(id);
        }
        public override MaturityEntryEntity Update(MaturityEntryEntity item)
        {
            return this.GenericUpdate<MaturityEntry>(item);
        }
    }
}