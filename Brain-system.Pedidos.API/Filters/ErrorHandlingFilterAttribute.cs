using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BrainSystem.Auth.API.Filters
{
    public class ErrorHandlingFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            context.Result = new ObjectResult(new { Error = "error: " + exception.Message })
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
