namespace PIED_LMS.API;

public static class Extensions
{
    public static void MapCustomEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

        if (app.Environment.IsDevelopment())
            app.MapGet("/", () => Results.Redirect("/docs"));
        else
            app.MapGet("/", () => Results.Ok(new { message = "PIED-LMS API is running." }));
    }
}
