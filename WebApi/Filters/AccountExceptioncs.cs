using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters
{
    public class AccountExceptioncs : IAsyncExceptionFilter
    {
        readonly IHostEnvironment env;
        readonly ILogger<AccountExceptioncs> logger;
        public AccountExceptioncs(IHostEnvironment hostEnvironment, ILogger<AccountExceptioncs> logger)
        {
            env = hostEnvironment;
            this.logger = logger;
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            string message = "unhandled account exception";
            logger.LogError(context.Exception, message);
            if (env.IsDevelopment())
            {
                message = context.Exception.ToString();
            }
            ObjectResult result = new ObjectResult(new { code = 500, message = message });
            result.StatusCode = 500;
            context.Result = result;
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
