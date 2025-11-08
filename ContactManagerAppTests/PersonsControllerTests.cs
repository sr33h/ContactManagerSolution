using AutoFixture;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using ContactManagerApp.Controllers;
using ServiceContracts.DTO;
using Microsoft.Data.SqlClient;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagerAppTests
{
    public class PersonsControllerTests
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTests()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //arrange
            List<PersonResponse?> person_response_list_filtered = _fixture.Create<List<PersonResponse?>>();
            List<PersonResponse> person_response_list_sorted = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(
                _personsService, _countriesService);

            _personsServiceMock.Setup(x => x.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(person_response_list_filtered);

            _personsServiceMock.Setup(x => x.GetSortedPersons(It.IsAny<List<PersonResponse?>>(), It.IsAny<string>(),
                It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(person_response_list_sorted);

            //act
            IActionResult result = await personsController.Index(
                _fixture.Create<string>(), 
                _fixture.Create<string>(), 
                _fixture.Create<string>(), 
                _fixture.Create<SortOrderOptions>());

            //assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(person_response_list_sorted);

        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_IfModelErrors_ReturnCreateView()
        {

            //arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(x => x.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsServiceMock.Setup(x => x.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(
                _personsService, _countriesService);

            //act

            personsController.ModelState.AddModelError("PersonName","Person Name cannot be null");

            IActionResult result = await personsController.Create(person_add_request);

            //assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(person_add_request);
        }

        [Fact]
        public async Task Create_IfNoModelErrors_RedirectToIndex()
        {

            //arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(x => x.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsServiceMock.Setup(x => x.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(
                _personsService, _countriesService);

            //act

            IActionResult result = await personsController.Create(person_add_request);

            //assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
