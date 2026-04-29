using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using StaffFlow.Controllers;

namespace StaffFlow.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter(ILogger<PersonCreateAndEditPostActionFilter> logger,ICountriesService countriesService) : IAsyncActionFilter
    {
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger = logger;
        private readonly ICountriesService _countriesService = countriesService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments.First(); 

                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }

            _logger.LogInformation("In after logic of {FilterName}", nameof(PersonCreateAndEditPostActionFilter));
        }
    }
}
