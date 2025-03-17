using AutoMapper;
using NDTCore.OpenTelemetry.Contact.Interfaces.ServiceClients.Product.Dtos;
using NDTCore.OpenTelemetry.Domain.Entities;

namespace NDTCore.OpenTelemetry.Contact
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
