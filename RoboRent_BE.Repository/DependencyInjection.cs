using Microsoft.Extensions.DependencyInjection;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;

namespace RoboRent_BE.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IRoboTypeRepository, RoboTypeRepository>();
        services.AddTransient<IPriceQuoteRepository, PriceQuoteRepository>();
        services.AddTransient<IModifyIdentityUserRepository, ModifyIdentityUserRepository>();
        services.AddTransient<IRentalRepository, RentalRepository>();
        services.AddTransient<IRentalDetailRepository, RentalDetailRepository>();
        services.AddTransient<IRobotRepository, RobotRepository>();
        services.AddTransient<ITypesOfRoboRepository, TypesOfRoboRepository>();
        
        services.AddTransient<IChatMessageRepository, ChatMessageRepository>();
        services.AddTransient<IChatRoomRepository, ChatRoomRepository>();
        services.AddTransient<IRentalContractRepository, RentalContractRepository>();
        services.AddTransient<IDraftClausesRepository, DraftClausesRepository>();
        services.AddTransient<IContractDraftsRepository, ContractDraftsRepository>();
        services.AddTransient<IContractTemplatesRepository, ContractTemplatesRepository>();
        services.AddTransient<ITemplateClausesRepository, TemplateClausesRepository>();
        services.AddTransient<IActivityTypeRepository, ActivityTypeRepository>();
        services.AddTransient<IEventActivityRepository, EventActivityRepository>();
        services.AddTransient<IRobotTypeOfActivityRepository, RobotTypeOfActivityRepository>();
        services.AddTransient<IActualDeliveryRepository, ActualDeliveryRepository>();
        
        services.AddTransient<IActivityTypeGroupRepository, ActivityTypeGroupRepository>();
        services.AddTransient<IGroupScheduleRepository, GroupScheduleRepository>();
        services.AddTransient<IContractReportsRepository, ContractReportsRepository>();
        services.AddTransient<IPaymentRecordRepository, PaymentRecordRepository>();
        return services;
    }
}