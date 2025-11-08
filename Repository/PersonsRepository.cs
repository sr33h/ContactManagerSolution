using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
           _db.Persons.RemoveRange(_db.Persons.Where(x => x.PersonID == personID));
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Person>> GetAllPersons()
        {
           return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
           return await _db.Persons.Include("Country")
                .Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByID(Guid? personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(x => x.PersonID == personId);
        }

        

        public async Task<Person> UpdatePerson(Person person)
        {
           Person? personToUpdate = await _db.Persons.FirstOrDefaultAsync(x => x.PersonID == person.PersonID);

            if (personToUpdate== null)
            {
                return person;
            } 

            personToUpdate.PersonName = person.PersonName;
            personToUpdate.Email = person.Email;
            personToUpdate.TIN = person.TIN;
            personToUpdate.Gender = person.Gender;
            personToUpdate.Address = person.Address;
            personToUpdate.DateOfBirth = person.DateOfBirth;
            personToUpdate.ReceiveNewsLetters = person.ReceiveNewsLetters;
            personToUpdate.CountryID = person.CountryID;
            personToUpdate.Country = person.Country;
            personToUpdate.PersonID = person.PersonID;

            await _db.SaveChangesAsync();

            return personToUpdate;
        }
    }
}
