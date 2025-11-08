using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;

        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
        {
            if(request == null) throw new ArgumentNullException(nameof(request));
            if(request.CountryName == null) throw new ArgumentException(nameof(request.CountryName));
            if( await _countriesRepository.GetCountryByName(request.CountryName) != null)
            {
                throw new ArgumentException($"Duplicate {nameof(request.CountryName)}");
            }

           Country country = request.ToCountry();
            country.CountryID = Guid.NewGuid();
            await _countriesRepository.AddCountry(country);           
            return country.ToCountryResponse();
           
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
          return (await _countriesRepository.GetAllCountries()).Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByID(Guid? countryId)
        {
            if(countryId == null)
            {
                return null;
            }
            Country? countryFoundByID = 
                await _countriesRepository.GetCountryByID(countryId);

            if(countryFoundByID == null)
            {
                return null;
            }
            else
            {
                return countryFoundByID.ToCountryResponse();
            }           
        }

        public async Task<int> UploadFromExcelFile(IFormFile formFile)
        {
           MemoryStream stream = new MemoryStream();
           await formFile.CopyToAsync(stream);
           int countriesInserted = 0;

            using (ExcelPackage  package = new ExcelPackage(stream))
            {
              ExcelWorksheet excelWorksheet =  package.Workbook.Worksheets["Countries"];

                int rowCount = excelWorksheet.Dimension.Rows;
               
                for(int i = 2;i<=rowCount;i++)
                {
                   string? cellValue = Convert.ToString(excelWorksheet.Cells[i, 1].Value);

                    if(!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;

                       if(_countriesRepository.GetCountryByName(countryName) == null) 
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,

                            };

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
