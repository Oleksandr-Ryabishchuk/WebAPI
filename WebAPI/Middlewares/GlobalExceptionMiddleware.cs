using WebAPI.Data;
using WebAPI.Entities;
using WebAPI.Exceptions;

namespace WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            try
            {
                await next(httpContext);
            }
            catch (SecureException ex)
            {
                context.Journals.Add(new Journal()
                {
                    Type = "Secure",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
            }
            catch (ControllerException ex)
            {
                context.Journals.Add(new Journal()
                {
                    Type = "Exception",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.CustomData.Message,
                    EventId = ex.CustomData.EventId
                });
            }
            catch (Exception ex)
            {
                context.Journals.Add(new Journal()
                {                    
                    Type = "Exception",
                    CreatedAt = DateTime.UtcNow,
                    Message = ex.Message                    
                });
            }
        }
    }
}
