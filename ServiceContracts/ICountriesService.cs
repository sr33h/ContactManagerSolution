using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="request">Country object to add but in the form of AddDTO</param>
        /// <returns>Returns the Country object as ResponseDTO</returns>
       Task<CountryResponse> AddCountry(CountryAddRequest? request);
       Task<List<CountryResponse>> GetAllCountries();

       Task<CountryResponse?> GetCountryByID(Guid? countryId);

       Task<int> UploadFromExcelFile(IFormFile formFile);
    }
}
