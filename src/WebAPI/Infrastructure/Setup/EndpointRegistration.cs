using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebAPI.Features.Authors;
using WebAPI.Features.Books;
using WebAPI.Features.Common;
using WebAPI.Features.Lending;

namespace WebAPI.Infrastructure.Setup;

public static class EndpointRegistration
{
	public static WebApplication UseEndpoints(this WebApplication app)
	{
		app.UseHealthChecks("/_health");

		var group = app.MapGroup("/api")
			.AddEndpointFilter<ProblemLoggingFilter>();

		group.MapAuthorsEndpoints();
		group.MapBooksEndpoints();
		group.MapLendingEndpoints();
		return app;
	}
}