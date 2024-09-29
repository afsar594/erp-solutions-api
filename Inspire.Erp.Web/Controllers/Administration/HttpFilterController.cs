using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class SpecialCharacterValidationFilter : IAsyncActionFilter
{
    private readonly string _pattern = @"[';#""=\\]|--"; // Updated to allow single hyphen but disallow double hyphen

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get all parameters (bound models) from the request
        foreach (var parameter in context.ActionArguments)
        {
            var value = parameter.Value;
            if (value != null)
            {
                // Check if the action has the SkipSpecialCharacterValidation attribute
                var skipValidation = context.ActionDescriptor.EndpointMetadata
                    .OfType<SkipSpecialCharacterValidationAttribute>().Any();

                // If skipValidation is true, bypass the special character check
                if (skipValidation)
                {
                    await next(); // Call the next action in the pipeline
                    return;
                }

                if (!CheckForSpecialCharacters(value))
                {
                    var objResponse = new ResponseInfo
                    {
                        IsSuccess = false,
                        Message = "Request contains special characters (" + @"';#""=\--)"
                    };
                    // If special characters are found, return an error response
                    context.Result = new ObjectResult(objResponse)
                    {
                        StatusCode = (int)HttpStatusCode.OK // Change as needed
                    };
                    return;
                }
            }
        }

        await next(); // Proceed to the next action in the pipeline
    }

    // Method to check for special characters using reflection
    private bool CheckForSpecialCharacters(object model)
    {
        var properties = model.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string))
            {
                string value = property.GetValue(model) as string;
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, _pattern))
                {
                    return false; // Return false if special characters are found
                }
            }
        }

        return true; // Return true if no special characters are found
    }
}

public class SkipSpecialCharacterValidationAttribute 
{
    // Empty class, only used as a marker for skipping validation
}
