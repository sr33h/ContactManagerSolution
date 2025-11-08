using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// represents data access logic for person entity
    /// </summary>
    public interface ICountriesRepository
    {
        Task<Country> AddCountry(Country country);
        Task<List<Country>> GetAllCountries();
        Task<Country?> GetCountryByID(Guid? countryId);
        Task<Country?> GetCountryByName(string countryName);
    }
}
