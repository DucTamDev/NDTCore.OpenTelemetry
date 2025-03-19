using AutoMapper;
using NDTCore.OpenTelemetry.Contract.Interfaces.ServiceClients.Product.Dtos;
using NDTCore.OpenTelemetry.Domain.Entities;

namespace NDTCore.OpenTelemetry.Contract
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
