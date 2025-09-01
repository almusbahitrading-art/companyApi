using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace companyApi.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string department { get; set; }

        public decimal salary { get; set; }

        public DateTime hiredate { get; set; }

      //  public DateTime HireDate { get; set; }
    }



    //    هذا Entity Model(EF Core) :

    //يمثل الجدول في قاعدة البيانات(employees).

    //فيه مفاتيح أساسية/أجنبية + Configurations([Key], [Table], [Column]).

    //EF Core يستخدمه علشان يولّد SQL Queries ويحفظ ويقرأ بيانات.

    //ما فيه Validations للمستخدمين، لأنه الهدف منه فقط يعكس الـ DB schema.

    //أي تغيير في الجدول (إضافة عمود/تعديل اسم) يبان هنا.




    //     الفرق الجوهري

    //DTO = Layer of communication(مخصص للتبادل بين الـ API والـ Client).

    //Entity = Layer of persistence(مخصص للتخزين في الـ Database عبر EF Core).



        //    الفائدة لما تستخدم الاثنين:

        //تعزل الداتا الداخلية(DB structure) عن الـ API contract(شو يشوف الـ Client).

        //تتحكم في التحقق من المدخلات(Validations) من غير ما توسخ الـ Entity بخصائص ما لها علاقة بـ DB.

        //تقدر تغيّر قاعدة البيانات أو الجداول بدون ما تكسر الـ API.

        //أمان أكثر (ما تعطي العميل حقول حساسة زي passwordHash أو IsAdmin بالخطأ).

        //مرونة: ممكن API يرجع DTO مختلف كليًا عن الـ Entity(مثلاً FullName بدال FirstName + LastName).

        // إذن:

        //استخدم Employee مع EF Core داخل الـ Repository/DbContext.

        //استخدم EmployeeDto في الـ Controller وواجهات API.

        //اربطهم مع بعض باستخدام AutoMapper أو تحويل يدوي.


}
