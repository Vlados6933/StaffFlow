using AutoFixture;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using StaffFlow.Controllers;

namespace StaffFlow
{
    public class PersonsControllerTest
    {
        private readonly Fixture _fixture;

        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        private readonly IConverter _pdfConverter;

        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly Mock<IConverter> _pdfConverterMock;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();
            _pdfConverterMock = new Mock<IConverter>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;
            _logger = _loggerMock.Object;
            _pdfConverter = _pdfConverterMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonList()
        {
            List<PersonResponse> person_responses_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger, _pdfConverter);

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(person_responses_list);

            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(person_responses_list);

            IActionResult actionResult = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());


            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.IsAssignableFrom<IEnumerable<PersonResponse>>(viewResult.ViewData.Model);
            Assert.Equal(person_responses_list, viewResult.ViewData.Model);
        }
        #endregion

        #region Create

        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger, _pdfConverter);

            IActionResult actionResult = await personsController.Create(person_add_request);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.True(redirectResult.ActionName == "Index");
        }
        #endregion

    }
}