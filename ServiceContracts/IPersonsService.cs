using System;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonsService
    {
       PersonResponse AddPerson(PersonAddRequest? request);
       List<PersonResponse> GetAllPersons();

    }
}
