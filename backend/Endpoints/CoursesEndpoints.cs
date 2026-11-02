using ElearningApi.Authz;

namespace ElearningApi.Endpoints;

public static class CoursesEndpoints
{
    public static void MapCourses(this IEndpointRouteBuilder app)
    {
        // GET /courses/{id}: la migrazione passa da un controllo sul ruolo al check relazionale.
        // Prima (RBAC): [Authorize(Roles = "student,author")]. Dopo: l'IAuthorizer.
        app.MapGet("/courses/{id}", async (string id, HttpContext ctx, IAuthorizer authz) =>
        {
            var allowed = await authz.AllowedAsync(ctx.User, "viewer", $"course:{id}");
            return allowed
                ? Results.Ok(new { course = id, access = "granted" })
                : Results.StatusCode(StatusCodes.Status403Forbidden);
        }).RequireAuthorization();

        // PUT /courses/{id}: modificare un corso richiede la relazione editor.
        app.MapPut("/courses/{id}", async (string id, HttpContext ctx, IAuthorizer authz) =>
        {
            var allowed = await authz.AllowedAsync(ctx.User, "editor", $"course:{id}");
            return allowed
                ? Results.Ok(new { course = id, updated = true })
                : Results.StatusCode(StatusCodes.Status403Forbidden);
        }).RequireAuthorization();
    }
}
