using System;
using System.Collections.Generic;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;

namespace ContactManagerAppTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry

        //when CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            CountryAddRequest? request = null;
             
            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        //when CountryName is null, throw ArgumentNullException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        //when CountryName is duplicate, ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA"};
            CountryAddRequest? request2= new CountryAddRequest() { CountryName = "USA" };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        //when valid CountryName, insert to the list
        [Fact]
        public void AddCountry_ValidCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
           
            CountryResponse response = _countriesService.AddCountry(request1);

            Assert.True(response.CountryID != Guid.Empty);
           
        }

        #endregion


        #region GetAllCountries

        [Fact]
        public void GetAllCountries_EmptyList()
        {
            List<CountryResponse> countryResponseList =  _countriesService.GetAllCountries();

            Assert.Empty(countryResponseList);
        }

        [Fact]
        public void GetCountries_AddFewCountries()
        {
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName = "USA" },
                new CountryAddRequest() { CountryName = "Russia" },
                new CountryAddRequest() { CountryName = "Canada" }
            };

            List<CountryResponse> countryResponsesFromAddCountry = new List<CountryResponse>();

            foreach (var item in countryAddRequests)
            {
                countryResponsesFromAddCountry.Add(_countriesService.AddCountry(item));
            }

            List<CountryResponse> countryResponsesFromGetAllCountries  = _countriesService.GetAllCountries();

            foreach(CountryResponse expectedCountry in countryResponsesFromAddCountry)
            {
                Assert.Contains(expectedCountry, countryResponsesFromGetAllCountries);
            }

        }

        #endregion


        #region GetCountryByCountryID

        [Fact]
        public void GetCountryByCountryID_ReturnsNullForNonExistingID()
        {
            Guid? countryID = Guid.NewGuid();

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            if(countryResponse.CountryID != countryID)
            {
                CountryResponse? responseFromGetByID = _countriesService.GetCountryByID(countryID);

                Assert.Null(responseFromGetByID);
            }
           
        }

        [Fact]
        public void GetCountryByCountryID_ReturnsNullIfListEmpty()
        {
            Guid? countryID = Guid.NewGuid();

            CountryResponse? responseFromGetByID = _countriesService.GetCountryByID(countryID);

            Assert.Null(responseFromGetByID);
        }

        [Fact]
        public void GetCountryByCountryID_ReturnsCountryResponseIfIDExist()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);

            CountryResponse? countryResponseFromByID = _countriesService.GetCountryByID(countryResponseFromAdd.CountryID);

            Assert.Equal(countryResponseFromByID,countryResponseFromAdd);

        }

        [Fact]
        public void GetCountryByCountryID_ReturnsNullIfCountryIDPassedIsNull()
        {
            Guid? countryID = null;
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };
            _countriesService.AddCountry(countryAddRequest);
            CountryResponse? responseFromGetByID = _countriesService.GetCountryByID(countryID);

            Assert.Null(responseFromGetByID);
        }

        #endregion

    }
}
