using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService(ICountriesRepository countriesRepository) : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository = countriesRepository;

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null) return null;

            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            if (country_response_from_list == null) return null;

            return country_response_from_list.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCout = worksheet.Dimension.Rows;

                for (int row = 2; row < rowCout; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        string countryName = cellValue;

                        if (await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}
