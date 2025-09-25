using CompanyEmployees.Application.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace CompanyEmployees.Application.Validators
{
    public sealed class UpdateAttendanceCommandValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        public UpdateAttendanceCommandValidator()
        {
            RuleFor(c => c.AttendanceId).NotEmpty().WithMessage("Attendance ID is required.");

            RuleFor(c => c.Attendance.WorkDate)
                .NotEmpty().WithMessage("Work date is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Work date cannot be in the future.");

            RuleFor(c => c.Attendance.CheckInUtc)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Check-in time cannot be in the future.")
                .When(c => c.Attendance.CheckInUtc.HasValue);

            RuleFor(c => c.Attendance.CheckOutUtc)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Check-out time cannot be in the future.")
                .When(c => c.Attendance.CheckOutUtc.HasValue);

            RuleFor(c => c.Attendance)
                .Must(HaveValidTimeRange)
                .WithMessage("Check-out time must be after check-in time.")
                .When(c => c.Attendance.CheckInUtc.HasValue && c.Attendance.CheckOutUtc.HasValue);

            RuleFor(c => c.Attendance.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.")
                .When(c => !string.IsNullOrEmpty(c.Attendance.Notes));
        }

        public override ValidationResult Validate(ValidationContext<UpdateAttendanceCommand> context)
        {
            return context.InstanceToValidate.Attendance is null
                ? new ValidationResult(new[] { new ValidationFailure("AttendanceForUpdateDto",
                    "AttendanceForUpdateDto object is null") })
                : base.Validate(context);
        }

        private static bool HaveValidTimeRange(Shared.DataTransferObjects.AttendanceForUpdateDto attendance)
        {
            if (attendance.CheckInUtc.HasValue && attendance.CheckOutUtc.HasValue)
            {
                return attendance.CheckOutUtc.Value >= attendance.CheckInUtc.Value;
            }
            return true; // Valid if one or both are null
        }
    }
}