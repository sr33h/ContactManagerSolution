using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class PersonService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonsRepository personsRepository, ILogger<PersonService> logger)
        {
            _personsRepository = personsRepository;        
            _logger = logger;
        }

        

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();

            Guid id = Guid.NewGuid();

            person.PersonID = id;

            await _personsRepository.AddPerson(person);
            

           // _db.sp_InsertPerson(person);

           return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse?>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();

           return persons
                .Select(person => person.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByID(Guid? personId)
        {
           if(personId == null)
            {
                throw new ArgumentNullException(nameof(personId));
            }

////           if((await _personsRepository.GetAllPersons()).ToList().Count == 0 )
//            {
//                return null;
//            }

            Person? person = await _personsRepository.GetPersonByID(personId.Value);

            return person?.ToPersonResponse();
        }

        public async Task<List<PersonResponse?>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFiltered persons executing");


            List<Person> matchingPersons = searchBy switch
            {
                nameof(Person.PersonName) =>
                     await _personsRepository.GetFilteredPersons(p =>
                     p.PersonName.Contains(searchString)),


                nameof(Person.Email) =>
                  await _personsRepository.GetFilteredPersons(p =>
                  p.Email.Contains(searchString)),

                nameof(Person.Gender) =>
                    await _personsRepository.GetFilteredPersons(p =>
                    p.Gender.Equals(searchString)),

                nameof(Person.Address) =>
                   await _personsRepository.GetFilteredPersons(p =>
                   p.Address.Contains(searchString)),

                nameof(Person.CountryID) =>
                    await _personsRepository.GetFilteredPersons(p =>
                    p.Country.CountryName.Contains(searchString)),

                nameof(Person.DateOfBirth) =>
                   await _personsRepository.GetFilteredPersons(p =>
                   p.DateOfBirth.Value.ToString("dd MMM yyyyy")
                   .Contains(searchString)),


                _ => await _personsRepository.GetAllPersons()

            };
                return matchingPersons.Select(p => p.ToPersonResponse()).ToList();

        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> personsToBeSorted, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation("GetSortedPersons executing");

           if(string.IsNullOrEmpty(sortBy)) return personsToBeSorted;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)

           switch
            {
                (nameof(Person.PersonName), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.PersonName), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.Email), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.Email), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.DateOfBirth), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.DateOfBirth).ToList(),

                (nameof(Person.DateOfBirth), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.Gender).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.Gender).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.CountryName).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.CountryName).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                personsToBeSorted.OrderBy(p => p.Address).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                personsToBeSorted.OrderByDescending(p => p.Address).ToList(),

                _ => throw new ArgumentException(sortBy)
            };

            return sortedPersons;
        }

        public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request)
        {
            if(request == null) throw new ArgumentNullException("Person update request cannot be null");

            if (request.PersonName == null) throw new ArgumentException("Person Name cannot be null");

            if(! (await _personsRepository.GetAllPersons()).ToList().Any(p => p.PersonID == request.PersonID))
            {
                throw new ArgumentException("Invalid person ID");
            }

            ValidationHelper.ModelValidation(request);

            Person? personToUpdate = await _personsRepository.GetPersonByID(request.PersonID);

            personToUpdate.PersonName = request.PersonName;
            personToUpdate.Address = request.Address;
            personToUpdate.Gender = request.Gender.ToString();
            personToUpdate.Email = request.Email;
            personToUpdate.CountryID = request.CountryID;
            personToUpdate.DateOfBirth = request.DateOfBirth;
            personToUpdate.ReceiveNewsLetters = request.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(personToUpdate);


            return personToUpdate.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
           if((await _personsRepository.GetAllPersons()).ToList().Count == 0 || personID==null || personID.Value == Guid.Empty) return false;

           if(!(await _personsRepository.GetAllPersons()).ToList().Any( p => p.PersonID == personID)) return false;

            await _personsRepository.DeletePerson(personID.Value);

            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();

            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.CountryName));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

            csvWriter.NextRecord();

            List<PersonResponse?> persons = await GetAllPersons();


            foreach(PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.CountryName);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                if(person.DateOfBirth != null)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MMM-dd"));
                }
                else
                {
                    csvWriter.WriteField("");
                }
                
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

           // await csvWriter.WriteRecordsAsync(persons);

            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
           MemoryStream memoryStream = new MemoryStream();
           using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsList");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Address";
                worksheet.Cells["C1"].Value = "Email";
                worksheet.Cells["D1"].Value = "Date of Birth";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";

                using (ExcelRange headercells = worksheet.Cells["A1:F1"])
                {
                    headercells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headercells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headercells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> personsResponse = await GetAllPersons();

                foreach(PersonResponse personResponse in personsResponse)
                {
                    worksheet.Cells[row, 1].Value = personResponse.PersonName;
                    worksheet.Cells[row, 2].Value = personResponse.Address;
                    worksheet.Cells[row, 3].Value = personResponse.Email;
                    if(personResponse.DateOfBirth != null)
                    {
                        worksheet.Cells[row, 4].Value = personResponse.DateOfBirth.Value.ToString("yyyy-MMM-dd");
                    }
                    else
                    {
                        worksheet.Cells[row, 4].Value = "";
                    }
                    
                    worksheet.Cells[row, 5].Value = personResponse.Gender;
                    worksheet.Cells[row, 6].Value = personResponse.CountryName;

                    row++;
                }

                worksheet.Cells[$"A1:F{row}"].AutoFitColumns();
                

                await excelPackage.SaveAsync();
            }

           memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
