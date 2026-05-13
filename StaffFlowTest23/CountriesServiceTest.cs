using AutoFixture;
using Entities;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Diagnostics.Metrics;
using Xunit;

namespace StaffFlowTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            CountryAddRequest? request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, null as string).Create();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            CountryAddRequest? first_country_request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();
            CountryAddRequest? second_country_request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();

            Country first_country = first_country_request.ToCountry();
            Country second_country = second_country_request.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesService.AddCountry(first_country_request);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(second_country_request);
            });
        }

        [Fact]
        public async Task AddCountry_FullCountry_ToBeSuccessful()
        {
            CountryAddRequest? country_request = _fixture.Create<CountryAddRequest>();

            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

            CountryResponse country_from_add_country = await _countriesService.AddCountry(country_request);

            country_response.CountryID = country_from_add_country.CountryID;

            Assert.True(country_from_add_country.CountryID != Guid.Empty);
            Assert.Equal(country_response, country_from_add_country);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_ToBeEmptyList()
        {
            List<Country> country_empty_list = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_ShouldHaveFewCountries()
        {
            List<Country> country_list = new List<Country>
            {
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create(),

                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create(),
            };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            Assert.Equal(country_response_list, actualCountryResponseList);
        }
        #endregion

        #region GetCountryByCountryID

        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            Guid? countryID = null;

            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            Country? country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();

            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);

            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country.CountryID);

            Assert.Equal(country_response, country_response_from_get);
        }

        #endregion
    }
}
