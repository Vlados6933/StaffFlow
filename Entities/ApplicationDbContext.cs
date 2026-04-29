using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson) ?? new List<Country>();

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);

            //Seed to Persons
            string personsJson = File.ReadAllText("persons.json");
            List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson) ?? new List<Person>();

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);
        }
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@Position", person.Position),
                new SqlParameter("@Salary", person.Salary)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @Position, @Salary", parameters);
        }
    }
}
