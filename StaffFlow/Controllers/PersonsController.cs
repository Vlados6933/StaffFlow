using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using StaffFlow.Extensions.ControllerExtensions;
using StaffFlow.Filters;
using StaffFlow.Filters.ActionFilters;
using StaffFlow.Filters.AuthorizationFilters;
using StaffFlow.Filters.ExceptionFilters;
using StaffFlow.Filters.ResourceFilters;
using StaffFlow.Filters.ResultFilters;

namespace StaffFlow.Controllers
{
    [Route("[controller]")]
    [ResponseHeaderFilterFactory("MyKey-From-Controller", "MyValue-From-Controller", 3)]
    [TypeFilter<HandleExceptionFilter>]
    [TypeFilter<PersonsAlwaysRunResultFilter>]
    public class PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger, IConverter pdfConverter) : Controller
    {
        private readonly IPersonsService _personsService = personsService;
        private readonly ICountriesService _countriesService = countriesService;
        private readonly ILogger<PersonsController> _logger = logger;
        private readonly IConverter _pdfConverter = pdfConverter;


        [Route("[action]")]
        [Route("/")]
        [TypeFilter<PersonsListActionFilter>(Order = 4)]
        [ResponseHeaderFilterFactory("MyKey-From-Action", "MyValue-From-Action", 1)]
        [TypeFilter<PersonsListResultFilter>]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonController");

            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter<PersonCreateAndEditPostActionFilter>]
        [TypeFilter<FeatureDisableResourseFilter>(Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [TypeFilter<TokenResultFilter>]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter<PersonCreateAndEditPostActionFilter>]
        [TypeFilter<TokenAuthorizationFilter>]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);

            return RedirectToAction("Index");

        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            string htmlContent = await this.RenderViewToStringAsync("PersonsPDF", persons);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 20, Right = 20, Bottom = 20, Left = 20 }
            },
                    Objects = {
                    new ObjectSettings() {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8", Background = true },
                    
                }
            }
            };

            byte[] pdfBytes = pdfConverter.Convert(doc);

            return File(pdfBytes, "application/pdf", "persons.pdf");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
