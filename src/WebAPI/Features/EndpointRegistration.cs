using Microsoft.AspNetCore.Builder;
using WebAPI.Features.Shelves;

namespace WebAPI.Features;

internal static class EndpointRegistration
{
	public static WebApplication UseEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api");
		group.MapShelvesEndpoints();
		return app;
	}
}