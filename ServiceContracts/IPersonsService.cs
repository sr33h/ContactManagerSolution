using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
       Task<PersonResponse> AddPerson(PersonAddRequest? request);
       Task<List<PersonResponse?>> GetAllPersons();

       Task<PersonResponse?> GetPersonByID(Guid? personId);

       Task<List<PersonResponse?>> GetFilteredPersons(string searchBy, string? searchString);

       Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse?> personsToBeSorted, string sortBy, SortOrderOptions sortOrder);

       Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request);

       Task<bool> DeletePerson(Guid? personID);

       Task<MemoryStream> GetPersonsCSV();

       Task<MemoryStream> GetPersonsExcel();

    }
}
