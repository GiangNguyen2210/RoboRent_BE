using Microsoft.Extensions.DependencyInjection;
using RoboRent_BE.Repository.Interface;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;

namespace RoboRent_BE.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IEventScheduleRepository, EventScheduleRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IEventRepository, EventRepository>();
        services.AddTransient<IRoboTypeRepository, RoboTypeRepository>();
        services.AddTransient<IPriceQuoteRepository, PriceQuoteRepository>();
        services.AddTransient<IEventRoboTypeRepository, EventRoboTypeRepository>();
        services.AddTransient<IModifyIdentityUserRepository, ModifyIdentityUserRepository>();
        services.AddTransient<IRentalRepository, RentalRepository>();
        services.AddTransient<IRentalDetailRepository, RentalDetailRepository>();
        services.AddTransient<IRobotRepository, RobotRepository>();
        services.AddTransient<ITypesOfRoboRepository, TypesOfRoboRepository>();
        services.AddTransient<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddTransient<IChatMessageRepository, ChatMessageRepository>();
        services.AddTransient<IChatRoomRepository, ChatRoomRepository>();
        services.AddTransient<IRentalContractRepository, RentalContractRepository>();
        return services;
    }
}