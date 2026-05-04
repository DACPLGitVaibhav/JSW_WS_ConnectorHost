using Serilog;
using WS_ConnectorHost;
using WS_Haimdall.Model_Class;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Fatal)
   .WriteTo.Console()  
   .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "WS_ConnectorHost";
});
builder.Services.Configure<appSettings>(
    builder.Configuration.GetSection("appSettings"));
var host = builder.Build();

host.Run();
