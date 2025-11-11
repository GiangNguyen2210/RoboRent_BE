using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RoboRent_BE.Model.DTOs.ActivityType;
using RoboRent_BE.Model.DTOS.Admin;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Model.DTOS.DraftClauses;
using RoboRent_BE.Model.DTOs.EventActivity;
using RoboRent_BE.Model.DTOs.EventSchedule;
using RoboRent_BE.Model.DTOS.RentalContract;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.DTOs.RoboTypeOfActivity;
using RoboRent_BE.Model.DTOS.TemplateClauses;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.Mapping;

public class MappingProfile : Profile
{
    private readonly UserManager<ModifyIdentityUser> _userManager;

    public MappingProfile()
    {
        CreateMap<RobotTypeOfActivity, RobotTypeOfActivityResponse>()
            .ForMember(dest => dest.RoboTypeName, opt => opt.MapFrom(src => src.RoboType.TypeName));
        
        CreateMap<EventActivity, EventActivityResponse>();

        CreateMap<ActivityType, ActivityTypeResponse>();
        
        CreateMap<CreateOrderRequest, Rental>()
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<UpdateOrderRequest, Rental>();
        
        CreateMap<Rental, OrderResponse>()
            .ForMember(dest => dest.EventActivityName, opt => opt.MapFrom(src => src.EventActivity.Name))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.Name));

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
        CreateMap<RentalDetail, RentalDetailResponse>();
        // Rental Contract mappings
        CreateMap<CreateRentalContractRequest, RentalContract>();
        CreateMap<UpdateRentalContractRequest, RentalContract>();
        CreateMap<RentalContract, RentalContractResponse>()
            .ForMember(dest => dest.RentalEventName, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.EventName : null));

        // Draft Clauses mappings
        CreateMap<CreateDraftClausesRequest, DraftClauses>();
        CreateMap<UpdateDraftClausesRequest, DraftClauses>();
        CreateMap<DraftClauses, DraftClausesResponse>()
            .ForMember(dest => dest.ContractDraftTitle, opt => opt.MapFrom(src => src.ContractDraft != null ? src.ContractDraft.Title : null))
            .ForMember(dest => dest.TemplateClauseTitle, opt => opt.MapFrom(src => src.TemplateClause != null ? src.TemplateClause.Title : null));

        // Contract Drafts mappings
        CreateMap<CreateContractDraftsRequest, ContractDrafts>();
        CreateMap<UpdateContractDraftsRequest, ContractDrafts>();
        CreateMap<ContractDrafts, ContractDraftsResponse>()
            .ForMember(dest => dest.ContractTemplateTitle, opt => opt.MapFrom(src => src.ContractTemplate != null ? src.ContractTemplate.Title : null))
            .ForMember(dest => dest.RentalEventName, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.EventName : null))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? src.Staff.FullName : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

        // Contract Templates mappings
        CreateMap<CreateContractTemplatesRequest, ContractTemplates>();
        CreateMap<UpdateContractTemplatesRequest, ContractTemplates>();
        CreateMap<ContractTemplates, ContractTemplatesResponse>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Created != null ? src.Created.FullName : null))
            .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.Updated != null ? src.Updated.FullName : null));

        // Template Clauses mappings
        CreateMap<CreateTemplateClausesRequest, TemplateClauses>();
        CreateMap<UpdateTemplateClausesRequest, TemplateClauses>();
        CreateMap<TemplateClauses, TemplateClausesResponse>()
            .ForMember(dest => dest.ContractTemplateTitle, opt => opt.MapFrom(src => src.ContractTemplate != null ? src.ContractTemplate.Title : null));
    }
}