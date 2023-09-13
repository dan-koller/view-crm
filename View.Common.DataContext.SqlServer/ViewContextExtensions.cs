using Microsoft.EntityFrameworkCore; // UseSqlServer
using Microsoft.Extensions.DependencyInjection; // IServiceCollection

namespace View.Shared
{
    public static class ViewContextExtensions
    {
        /// <summary>
        /// Adds ViewContext to the specified IServiceCollection. Uses the SqlServer database provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">Set to override the default.</param>
        /// <returns>An IServiceCollection that can be used to add more services.</returns>
        public static IServiceCollection AddViewContext(
          this IServiceCollection services,
          string connectionString = "Data Source=.;Initial Catalog=View;" +
            "Integrated Security=true;MultipleActiveResultsets=true;Encrypt=false")
        {
            services.AddDbContext<ViewContext>(options =>
            {
                options.UseSqlServer(connectionString);

                options.LogTo(WriteLine, // Console
                  new[] { Microsoft.EntityFrameworkCore
          .Diagnostics.RelationalEventId.CommandExecuting });
            },
            // Register with a transient lifetime to avoid concurrency issues with Blazor Server projects.
            contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Transient);

            return services;
        }
    }
}
