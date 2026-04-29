using StaffFlow.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace StaffFlow.Filters.ActionFilters
{
    public class PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) : IAsyncActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecutionAsync));

            PersonsController personsController = (PersonsController)context.Controller;

            personsController.ViewBag.SearhFields = new Dictionary<string, string>()
            {
                {nameof(PersonResponse.PersonName), "Person Name" },
                {nameof(PersonResponse.Email), "Email" },
                {nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
                {nameof(PersonResponse.Gender), "Gender" },
                {nameof(PersonResponse.CountryID), "Country" },
                {nameof(PersonResponse.Address), "Address" }
            };

            await next();

            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecutionAsync));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOption = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address)
                    };

                    if (searchByOption.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("serchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("serchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
