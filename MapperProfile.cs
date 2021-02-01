using AutoMapper;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile() {
            this.CreateMap<Voucher, VoucherEntity>().ReverseMap();
            this.CreateMap<MaturityEntry, MaturityEntryEntity>().ReverseMap();
        } 
    } 
}