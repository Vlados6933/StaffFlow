using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger) : IPersonsRepository
    {
        private readonly ApplicationDbContext _db = db;
        private readonly ILogger<PersonsRepository> _logger = logger;

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            int rowsDeleted = await _db.Persons.Where(temp => temp.PersonID == personID).ExecuteDeleteAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsRepository");

            return await _db.Persons
                .AsNoTrackingWithIdentityResolution()
                .Include("Country")
                .ToListAsync();

        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _db.Persons
                .AsNoTrackingWithIdentityResolution()
                .Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _db.Persons
                .AsNoTrackingWithIdentityResolution()
                .Include("Country")
                .FirstOrDefaultAsync(tenp => tenp.PersonID == personID);
        }

        public async Task<bool> UpdatePerson(Person person)
        {
            int rowsDeleted = await _db.Persons
                .Where(temp => temp.PersonID == person.PersonID)
                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.PersonName, person.PersonName)
                .SetProperty(p => p.Email, person.Email)
                .SetProperty(p => p.DateOfBirth, person.DateOfBirth)
                .SetProperty(p => p.Gender, person.Gender)
                .SetProperty(p => p.CountryID, person.CountryID)
                .SetProperty(p => p.Address, person.Address)
                .SetProperty(p => p.Position, person.Position)
                .SetProperty(p => p.Salary, person.Salary)
                );

            return rowsDeleted > 0;
        }
    }
}
