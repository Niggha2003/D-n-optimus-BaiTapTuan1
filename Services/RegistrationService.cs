using BaiTap1.Handlers;

namespace BaiTap1.Services
{
    public static class RegistrationService
    {
        public static void AddMyHandlers(this IServiceCollection services)
        {
            // add handlers for controllers
            services.AddScoped<BookHandler>();
            services.AddScoped<AuthorHandler>();
        }

    }
}
