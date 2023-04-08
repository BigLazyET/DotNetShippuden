namespace Learn.Demo.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var hostBuilder = Host.CreateDefaultBuilder(args)
            //                      .ConfigureEnvironment()
            //                      .UseApm();

            Environment.SetEnvironmentVariable("APPNAME", "opentelemetry.collector.test");
            Environment.SetEnvironmentVariable("ENVIRONMENTTYPE", "qa");
            Environment.SetEnvironmentVariable("ENVIRONMENT", "qa");

            var builder = WebApplication.CreateBuilder(args);
            // builder.Host.ConfigureEnvironment().UseApm();

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}