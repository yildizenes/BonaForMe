using AutoMapper;
using BonaForMe.DomainCore.DBModel;

namespace BonaForMe.DomainCore.DTO
{
    class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Announcement, AnnouncementDto>()
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : ""));
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

            CreateMap<Order, OrderDto>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.User != null ? x.User.FirstName + " " + x.User.LastName : ""))
                .ForMember(x => x.OrderStatusName, opt => opt.MapFrom(x => x.OrderStatus != null ? x.OrderStatus.Name : ""));
            CreateMap<OrderDto, Order>();

            CreateMap<OrderStatus, OrderStatusDto>();
            CreateMap<OrderStatusDto, OrderStatus>();

            CreateMap<PaymentInfo, PaymentInfoDto>();
            CreateMap<PaymentInfoDto, PaymentInfo>();

            CreateMap<Product, ProductDto>()
                .ForMember(x => x.ProductUnitName, opt => opt.MapFrom(x => x.ProductUnit != null ? x.ProductUnit.Name : ""))
                .ForMember(x => x.CurrencyUnitName, opt => opt.MapFrom(x => x.CurrencyUnit != null ? x.CurrencyUnit.Name : ""))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : ""));
            CreateMap<ProductDto, Product>();

            CreateMap<ProductUnit, ProductUnitDto>();
            CreateMap<ProductUnitDto, ProductUnit>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<ApplicationSetting, ApplicationSettingDto>();
            CreateMap<ApplicationSettingDto, ApplicationSetting>();

            CreateMap<CampaignProduct, CampaignProductDto>()
                .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product != null ? x.Product.Name : ""));
            CreateMap<CampaignProductDto, CampaignProduct>();

            CreateMap<CourierCoordinate, CourierCoordinateDto>();
            CreateMap<CourierCoordinateDto, CourierCoordinate>();

            CreateMap<SpecialPrice, SpecialPriceDto>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.User != null ? x.User.FullName : ""))
                .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product != null ? x.Product.Name : ""));
            CreateMap<SpecialPriceDto, SpecialPrice>();

            CreateMap<OrderLog, OrderLogDto>();
            CreateMap<OrderLogDto, OrderLog>();

            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailDto, OrderDetail>();

            CreateMap<WorkPartner, WorkPartnerDto>();
            CreateMap<WorkPartnerDto, WorkPartner>();

            CreateMap<OrderHour, OrderHourDto>();
            CreateMap<OrderHourDto, OrderHour>();
        }
    }
}