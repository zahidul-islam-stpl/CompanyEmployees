using CompanyEmployees.Application.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace CompanyEmployees.Application.Validators
{
    public sealed class UpdateCheckOutCommandValidator : AbstractValidator<UpdateCheckOutCommand>
    {
        public UpdateCheckOutCommandValidator()
        {
            RuleFor(c => c.AttendanceId).NotEmpty().WithMessage("Attendance ID is required.");

            RuleFor(c => c.CheckOut.CheckOutUtc)
                .NotEmpty().WithMessage("Check-out time is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Check-out time cannot be in the future.");

            RuleFor(c => c.CheckOut.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.")
                .When(c => !string.IsNullOrEmpty(c.CheckOut.Notes));
        }

        public override ValidationResult Validate(ValidationContext<UpdateCheckOutCommand> context)
        {
            return context.InstanceToValidate.CheckOut is null
                ? new ValidationResult(new[] { new ValidationFailure("CheckOutDto",
                    "CheckOutDto object is null") })
                : base.Validate(context);
        }
    }
}