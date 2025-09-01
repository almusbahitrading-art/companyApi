using companyApi.Controllers;
using companyApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace companyApi.Data
{
    public class EmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<EmployeeDto>> GetAllFromFunctionAsync()

        {
            var employees = await _context.EmployeeResults
                    .FromSqlRaw("SELECT * FROM SelectAllEmployeeIds()")
                    .ToListAsync();

            return employees;
        }


        public async Task<IEnumerable<EmployeeDto>> GetFromFunctionByIdAsync(int id)
        {
            var employees = await _context.EmployeeResults
                    .FromSqlRaw("SELECT * FROM GetEmployeeByIds({0})", id)
                    .ToListAsync();

            return employees;
        }
        

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var sql = "CALL employee_operation(@op_type,@emp_id,@emp_name,@emp_department,@emp_salary)";

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                new NpgsqlParameter("@op_type", "DELETE"),
                new NpgsqlParameter("@emp_id", id),
                new NpgsqlParameter("@emp_name", DBNull.Value),
                new NpgsqlParameter("@emp_department", DBNull.Value),
                new NpgsqlParameter("@emp_salary", DBNull.Value)
                );

            return true;
        }


        public async Task<bool> InsertEmployeeAsync(EmployeeDto dto)
        {
            var sql = "CALL employee_operation(@op_type, @emp_id, @name, @department, @salary);";

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                new NpgsqlParameter("@op_type", "INSERT"),
                    new NpgsqlParameter("@emp_id", DBNull.Value),
                    new NpgsqlParameter("@name", dto.Name),
                    new NpgsqlParameter("@department", dto.Department),
                    new NpgsqlParameter("@salary", dto.Salary)
                );

            return true;
        }


        public async Task<bool> UpdateEmployeeAsync(EmployeeDto dto)
        {
            var sql = "CALL employee_operation(@op_type,@emp_id,@emp_name,@emp_department,@emp_salary)";

            await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new NpgsqlParameter("@op_type", "UPDATE"),
                    new NpgsqlParameter("@emp_id", dto.Id),
                    new NpgsqlParameter("@emp_name", dto.Name),
                    new NpgsqlParameter("@emp_department", dto.Department),
                    new NpgsqlParameter("@emp_salary", dto.Salary)
                    );

            return true;
        }


        public async Task<bool> UpdateEmployeePartialAsync(int id, EmployeeDto employeeToPatch)
        {
            var sql = "CALL employee_operation(@op_type,@emp_id,@emp_name,@emp_department,@emp_salary)";

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                new NpgsqlParameter("@op_type", "UPDATE"),
                new NpgsqlParameter("@emp_id", id),
                new NpgsqlParameter("@emp_name", employeeToPatch.Name),
                new NpgsqlParameter("@emp_department", employeeToPatch.Department),
                new NpgsqlParameter("@emp_salary", employeeToPatch.Salary)
            );

            return true;
        }


        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByHireDateAsync(DateTime hire_date)
        {
            var dateString = hire_date.ToString("yyyy-MM-dd");

            var result = await _context.EmployeeResults
                .FromSqlRaw("SELECT * FROM SelectByHireDate(CAST(@p0 AS DATE))", dateString)
                .ToListAsync();

            return result;
        }


        public async Task<IEnumerable<HireDateDto>> GetAllDistinctHireDatesAsync()
        {
            var hiredate = await _context.HireDateResults
                    .FromSqlRaw("SELECT * FROM SelectAllDistinctHireDates()")
                    .ToListAsync();

            return hiredate;
        }


        public async Task<IEnumerable<EmployeeDto>> GetBaseEmployeeDataAsync()
        {
            var employees = await _context.BaseEmployeeResults
                    .FromSqlRaw("SELECT * FROM BaseEmployeeQuery()")
                    .ToListAsync();

            return employees;
        }


    }
}
