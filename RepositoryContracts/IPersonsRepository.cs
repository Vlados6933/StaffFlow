using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        Task<Person> AddPerson(Person person);

        Task<List<Person>> GetAllPersons();

        Task<Person?> GetPersonByPersonID(Guid personID);

        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        Task<bool> DeletePersonByPersonID(Guid personID);

        Task<Person> UpdatePerson(Person person);
    }
}
