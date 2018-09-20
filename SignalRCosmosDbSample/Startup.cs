using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRCosmosDbSample.Startup))]
namespace SignalRCosmosDbSample
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();

			// initalize 
		}
	}
}
