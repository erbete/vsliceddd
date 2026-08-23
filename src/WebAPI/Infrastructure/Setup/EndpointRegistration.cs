using Microsoft.AspNetCore.Builder;
using WebAPI.Features.Authors;
using WebAPI.Features.Books;

namespace WebAPI.Infrastructure.Setup;

public static class EndpointRegistration
{
	public static WebApplication UseEndpoints(this WebApplication app)
	{
		app.UseHealthChecks("/_health");

		var group = app.MapGroup("/api");
		group.MapAuthorsEndpoints();
		group.MapBooksEndpoints();
		return app;
	}
}