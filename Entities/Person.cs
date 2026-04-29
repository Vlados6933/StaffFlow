using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)] //nvarchar(40)
        public string? PersonName { get; set; }
        [StringLength(40)] //nvarchar(40)
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public string? Salary { get; set; }
        [StringLength(100)]
        public string? Position { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date Of Birth: {DateOfBirth?.ToString("MM/dd/yyyy")}, Gender: {Gender}, Country ID: {CountryID}, Country: {Country?.CountryName}, Salary {Salary}, Position {Position}";
        }
    }
}
