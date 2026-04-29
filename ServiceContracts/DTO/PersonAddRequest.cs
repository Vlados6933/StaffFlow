using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select gender of the person")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Salary can't be blank")]
        public string? Salary { get; set; }

        [Required(ErrorMessage = "Position can't be blank")]
        public string? Position { get; set; }

        /// <summary>
        /// Converts the current object of PersonAddRequest into a new object of Person type
        /// </summary>
        /// <returns>Returns Person object</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                CountryID = CountryID,
                Salary = Salary,
                Position = Position
            };
        }
    }
}
