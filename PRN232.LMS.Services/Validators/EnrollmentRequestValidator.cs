using FluentValidation;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Validators
{
    public class EnrollmentRequestValidator : AbstractValidator<EnrollmentRequest>
    {
        public EnrollmentRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("StudentId must be a positive number");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be a positive number");

            RuleFor(x => x.EnrollDate)
                .NotEmpty().WithMessage("EnrollDate is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("EnrollDate cannot be a future date");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(s => s == "Active" || s == "Inactive" || s == "Pending")
                .WithMessage("Status must be one of: Active, Inactive, Pending");
        }
    }
}
