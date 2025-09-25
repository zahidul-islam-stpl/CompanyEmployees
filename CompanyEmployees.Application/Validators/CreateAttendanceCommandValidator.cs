using CompanyEmployees.Application.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace CompanyEmployees.Application.Validators
{
    public sealed class CreateAttendanceCommandValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public CreateAttendanceCommandValidator()
        {
            RuleFor(c => c.EmployeeId).NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(c => c.Attendance.WorkDate)
                .NotEmpty().WithMessage("Work date is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Work date cannot be in the future.");

            RuleFor(c => c.Attendance.CheckInUtc)
                .Must(BeValidCheckInTime)
                .WithMessage("Check-in time cannot be in the future.")
                .When(c => c.Attendance.CheckInUtc.HasValue);

            RuleFor(c => c.Attendance.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.")
                .When(c => !string.IsNullOrEmpty(c.Attendance.Notes));
        }

        public override ValidationResult Validate(ValidationContext<CreateAttendanceCommand> context)
        {
            return context.InstanceToValidate.Attendance is null
                ? new ValidationResult(new[] { new ValidationFailure("AttendanceForCreationDto",
                    "AttendanceForCreationDto object is null") })
                : base.Validate(context);
        }

        private static bool BeValidCheckInTime(DateTime? checkInTime)
        {
            return !checkInTime.HasValue || checkInTime.Value <= DateTime.UtcNow;
        }
    }
}