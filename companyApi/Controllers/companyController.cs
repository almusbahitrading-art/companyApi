using companyApi.Data;
using companyApi.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using System.Diagnostics;


namespace companyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class companyController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly EmployeeRepository _employeeRepository;
        public companyController(AppDbContext context)
        {
            _context = context;
            _employeeRepository = new EmployeeRepository(context);
        }


        /// <summary>
        /// Fetches all employee records by executing the SQL function SelectAllEmployeeIds from the database.
        /// Returns a list of EmployeeDto objects representing each employee.
        /// </summary>
        /// <returns>
        /// HTTP 200 with the list of employees on success, or HTTP 500 with error details if the operation fails.
        /// </returns>

      
        [HttpGet]
        [Route("AllFromFunction", Name = "GetAllFromFunction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllFromFunction()
        {
            Log.Debug("Executing {Action}", nameof(GetAllFromFunction));

            try
            {
                var employees = await _employeeRepository.GetAllFromFunctionAsync();

                if (employees == null || !employees.Any())
                {
                    Log.Warning("No employees returned from {Action}", nameof(GetAllFromFunction));
                }
                else
                {
                    Log.Information("Successfully retrieved {Count} employees From {Action}", nameof(GetAllFromFunction),employees.Count());
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while executing {Action}", nameof(GetAllFromFunction));
                return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
            }
        }


        /// <summary>
        /// Retrieves employee records by a specific ID using the SQL function GetEmployeeByIds.
        /// Executes a raw SQL query and returns a list of matching EmployeeDto objects.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to retrieve.</param>
        /// <returns>
        /// HTTP 200 with a list of matching employees if found.
        /// HTTP 404 if no employee matches the provided ID.
        /// HTTP 500 if a server-side error occurs during data retrieval.
        /// </returns>

        [HttpGet]
        [Route("FromFunctionById/{id}", Name = "GetFromFunctionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetFromFunctionById(int id)
        {
            Log.Debug("Executing GetFromFunctionById with id: {Id}", id);

            try
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.id == id);
                if (!employeeExists)
                {
                    Log.Warning("{Action} Employee with Id {Id} not found", nameof(GetFromFunctionById),id);
                    return NotFound($"Employee With Id {id} Not Found.");
                }

                var employees = await _employeeRepository.GetFromFunctionByIdAsync(id);

                Log.Information("Successfully retrieved employees for Id {Id}", id);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while executing GetFromFunctionById for Id {Id}", id);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes an employee record by ID using the stored procedure employee_operation.
        /// The procedure is called with the operation type "DELETE" and the specified employee ID.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <returns>
        /// HTTP 200 if the deletion is successful.
        /// HTTP 400 if the request is invalid.
        /// HTTP 404 if the employee with the given ID does not exist.
        /// HTTP 500 if a server-side error occurs during the operation.
        /// </returns>

        [HttpDelete]
        [Route("{id}", Name = "DeleteEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            Log.Debug("Excuting {Action} with id {Id}", nameof(DeleteEmployee),id);

            try
            {
                Log.Information("Start excute {Action}", nameof(DeleteEmployee));
                var employeeExists = await _context.Employees.AnyAsync(e => e.id == id);
                if (!employeeExists) 
                {
                    Log.Warning("{Action} Employee With Id {id} Not Found.", nameof(DeleteEmployee),id);
                    return NotFound($"Employee With Id {id} Not Found.");
                }

                await _employeeRepository.DeleteEmployeeAsync(id);

                Log.Information("Done excute {Action} from companyController page line 145 and delete employee Id {id}.", nameof(DeleteEmployee), id);
                return Ok($"Employee with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There is an error at excute {Action}", nameof(DeleteEmployee));
                return StatusCode(500, "An unexpected error occurred. Please try again later." + ex.Message);
            }
        }


        /// <summary>
        /// Inserts a new employee record into the database using the stored procedure employee_operation.
        /// The procedure is called with the operation type "INSERT" and the employee details provided in the request body.
        /// </summary>
        /// <param name="dto">An EmployeeDto object containing the employee's name, department, and salary.</param>
        /// <returns>
        /// HTTP 201 if the employee is successfully inserted.
        /// HTTP 400 if the request data is invalid or incomplete.
        /// HTTP 500 if a server-side error occurs during the insertion process.
        /// </returns>
        
        [HttpPost]
        [Route("InsertEmployee")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsertEmployee([FromBody] EmployeeDto dto)
        {
            Log.Debug("Excuting InsertEmployee {Action}", nameof(InsertEmployee));

            try
            {
                if(string.IsNullOrEmpty(dto.Name) ||string.IsNullOrEmpty(dto.Department) )
                {
                    Log.Warning("{Action} called with invalid DTO: {@Dto}", nameof(InsertEmployee),dto);
                    return BadRequest("Employee data is required and must be valid.");
                }

                Log.Information("Start excute {Action}", nameof(InsertEmployee));
                await _employeeRepository.InsertEmployeeAsync(dto);

                Log.Information("Done excute {Action} from companyController page line 188" , nameof(InsertEmployee));
                return StatusCode(201, "Employee inserted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There is an error at excute {Action}", nameof(InsertEmployee));
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        

        /// <summary>
        /// Updates an existing employee record using the stored procedure employee_operation.
        /// The procedure is called with the operation type "UPDATE" and the updated employee details provided in the request body.
        /// </summary>
        /// <param name="dto">An EmployeeDto object containing the employee's ID and updated information (name, department, salary).</param>
        /// <returns>
        /// HTTP 200 if the update is successful.
        /// HTTP 400 if the request data is invalid.
        /// HTTP 404 if the employee with the given ID does not exist.
        /// HTTP 500 if a server-side error occurs during the update process.
        /// </returns>


        [HttpPut]
        [Route("{id}", Name = "UpdateEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto dto)
        {
            Log.Debug("Excuting {Action} with EmployeeID = {EmployeeId}", nameof(UpdateEmployee) , id);

            try
            {
                Log.Information("Start excute {Action} with EmployeeID = {EmployeeId}", nameof(UpdateEmployee), id);

                var employeeExists = await _context.Employees.AnyAsync(e => e.id == id);
                if (!employeeExists)
                {
                    Log.Warning("Employee with Id {id} not found.", id);
                    return NotFound($"Employee with ID {id} not found.");
                }

                if (dto.Id != id)
                    return BadRequest("Mismatch between route ID and body ID.");

                await _employeeRepository.UpdateEmployeeAsync(dto);

                Log.Information("Done excute {Action}  from companyController page line 240", nameof(UpdateEmployee));
                return Ok($"Employee with ID {dto.Id} updated successfully.");
            }
            catch (Exception ex) 
            {
                Log.Error(ex, "There is an error at excute {Action}", nameof(UpdateEmployee));
                return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
            }
        }


        /// <summary>
        /// Partially updates an existing employee using a JSON Patch document.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to be updated.</param>
        /// <param name="dto">The JSON Patch document containing the changes to apply to the employee.</param>
        /// <returns>
        /// Returns 200 OK if the update is successful, 
        /// 400 Bad Request if the input is invalid, 
        /// 404 Not Found if the employee does not exist, 
        /// or 500 Internal Server Error if an unexpected error occurs.
        /// </returns>


        //[HttpPatch]
        //[Route("{id}", Name = "UpdateEmployeePartial")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> UpdateEmployeePartial(int id, [FromBody] JsonPatchDocument<EmployeeDto> dto)
        //{
        //    Log.Debug("Excuting UpdateEmployeePartial");

        //    try
        //    {
        //        Log.Information("Start excute UpdateEmployeePartial");
        //        var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.id == id);
        //        if (existingEmployee == null)
        //        {
        //            Log.Warning("Employee with ID {id} not found.", id);
        //            return NotFound($"Employee with ID {id} not found.");
        //        }

        //        var employeeToPatch = new EmployeeDto
        //        {
        //            Id = existingEmployee.id,
        //            Name = existingEmployee.name,
        //            Department = existingEmployee.department,
        //            Salary = existingEmployee.salary
        //        };

        //        dto.ApplyTo(employeeToPatch);

        //        await _employeeRepository.UpdateEmployeePartialAsync(id,employeeToPatch);

        //        Log.Information($"Done excute {nameof(UpdateEmployeePartial)} from companyController page line 296.");
        //        return Ok($"Employee with ID {id} updated successfully.");
        //    }

        //    catch (Exception ex) 
        //    {
        //        Log.Error(ex, "There is an error at excute UpdateEmployeePartial");
        //        return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
        //    }
        //}

        [HttpPatch("{id}", Name = "UpdateEmployeePartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmployeePartial(int id, [FromBody] JsonPatchDocument<EmployeeDto> dto)
        {
            Log.Debug("Executing {Action} with EmployeeId={EmployeeId}", nameof(UpdateEmployeePartial), id);

            try
            {
                Log.Information("Start executing {Action} for EmployeeId={EmployeeId}", nameof(UpdateEmployeePartial), id);

                var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.id == id);
                if (existingEmployee == null)
                {
                    Log.Warning("Employee with ID {EmployeeId} not found", id);
                    return NotFound($"Employee with ID {id} not found.");
                }

                var employeeToPatch = new EmployeeDto
                {
                    Id = existingEmployee.id,
                    Name = existingEmployee.name,
                    Department = existingEmployee.department,
                    Salary = existingEmployee.salary
                };

                dto.ApplyTo(employeeToPatch);

                await _employeeRepository.UpdateEmployeePartialAsync(id, employeeToPatch);

                Log.Information("Successfully executed {Action} for EmployeeId={EmployeeId}", nameof(UpdateEmployeePartial), id);
                return Ok($"Employee with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing {Action} for EmployeeId={EmployeeId}", nameof(UpdateEmployeePartial), id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }



        /// <summary>
        /// Retrieves a list of employees who were hired on a specific date by executing the SQL function SelectByHireDate.
        /// The hire date is passed as a query parameter and cast to DATE format in the SQL query.
        /// </summary>
        /// <param name="hire_date">The date on which the employees were hired (passed via query string).</param>
        /// <returns>
        /// HTTP 200 with a list of matching EmployeeDto objects if found.
        /// HTTP 404 if no employees match the specified hire date.
        /// HTTP 500 if a server-side error occurs during data retrieval.
        /// </returns>


        [HttpGet]
        [Route("ByHireDate", Name = "GetEmployeesByHireDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByHireDate([FromQuery] DateTime hire_date)
        {
            Log.Debug("Excuting {Action}" , nameof(GetEmployeesByHireDate));

            try
            {
                Log.Information("Start excute {Action}", nameof(GetEmployeesByHireDate));

                var employee= await _employeeRepository.GetEmployeesByHireDateAsync(hire_date);
                
                
                var dtoList = employee.Select(r => new EmployeeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Department = r.Department,
                    Salary = r.Salary,
                    HireDate = r.HireDate
                }).ToList();

                if (dtoList.Count == 0)
                {
                    Log.Warning("{Action} No employees found for the given hire date {hire_date}.", nameof(GetEmployeesByHireDate), hire_date);
                    return NotFound("No employees found for the given hire date.");
                }

                Log.Information("Done excute {Action} from companyController page line 351 hire_date {hire_date}.", nameof(GetEmployeesByHireDate), hire_date);
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There is an error at excute {Action}", nameof(GetEmployeesByHireDate));
                return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
            }
        }


        /// <summary>
        /// Retrieves all distinct employee hire dates by executing the SQL function SelectAllDistinctHireDates.
        /// Useful for filtering, reporting, or generating date-based analytics.
        /// </summary>
        /// <returns>
        /// HTTP 200 with a list of unique hire dates wrapped in EmployeeDto objects.
        /// HTTP 500 if a server-side error occurs during data retrieval.
        /// </returns>
 

        [HttpGet]
        [Route("AllHireDates", Name = "GetAllDistinctHireDates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllDistinctHireDates()
        {
            Log.Debug("Excuting {Action}", nameof(GetAllDistinctHireDates));

            try
            {
                Log.Information("Start excute {Action}", nameof(GetAllDistinctHireDates));
                var hiredate = await _employeeRepository.GetAllDistinctHireDatesAsync();

                Log.Information("Done excute {Action} from companyController page line 385.", nameof(GetAllDistinctHireDates));
                return Ok(hiredate);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There is an error at excute GetAllDistinctHireDates");
                return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
            }
        }


        /// <summary>
        /// Retrieves a simplified set of employee data by executing the SQL function BaseEmployeeQuery.
        /// This endpoint is useful for scenarios where only core employee information is needed (e.g., ID, name, department).
        /// </summary>
        /// <returns>
        /// HTTP 200 with a list of EmployeeDto objects containing base employee data.
        /// HTTP 500 if a server-side error occurs during data retrieval.
        /// </returns>


        [HttpGet]
        [Route("BaseEmployee", Name = "GetBaseEmployeeData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetBaseEmployeeData()
        {
            Log.Debug("Excuting {Action}", nameof(GetBaseEmployeeData));

            try
            {
                Log.Information("Start excute {Action}", nameof(GetBaseEmployeeData));
                var emploees = await _employeeRepository.GetBaseEmployeeDataAsync();

                Log.Information("Done excute {Action} from companyController page line 419.", nameof(GetBaseEmployeeData));
                return Ok(emploees);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There is an error at excute GetBaseEmployeeData");
                return StatusCode(500, "An unexpected error occurred. Please try again later. " + ex.Message);
            }
        }

    }
}

