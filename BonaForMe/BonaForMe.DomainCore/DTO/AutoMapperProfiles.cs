using AutoMapper;
using BonaForMe.DomainCore.DBModel;

namespace BonaForMe.DomainCore.DTO
{
    class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Announcement, AnnouncementDto>();
            CreateMap<AnnouncementDto, Announcement>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<ContactInformation, ContactInformationDto>();
            CreateMap<ContactInformationDto, ContactInformation>();

            CreateMap<ContactInformation, ContactInformationDto>();
            CreateMap<ContactInformationDto, ContactInformation>();

            CreateMap<CurrencyUnit, CurrencyUnitDto>();
            CreateMap<CurrencyUnitDto, CurrencyUnit>();

            CreateMap<LinkOrderProduct, LinkOrderProductDto>();
            CreateMap<LinkOrderProductDto, LinkOrderProduct>();

            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();

            CreateMap<OrderStatus, OrderStatusDto>();
            CreateMap<OrderStatusDto, OrderStatus>();

            CreateMap<PaymentInfo, PaymentInfoDto>();
            CreateMap<PaymentInfoDto, PaymentInfo>();

            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            CreateMap<ProductUnit, ProductUnitDto>();
            CreateMap<ProductUnitDto, ProductUnit>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
