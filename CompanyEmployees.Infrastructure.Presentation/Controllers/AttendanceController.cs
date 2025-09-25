using CompanyEmployees.Application.Commands;
using CompanyEmployees.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ISender _sender;

        public AttendanceController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Record check-in for an employee
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="attendanceForCreation">Attendance creation data</param>
        /// <returns>Created attendance record</returns>
        [HttpPost("employees/{employeeId:guid}/attendance")]
        public async Task<IActionResult> CreateAttendance(Guid employeeId, [FromBody] AttendanceForCreationDto attendanceForCreation)
        {
            if (attendanceForCreation is null)
                return BadRequest("AttendanceForCreationDto object is null");

            var attendance = await _sender.Send(new CreateAttendanceCommand(employeeId, attendanceForCreation));

            return CreatedAtRoute("AttendanceById", new { id = attendance.Id }, attendance);
        }

        /// <summary>
        /// Record check-out for an attendance record
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <param name="checkOut">Check-out data</param>
        /// <returns>Updated attendance record</returns>
        [HttpPut("attendance/{id:guid}/checkout")]
        public async Task<IActionResult> UpdateCheckOut(Guid id, [FromBody] CheckOutDto checkOut)
        {
            if (checkOut is null)
                return BadRequest("CheckOutDto object is null");

            var attendance = await _sender.Send(new UpdateCheckOutCommand(id, checkOut));

            return Ok(attendance);
        }

        /// <summary>
        /// Get employee attendance history
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="fromDate">Start date filter (optional)</param>
        /// <param name="toDate">End date filter (optional)</param>
        /// <returns>List of attendance records for the employee</returns>
        [HttpGet("employees/{employeeId:guid}/attendance")]
        public async Task<IActionResult> GetEmployeeAttendance(Guid employeeId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var attendanceRecords = await _sender.Send(new GetEmployeeAttendanceQuery(employeeId, fromDate, toDate, TrackChanges: false));

            return Ok(attendanceRecords);
        }

        /// <summary>
        /// Get specific attendance record by ID
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <returns>Attendance record details</returns>
        [HttpGet("attendance/{id:guid}", Name = "AttendanceById")]
        public async Task<IActionResult> GetAttendance(Guid id)
        {
            var attendance = await _sender.Send(new GetAttendanceByIdQuery(id, TrackChanges: false));

            return Ok(attendance);
        }

        /// <summary>
        /// Get company attendance summary
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="fromDate">Start date filter (optional)</param>
        /// <param name="toDate">End date filter (optional)</param>
        /// <returns>List of attendance records for all employees in the company</returns>
        [HttpGet("attendance/company/{companyId:guid}")]
        public async Task<IActionResult> GetCompanyAttendance(Guid companyId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var attendanceRecords = await _sender.Send(new GetCompanyAttendanceQuery(companyId, fromDate, toDate, TrackChanges: false));

            return Ok(attendanceRecords);
        }

        /// <summary>
        /// Update attendance record (admin operation)
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <param name="attendanceForUpdate">Attendance update data</param>
        /// <returns>No content on success</returns>
        [HttpPut("attendance/{id:guid}")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] AttendanceForUpdateDto attendanceForUpdate)
        {
            if (attendanceForUpdate is null)
                return BadRequest("AttendanceForUpdateDto object is null");

            await _sender.Send(new UpdateAttendanceCommand(id, attendanceForUpdate, TrackChanges: true));

            return NoContent();
        }
    }
}