using companyApi.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace companyApi.Models
{
    public class EmployeeDto
    {
        [ValidateNever]
        public int Id { get; set; }


        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Department is required.")]
        [StringLength(20)]
        public string Department { get; set; }


        [Range(2000, 50000, ErrorMessage = "Salary must be between 2000 and 50000.")]
        public decimal Salary { get; set; }

        
        //[DateCheckAtrribute]
        public DateTime HireDate { get; set; }
    }

    [Keyless]
    public class HireDateDto
    {
        public DateTime HireDate { get; set; }
    }

}


//🔹 هذا Data Transfer Object (DTO):

//هدفه تبادل البيانات بين API والـ Clients(Angular/React/Reports...).

//يحتوي على Validation attributes(زي[Required], [Range]) علشان تتأكد من صحة البيانات قبل ما تدخل DB.

//ما له علاقة بالـ DB مباشرة(ما فيه [Table] أو[Key]).

//ممكن تضيف فيه خصائص زيادة(DisplayName, Calculated Fields) غير موجودة في قاعدة البيانات.

//يخلي الـ API مستقل عن شكل قاعدة البيانات(لو غيرت DB ما يتأثر الـ Client).
