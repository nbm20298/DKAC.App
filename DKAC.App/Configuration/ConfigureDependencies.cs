using DKAC.App.Helpers;
using DKAC.App.Interfaces;
using DKAC.Services.Implementations;
using DKAC.Services.Interfaces;

namespace DKAC.App.Configuration
{
    public static class ConfigureDependencies
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IRegisterService, RegisterService>();
            services.AddTransient<IUserService, UserService>();

            services.AddHttpContextAccessor();


            return services;
        }
    }
}
