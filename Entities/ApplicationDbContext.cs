using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole, Guid>
    {
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            List<Country> countries = new List<Country>() {
                new Country() { CountryID = Guid.Parse("D8C07225-BC6A-4B15-B520-A1F7131F46F6"),CountryName="USA" },
                new Country() { CountryID = Guid.Parse("01EF672F-5AEF-4C0C-8474-BABF107220FD"), CountryName = "UK" },
                new Country() { CountryID = Guid.Parse("920F3264-4D96-4245-9F2F-7FF14D766887"), CountryName = "Germany" },
                new Country() { CountryID = Guid.Parse("A2053BF3-7AAA-4C77-A23D-8FBBEA341754"), CountryName = "Russia" }
                };

            foreach(var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            List<Person> persons = new List<Person>()
                {
                    new Person()
                    { PersonID = Guid.Parse("A7AB253E-05C7-4BF7-93AA-22E551124171"),
                      PersonName = "John Doe1",
                      Address = "address1",
                      Email = "johndoe1@gmail.com",
                      DateOfBirth = DateTime.Parse("1999-12-31"),
                      Gender = "Female",
                      ReceiveNewsLetters = true,
                      CountryID = Guid.Parse("920F3264-4D96-4245-9F2F-7FF14D766887")
                    },
                    new Person()
                    { PersonID = Guid.Parse("ED9623AA-E92C-4170-8C76-F0660D783F05"),
                      PersonName = "John Doe2",
                      Address = "address2",
                      Email = "johndoe2@gmail.com",
                      DateOfBirth = DateTime.Parse("1999-12-31"),
                      Gender = "Other",
                      ReceiveNewsLetters = true,
                      CountryID = Guid.Parse("D8C07225-BC6A-4B15-B520-A1F7131F46F6")
                    },
                    new Person()
                    { PersonID = Guid.Parse("9219BBDD-D30C-45F4-929F-024D9904F188"),
                      PersonName = "John Doe3",
                      Address = "address3",
                      Email = "johndoe3@gmail.com",
                      DateOfBirth = DateTime.Parse("1999-12-31"),
                      Gender = "Female",
                      ReceiveNewsLetters = true,
                      CountryID = Guid.Parse("01EF672F-5AEF-4C0C-8474-BABF107220FD")
                    },
                    new Person()
                    { PersonID = Guid.Parse("FDB0AE15-1D53-460D-9352-75A67293C731"),
                      PersonName = "John Doe4",
                      Address = "address4",
                      Email = "johndoe4@gmail.com",
                      DateOfBirth = DateTime.Parse("1999-12-31"),
                      Gender = "Male",
                      ReceiveNewsLetters = true,
                      CountryID =Guid.Parse("A2053BF3-7AAA-4C77-A23D-8FBBEA341754")
                    }

                };

            foreach(Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent API
            modelBuilder.Entity<Person>().Property(x => x.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
       }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] 
            {
                new SqlParameter("@PersonID",person.PersonID),
                new SqlParameter("@PersonName",person.PersonName),
                new SqlParameter("@Email",person.Email),
                new SqlParameter("@DateOfBirth",person.DateOfBirth),
                new SqlParameter("@Gender",person.Gender),
                new SqlParameter("@CountryID",person.CountryID),
                new SqlParameter("@Address",person.Address),
                new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters)
            };

           return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson]" +
                " @PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID," +
                "@Address,@ReceiveNewsLetters",parameters);


        }
    }
}
