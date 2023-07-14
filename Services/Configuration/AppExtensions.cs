using Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace MOCA.Services.Configuration
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app, IConfiguration configuration, string apiTitle)
        {
            var swaggerSettings = configuration.GetSection("SwaggerSettings").Get<SwaggerSettings>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", apiTitle);
                c.RoutePrefix = $"{swaggerSettings.Key}/swagger";
            });
        }

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void UseStaticFilesExtension(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),

                RequestPath = new PathString("/Resources")

            });
        }
    }
}
