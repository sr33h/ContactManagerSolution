using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Net;
using System.Reflection;
using Xunit;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;

namespace ContactManagerAppTests
{
    public class PersonsServiceTests
    {
       private readonly IPersonsService _personsService;
       private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        public PersonsServiceTests()
        {
            _fixture = new Fixture();

            var countries = new List<Country> { };
            var persons = new List<Person> { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                 new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(x => x.Countries, countries);
            dbContextMock.CreateDbSetMock(x => x.Persons, persons);

            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            _countriesService = new CountriesService(null);          

            _personsService = new PersonService(_personsRepository);
           
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_ThrowArgumentNullExceptionIfPersonIsNull()
        {
            PersonAddRequest? personAddRequest = null;

         Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);

         await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_ThrowArgumentExceptionIfPersonNameIsNull()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(x => x.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);

            await action.Should().ThrowAsync<ArgumentException>();

          // await  Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_InsertsPersonIfPersonAddRequestIsValid()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(x => x.Email, "abcd@efg.com").Create();

            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse personResponse = await  _personsService.AddPerson(personAddRequest);

            Assert.True(personResponse.PersonID != Guid.Empty);

        }





        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_ReturnsListOfAllPersons()
        {
            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = Guid.NewGuid(),
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = Guid.NewGuid(),
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = Guid.NewGuid(),
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse?> personResponses = await _personsService.GetAllPersons();           

            
            foreach (PersonResponse? personResponse in personResponses)
            {
               Assert.Contains(personResponse, personsAdded);
            }
           
        }

        [Fact]
        public async Task GetAllPersons_ReturnsEmptyListIfListIsEmpty()
        {
            _personsRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(new List<Person>());
            List<PersonResponse?> personResponses = await _personsService.GetAllPersons();

            Assert.Empty(personResponses);
        }

        #endregion

        #region GetPersonByID

        [Fact]
        public async Task GetPersonByID_ThrowsArgumentNullExceptionIfIDNull()
        {
            Guid? personId = null;

          await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.GetPersonByID(personId));
        }

        [Fact]
        public async Task GetPersonByID_ReturnsNullIfIDNotExist()
        {
            Guid? personId = Guid.NewGuid();

            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "person@email.com",
                DateOfBirth = new DateTime(1999, 12, 31),
                Gender = ServiceContracts.Enums.GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                Address = "Address",
                ReceiveNewsLetters = true
            };

            await _personsService.AddPerson(personAddRequest);

            PersonResponse? personResponse = await _personsService.GetPersonByID(personId);

            Assert.Null(personResponse);            
        }

        [Fact]
        public async Task GetPersonByID_ReturnsNullIfListEmpty()
        {
            Guid? personId = Guid.NewGuid();
            PersonResponse? personResponse = await _personsService.GetPersonByID(personId);
            Assert.Null(personResponse);

        }

        [Fact]
        public async Task GetPersonByID_ReturnsValidPersonResponseIfIDExist()
        {
            Person personToAdd = _fixture.Build<Person>().With(x => x.Email, "someemail@abc.com")
                .With(x => x.Country, null as Country)
                .Create();
            
            PersonResponse? personAdded = personToAdd.ToPersonResponse();

            _personsRepositoryMock.Setup(x => x.GetPersonByID(It.IsAny<Guid?>())).ReturnsAsync(personToAdd);

            PersonResponse? person_response_from_get = await  _personsService.GetPersonByID(personToAdd.PersonID);

            Assert.Equal(person_response_from_get, personAdded);
           
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_ReturnsEmptyListIfSearchDoesNotMatchWithAnyPerson()
        {
            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse?> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "xyzTTT");

           Assert.Empty(filteredPersons);
        }

        [Fact]
        public async Task GetFilteredPersons_ReturnsListOfMatchingPersonsIfSearchByIsValid()
        {

            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add( await _personsService.AddPerson(person));
            }

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "john");

            foreach (PersonResponse personAdded in personsAdded)
            {
                if(personAdded.PersonName !=  null)
                {
                    if (personAdded.PersonName.Contains("john", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personAdded, filteredPersons);
                    }
                }            
            }
        }


        [Fact]
        public async Task GetFilteredPersons_ThrowsArgumentExceptionIfSearchByIsInvalid()
        {
            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

           await Assert.ThrowsAsync<ArgumentException>(async () =>await _personsService.GetFilteredPersons("invalidproperty", "searchTerm"));
        }


        [Fact]
        public async Task GetFilteredPersons_ReturnsAllPersonsIfSearchByNull()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(null, "john");

            foreach (PersonResponse person in filteredPersons)
            {
                Assert.Contains(person, personsAdded);
            }
        }

        [Fact]
        public async Task GetFilteredPersons_ReturnsAllPersonsIfSearchTextIsEmptyString()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            foreach(PersonResponse person in filteredPersons)
            {
                Assert.Contains(person, personsAdded);
            }
        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedOptions_ReturnsEmptyListIfInputListIsEmpty()
        {
           List<PersonResponse> personsToSort = new List<PersonResponse>();          

           Assert.Empty(await _personsService.GetSortedPersons(personsToSort,nameof(Person.PersonName),SortOrderOptions.ASC));
        }

        [Fact]
        public async Task GetSortedOptions_ReturnsSortedListIfSortByIsValid()
        {

            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse> listFromGetSortedPersons =await _personsService.GetSortedPersons(personsAdded,nameof(Person.PersonName),SortOrderOptions.DESC);
            List<PersonResponse> sortedPersons = personsAdded.OrderByDescending(person => person.PersonName).ToList();

            for(int i = 0;i < sortedPersons.Count; i++)
            {
                Assert.Equal(sortedPersons[i], listFromGetSortedPersons[i]);
            }
        }


        [Fact]
        public async void GetSortedOptions_ThrowsArgumentExceptionIfSearchByIsInvalid()
        {

            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

           await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.GetSortedPersons(personsAdded,"invalidproperty", SortOrderOptions.DESC));
        }


        [Fact]
        public async Task GetSortedOptions_ReturnsAllPersonsIfSortByNull()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "John Doe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            List<PersonResponse> filteredPersons = await _personsService.GetSortedPersons(personsAdded,null, SortOrderOptions.DESC);

            foreach (PersonResponse person in filteredPersons)
            {
                Assert.Contains(person, personsAdded);
            }
        }


        #endregion

        #region UpdatePersons

        [Fact]
        public async Task UpdatePerson_ThrowsArgumentNullExceptionIfPersonUpdateRequestNull()
        {
          await  Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.UpdatePerson(null));
        }


        [Fact]
        public async Task UpdatePerson_ReturnsTheUpdatedPersonDetailsIfUpdateRequestIsValid()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            PersonAddRequest personToAdd = new PersonAddRequest()
            {
                PersonName = "John Doe1",
                Email = "person1@email.com",
                DateOfBirth = new DateTime(1999, 12, 31),
                Gender = ServiceContracts.Enums.GenderOptions.Male,
                CountryID = countryResponse.CountryID,
                Address = "Address",
                ReceiveNewsLetters = true
            };
            PersonResponse expectedUpdatedPerson = await _personsService.AddPerson(personToAdd);

            expectedUpdatedPerson.PersonName = "updated name";

            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = expectedUpdatedPerson.PersonID,
                PersonName = "updated name",
                Email = "person1@email.com",
                DateOfBirth = new DateTime(1999, 12, 31),
                Gender = ServiceContracts.Enums.GenderOptions.Male,
                CountryID = countryResponse.CountryID,
                Address = "Address",
                ReceiveNewsLetters = true
            };

            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);            

            Assert.Equal(expectedUpdatedPerson, updatedPerson);

        }

        [Fact]
        public async Task UpdatePerson_ThrowsArgumentExceptionIfPersonIDIsInvalid()
        {
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };

         await  Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ThrowsArgumentExceptionIfPersonNameNull()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            PersonAddRequest personToAdd = new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                };
            PersonResponse personAdded = await _personsService.AddPerson(personToAdd);


            PersonUpdateRequest? personUpdateRequest = personAdded.ToPersonUpdateRequest();

            personUpdateRequest.PersonName = null;

           await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ReturnsFalseIfIdDoesNotExist ()
        {
            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            

            foreach (PersonAddRequest person in personsToAdd)
            {
              await _personsService.AddPerson(person);
            }

            Assert.False(await _personsService.DeletePerson(new Guid()));
        }


        [Fact]
        public async Task DeletePerson_ReturnsTrueIfIdExist()
        {
            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
                await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            Guid idToDelete = personsAdded[1].PersonID;

            Assert.True(await _personsService.DeletePerson(idToDelete));

        }


        [Fact]
        public async Task DeletePerson_RemovesPersonFromListIfIDValid()
        {
            CountryAddRequest countryAddRequest = new()
            {
                CountryName = "Japan"
            };

            CountryResponse countryResponse =
               await _countriesService.AddCountry(countryAddRequest);


            List<PersonAddRequest> personsToAdd = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                      PersonName = "John Doe1",
                      Email = "person1@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "john Doe2",
                      Email = "person2@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                      PersonName = "J johnDoe3",
                      Email = "person3@email.com",
                      DateOfBirth = new DateTime(1999,12,31),
                      Gender = ServiceContracts.Enums.GenderOptions.Male,
                      CountryID = countryResponse.CountryID,
                      Address = "Address",
                      ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personsAdded = new List<PersonResponse>();

            foreach (PersonAddRequest person in personsToAdd)
            {
                personsAdded.Add(await _personsService.AddPerson(person));
            }

            PersonResponse personToDelete = personsAdded[1];

           await _personsService.DeletePerson(personToDelete.PersonID);

            List<PersonResponse?> personsListAfterDeletion = await _personsService.GetAllPersons();

            Assert.DoesNotContain(personToDelete, personsListAfterDeletion);
        }

        #endregion
    }
}
