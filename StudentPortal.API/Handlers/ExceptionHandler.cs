using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.API.Handlers
{
    public static class ExceptionHandler
    {
        public static void ConfigureException(this IApplicationBuilder app, IWebHostEnvironment hostEnvironment)
        {

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    IExceptionHandlerFeature? exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandleFeature != null)
                    {
                        var status = (int) ResponseStatus.FATAL_ERROR;
                        switch (exceptionHandleFeature.Error)
                        {
                            //More Exceptions can be added as they are identified, those that aren't identified will default to the 500 status code 
                            case InvalidDataException:
                            case InvalidOperationException:
                            case KeyNotFoundException:
                            case ArgumentException:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                status = (int)ResponseStatus.APP_ERROR;
                                break;

                            default:
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                break;
                        }

                        ErrorResponse err = new() { Success = false, Status = status };
                        err.Message =
                            hostEnvironment.IsProduction() && context.Response.StatusCode ==
                            StatusCodes.Status500InternalServerError
                                ? "We currently cannot complete this request process. Please retry or contact our support"
                                : exceptionHandleFeature.Error.Message;

                        JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        string msg = JsonConvert.SerializeObject(err, serializerSettings);
                        await context.Response.WriteAsync(msg);
                    }
                });
            });
        }
    }
    
}
