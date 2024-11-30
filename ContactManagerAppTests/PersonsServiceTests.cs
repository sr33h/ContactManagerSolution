using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Net;
using System.Reflection;
using Xunit;

namespace ContactManagerAppTests
{
    public class PersonsServiceTests
    {
       private readonly IPersonsService _personsService;

        public PersonsServiceTests()
        {
            _personsService = new PersonService();
        }

        #region AddPerson

        [Fact]
        public void AddPerson_ThrowArgumentNullExceptionIfPersonIsNull()
        {
            PersonAddRequest? personAddRequest = null;

            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));
        }

        [Fact]
        public void AddPerson_ThrowArgumentExceptionIfPersonNameIsNull()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName=null,
            };

            Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
        }

        [Fact]
        public void AddPerson_InsertsPersonIfPersonAddRequestIsValid()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "Email",
                DateOfBirth = new DateTime(1999,12,31),
                Gender = ServiceContracts.Enums.GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                Address = "Address",
                ReceiveNewsLetters = true
            };

            PersonResponse personResponse =  _personsService.AddPerson(personAddRequest);

            Assert.True(personResponse.PersonID != Guid.Empty);

        }





        #endregion


    }
}
