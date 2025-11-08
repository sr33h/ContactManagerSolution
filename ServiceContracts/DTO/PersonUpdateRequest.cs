using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Mandatory field")]
        public Guid PersonID { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public Guid? CountryID { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Mandatory field")]
        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
