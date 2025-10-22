using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RoboRent_BE.Model.DTOS.Admin;
using RoboRent_BE.Model.DTOs.EventSchedule;
using RoboRent_BE.Model.DTOS.RentalContract;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.Mapping;

public class MappingProfile : Profile
{
    private readonly UserManager<ModifyIdentityUser> _userManager;

    public MappingProfile()
    {
        CreateMap<CreateOrderRequest, Rental>()
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<UpdateOrderRequest, Rental>();
        
        CreateMap<Rental, OrderResponse>();

        CreateMap<CreateEventScheduleRequest, EventSchedule>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.EventDate));

        CreateMap<UpdateScheduleEventRequest, EventSchedule>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.EventDate));

        CreateMap<EventSchedule, EventScheduleResponse>();

        CreateMap<ModifyIdentityUser, UpdateUserResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        CreateMap<Account, UpdateUserResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.gender, opt => opt.MapFrom(src => src.gender))
            .ForMember(dest => dest.IdentificationIsValidated, opt => opt.MapFrom(src => src.IdentificationIsValidated))
            .ForMember(dest => dest.isDeleted, opt => opt.MapFrom(src => src.isDeleted))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // Rental Detail mappings
        CreateMap<CreateRentalDetailRequest, RentalDetail>();
        CreateMap<UpdateRentalDetailRequest, RentalDetail>();
        CreateMap<RentalDetail, RentalDetailResponse>()
            .ForMember(dest => dest.RoboTypeName, opt => opt.MapFrom(src => src.RoboType != null ? src.RoboType.TypeName : null))
            .ForMember(dest => dest.RentalEventName, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.EventName : null));

        // Rental Contract mappings
        CreateMap<CreateRentalContractRequest, RentalContract>();
        CreateMap<UpdateRentalContractRequest, RentalContract>();
        CreateMap<RentalContract, RentalContractResponse>()
            .ForMember(dest => dest.RentalEventName, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.EventName : null));
    }
}