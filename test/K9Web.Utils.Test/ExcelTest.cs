using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using EPPlus.Core.Extensions;
using Xunit;

namespace K9Web.Utils.Test
{
    public class ExcelTest
    {
        [Fact]
        public void Export()
        {
            var persons = Generate();
            using (var excelPackage = persons.ToExcelPackage())
            {
                excelPackage.SaveAs(new FileInfo("person.xlsx"));
            }
        }

        private static IList<PersonDto> Generate(int rows = 1000)
        {
            return Enumerable.Range(1, rows)
                .Select(x => new PersonDto
                {
                    FirstName = "FirstName" + x,
                    LastName = "LastName" + x,
                    YearBorn = 1900 + x,
                    NotMapped = x
                })
                .ToList();
        }
    }

    public class PersonDto
    {
        [ExcelTableColumn("First name")]
        [Required(ErrorMessage = "First name cannot be empty.")]
        [MaxLength(50, ErrorMessage = "First name cannot be more than {1} characters.")]
        public string FirstName { get; set; }

        [ExcelTableColumn("Last name")]
        public string LastName { get; set; }

        [ExcelTableColumn(3)]
        [Range(1900, 9999, ErrorMessage = "Please enter a value bigger than {1}")]
        public int YearBorn { get; set; }

        public decimal NotMapped { get; set; }
    }
}
