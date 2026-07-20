using Microsoft.AspNetCore.Builder;
using WebAPI.Features.Shelves;

namespace WebAPI.Infrastructure.Setup;

public static class EndpointRegistration
{
	public static WebApplication UseEndpoints(this WebApplication app)
	{
		app.UseHealthChecks("/_health");

		var group = app.MapGroup("/api");
		group.MapShelvesEndpoints();
		return app;
	}
}