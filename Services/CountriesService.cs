using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }
        public CountryResponse AddCountry(CountryAddRequest? request)
        {
            if(request == null) throw new ArgumentNullException(nameof(request));
            if(request.CountryName == null) throw new ArgumentException(nameof(request.CountryName));
            if( _countries.Any(c => c.CountryName == request.CountryName))
            {
                throw new ArgumentException($"Duplicate {nameof(request.CountryName)}");
            }

           Country country = request.ToCountry();
            country.CountryID = Guid.NewGuid();
            _countries.Add(country);

            return country.ToCountryResponse();
           
        }

        public List<CountryResponse> GetAllCountries()
        {
          return _countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByID(Guid? countryId)
        {
            if(countryId == null)
            {
                return null;
            }
            Country? countryFoundByID = _countries.FirstOrDefault(c => c.CountryID == countryId);

            if(countryFoundByID == null)
            {
                return null;
            }
            else
            {
                return countryFoundByID.ToCountryResponse();
            }           
        }
    }
}
