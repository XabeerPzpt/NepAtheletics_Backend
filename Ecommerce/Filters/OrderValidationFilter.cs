using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Ecommerce.Filters
{
    public class OrderValidationFilter : IAsyncActionFilter
    {

        //ActionExecutingContext  =  Provides access to details about the current request, including the ModelState (it's validation also)
        //ActionExecutionDelegate = represents the next step in the pipeline
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                // Extract the first validation error message from the model state
                var errorMessage = context.ModelState.Values       //Gets all validation error collections.
                    .SelectMany(v => v.Errors)              // Access each error in model state values
                    .Select(e => e.ErrorMessage)                // Extracts just the error message from each error.
                    .FirstOrDefault();                                   //Retrieves the first error message found.

                var statusCode = context.HttpContext.Response.StatusCode;
                
                context.Result = new BadRequestObjectResult(new
                {
                    Code = statusCode,  // Custom code representing a bad request error
                    Message = errorMessage // Error message describing the validation issue
                });


                return; // Skip further action execution since validation failed
            }

            await next();   // if validation succeed : pass control to the next action or middleware
        }
    }
}
