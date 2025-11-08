using System;
using System.Collections.Generic;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;

namespace ContactManagerAppTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            var countries = new List<Country> { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                 new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(x => x.Countries, countries);

            _countriesService = new CountriesService(null);
        }

        #region AddCountry

        //when CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest? request = null;
             
            await Assert.ThrowsAsync<ArgumentNullException>( async () =>
            {
              await  _countriesService.AddCountry(request);
            });
        }

        //when CountryName is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            await Assert.ThrowsAsync<ArgumentException>( async () =>
            {
              await  _countriesService.AddCountry(request);
            });
        }

        //when CountryName is duplicate, ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA"};
            CountryAddRequest? request2= new CountryAddRequest() { CountryName = "USA" };

            await _countriesService.AddCountry(request1);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                
              await  _countriesService.AddCountry(request2);
            });
        }

        //when valid CountryName, insert to the list
        [Fact]
        public async Task AddCountry_ValidCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
           
            CountryResponse response = await _countriesService.AddCountry(request1);

            Assert.True(response.CountryID != Guid.Empty);
           
        }

        #endregion


        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            List<CountryResponse> countryResponseList = await  _countriesService.GetAllCountries();

            Assert.Empty(countryResponseList);
        }

        [Fact]
        public async Task GetCountries_AddFewCountries()
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
                countryResponsesFromAddCountry.Add( await _countriesService.AddCountry(item));
            }

            List<CountryResponse> countryResponsesFromGetAllCountries  = await _countriesService.GetAllCountries();

            foreach(CountryResponse expectedCountry in countryResponsesFromAddCountry)
            {
                Assert.Contains(expectedCountry, countryResponsesFromGetAllCountries);
            }

        }

        #endregion


        #region GetCountryByCountryID

        [Fact]
        public async Task GetCountryByCountryID_ReturnsNullForNonExistingID()
        {
            Guid? countryID = Guid.NewGuid();

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            if(countryResponse.CountryID != countryID)
            {
                CountryResponse? responseFromGetByID = await _countriesService.GetCountryByID(countryID);

                Assert.Null(responseFromGetByID);
            }
           
        }

        [Fact]
        public async Task GetCountryByCountryID_ReturnsNullIfListEmpty()
        {
            Guid? countryID = Guid.NewGuid();

            CountryResponse? responseFromGetByID = await _countriesService.GetCountryByID(countryID);

            Assert.Null(responseFromGetByID);
        }

        [Fact]
        public async Task GetCountryByCountryID_ReturnsCountryResponseIfIDExist()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };

            CountryResponse countryResponseFromAdd = await _countriesService.AddCountry(countryAddRequest);

            CountryResponse? countryResponseFromByID = await _countriesService.GetCountryByID(countryResponseFromAdd.CountryID);

            Assert.Equal(countryResponseFromByID,countryResponseFromAdd);

        }

        [Fact]
        public async Task GetCountryByCountryID_ReturnsNullIfCountryIDPassedIsNull()
        {
            Guid? countryID = null;
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };
            await _countriesService.AddCountry(countryAddRequest);
            CountryResponse? responseFromGetByID = await _countriesService.GetCountryByID(countryID);

            Assert.Null(responseFromGetByID);
        }

        #endregion

    }
}
