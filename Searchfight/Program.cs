using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Searchfight.Models;
using Searchfight.Services;
using System.IO;
using System.Threading.Tasks;

namespace Searchfight
{
	internal class Program
	{
		private static async Task Main()
		{
			var services = ConfigureServices();

			var serviceProvider = services.BuildServiceProvider();

			// calls the Run method in App, which is replacing Main
			await serviceProvider.GetService<App>().Run();
		}

		private static IServiceCollection ConfigureServices()
		{
			IServiceCollection services = new ServiceCollection();

			var config = LoadConfiguration();
			services.AddSingleton(config);

			services.AddTransient<SearchConfigurationModel>();
			services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();

			// required to run the application
			services.AddTransient<App>();

			return services;
		}

		public static IConfiguration LoadConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true);

			return builder.Build();
		}
	}
}
