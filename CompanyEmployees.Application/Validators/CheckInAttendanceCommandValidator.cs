using CompanyEmployees.Application.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace CompanyEmployees.Application.Validators;

public sealed class CheckInAttendanceCommandValidator : AbstractValidator<CheckInAttendanceCommand>
{
    public CheckInAttendanceCommandValidator()
    {
        RuleFor(a => a.Attendance.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required.");

        // BUG: Missing validation for future dates
        RuleFor(a => a.Attendance.CheckInTime)
            .NotEmpty()
            .WithMessage("Check-in time is required.");

        // BUG: Should validate against allowed status values (Present, Absent, Late, etc.)
        RuleFor(a => a.Attendance.Status)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Status is required and must not exceed 10 characters.");

        RuleFor(a => a.Attendance.Notes)
            .MaximumLength(200)
            .When(a => !string.IsNullOrEmpty(a.Attendance.Notes))
            .WithMessage("Notes must not exceed 200 characters.");
        
        // BUG: Missing validation for:
        // - CheckInTime should not be in the future
        // - CheckInTime should not be too old (e.g., more than 24 hours ago)
        // - Status should be from allowed values
    }

    public override ValidationResult Validate(ValidationContext<CheckInAttendanceCommand> context)
    {
        return context.InstanceToValidate.Attendance is null
            ? new ValidationResult(new[] { new ValidationFailure("AttendanceForCreationDto",
        "AttendanceForCreationDto object is null") })
            : base.Validate(context);
    }
}
