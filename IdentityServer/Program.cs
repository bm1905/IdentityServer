using IdentityServer;
using IdentityServer.Initializer;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    WebApplication app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    // this seeding is only for the template to bootstrap the DB and users.
    // in production you will likely want a different approach.
    //if (args.Contains("/seed"))
    //{
    //    Log.Information("Seeding database...");
    //    await DbInitializer.EnsureSeedData(app);
    //    Log.Information("Done seeding database. Exiting.");
    //    return;
    //}

    Log.Information("Seeding database...");
    bool isSeedingSuccessful = await DbInitializer.EnsureSeedData(app);
    if (isSeedingSuccessful)
    {
        Log.Information("Done seeding database. Exiting.");
    }
    else
    {
        Log.Error("Seeding database failed. Exiting.");
        throw new Exception("Seeding database failed.");
    }

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}