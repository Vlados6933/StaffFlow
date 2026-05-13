using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog;
using Microsoft.Extensions.Logging;

namespace StaffFlow
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        private readonly Mock<IDiagnosticContext> _diagnosticContextMock;
        private readonly Mock<ILogger<PersonsService>> _loggerMock;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

             _diagnosticContextMock = new Mock<IDiagnosticContext>();
             _loggerMock = new Mock<ILogger<PersonsService>>();

            _personsService = new PersonsService(_personsRepository, _loggerMock.Object, _diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_PresonNameIsNull_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 await _personsService.AddPerson(personAddRequest);
             });
        }

        [Fact]
        public async Task AddPerson_FullPeroperDetails_ToBeSuccessful()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            person_response_expected.PersonID = person_response_from_add.PersonID;

            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Equal(person_response_expected, person_response_from_add);
        }

        #endregion

        #region GetPeronByPersonID

        [Fact]
        public async Task GetPeronByPersonID_NullPersonID_ToBeNull()
        {
            Guid? personID = null;

            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

            Assert.Null(person_response_from_get);
        }

        [Fact]
        public async Task GetPeronByPersonID_WithPersonID_ToBeSucessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "person@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person.PersonID);

            Assert.Equal(person_response_expected, person_response_from_get);
        }
        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            var persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> person_from_get = await _personsService.GetAllPersons();

            Assert.Empty(person_from_get);
        }

        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "email1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "email2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "email3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (var person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> person_list_from_get = await _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual:");
            foreach (var person_response_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            Assert.Equal(person_response_list_expected, person_list_from_get);
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "email1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "email2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "email3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (var person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse> person_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual:");
            foreach (var person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            Assert.Equal(person_response_list_expected, person_list_from_search);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "email1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "email2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "email3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (var person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse> person_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            _testOutputHelper.WriteLine("Actual:");
            foreach (var person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            Assert.Equal(person_response_list_expected, person_list_from_search);
        }
        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_TobeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "email1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "email2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "email3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            _testOutputHelper.WriteLine("Expected:");
            foreach (var person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            List<PersonResponse> person_list_from_sort = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual:");
            foreach (var person_response_from_get in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            Assert.Equal(person_response_list_expected.OrderByDescending(temp => temp.PersonName), person_list_from_sort);
        }
        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? person_update_request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            PersonUpdateRequest? person_update_request = _fixture.Create<PersonUpdateRequest>();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "email@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetails_TobeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest? person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

            Assert.Equal(person_response_expected, person_response_from_update);
        }
        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidPersonID_TobeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            bool isDeleted = await _personsService.DeletePerson(person.PersonID);

            Assert.True(isDeleted);
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonID_TobeSuccessful()
        {
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);
        }
        #endregion
    }
}
