using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age {  get; set; }


        public override bool Equals(object? obj)
        {
            if(obj is PersonResponse c)
            {
                return this.PersonID == c.PersonID && this.PersonName == c.PersonName && this.Email == c.Email && this.Gender == c.Gender;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date of Birth: " +
                $"{DateOfBirth?.ToString("dd MMM yyyy")},Gender: {Gender}, Country ID: {CountryID}, Country Name: {CountryName}, " +
                $"Address: {Address}, Receive News Letters: {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions),this.Gender,true),
                Address = this.Address,
                CountryID = this.CountryID,
                ReceiveNewsLetters = this.ReceiveNewsLetters,
            };
        }

    }

    public static class PersonExtensions
    {
        public static PersonResponse? ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                CountryName = person.Country?.CountryName

            };
        }
    }
}
