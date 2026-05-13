using Entities;
using RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CountriesRepository(ApplicationDbContext db) : ICountriesRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<Country> AddCountry(Country country)
        {
            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryID(Guid countryID)
        {
            return await _db.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(temp => temp.CountryID == countryID);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _db.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
