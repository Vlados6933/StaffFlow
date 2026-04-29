using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person ID can't be blank")]
        public Guid PersonID { get; set; }

        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be valid email")]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Position can't be blank")]
        public string? Position { get; set; }
        [Required(ErrorMessage = "Salary can't be blank")]
        public string? Salary { get; set; }

        /// <summary>
        /// Converts the current object of PersonAddRequest into a new object of Person type
        /// </summary>
        /// <returns>Returns Person object</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                CountryID = CountryID,
                Position = Position,
                Salary = Salary
            };
        }
    }
}
