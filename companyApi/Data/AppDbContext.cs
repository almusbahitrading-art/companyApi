using Microsoft.EntityFrameworkCore;

namespace companyApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDto> EmployeeResults { get; set; } // هذا ليس جدول فعلي بل مجرد استخدام للاستعلام
        public object HireDate { get; internal set; }
        public DbSet<HireDateDto> HireDateResults { get; set; }
        public DbSet<EmployeeDto> BaseEmployeeResults { get; set; }
    }
}
