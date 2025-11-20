using Microsoft.Extensions.DependencyInjection;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interface;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Service.Services;

namespace RoboRent_BE.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IEventRoboTypeService, EventRoboTypeService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IModifyIdentityUserService, ModifyIdentityUserService>();
        services.AddTransient<IPriceQuoteService, PriceQuoteService>();
        services.AddTransient<IRentalDetailService, RentalDetailService>();
        services.AddTransient<IRentalService, RentalService>();
        services.AddTransient<IRobotService, RobotService>();
        services.AddTransient<IRoboTypeService, RoboTypeService>();
        services.AddTransient<ITypesOfRoboService, TypesOfRoboService>();
        services.AddTransient<IPayOSService, PayOSService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IRentalContractService, RentalContractService>();
        services.AddTransient<IDraftClausesService, DraftClausesService>();
        services.AddTransient<IContractDraftsService, ContractDraftsService>();
        services.AddTransient<IContractTemplatesService, ContractTemplatesService>();
        services.AddTransient<ITemplateClausesService, TemplateClausesService>();
        services.AddTransient<IActivityTypeService, ActivityTypeService>();
        services.AddTransient<IEventActivityService, EventActivityService>();
        services.AddTransient<IRobotTypeOfActivityService, RobotTypeOfActivityService>();
        services.AddTransient<IActivityTypeGroupService, ActivityTypeGroupService>();
        services.AddTransient<IGroupScheduleService, GroupScheduleService>();
        return services;
    }
}