using CompanyEmployees.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("api/attendance")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly ISender _sender;

    public AttendanceController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Records employee daily check-in
    /// </summary>
    /// <param name="attendanceForCreationDto">Attendance check-in data</param>
    /// <returns>Created attendance record</returns>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] AttendanceForCreationDto attendanceForCreationDto)
    {
        if (attendanceForCreationDto is null)
            return BadRequest("AttendanceForCreationDto object is null");

        var attendance = await _sender.Send(new CheckInAttendanceCommand(attendanceForCreationDto));

        return Ok(attendance);
    }
}
