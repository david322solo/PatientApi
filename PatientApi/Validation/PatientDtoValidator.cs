using FluentValidation;
using PatientApi.Models.Dto;

namespace PatientApi.Validation
{
    public class PatientDtoValidator : AbstractValidator<PatientDto>
    {
        public PatientDtoValidator()
        {
            RuleFor(x => x.Family).NotEmpty().MaximumLength(100);
            RuleFor(x => x.BirthDate).NotEmpty().LessThan(DateTime.Now);
            RuleFor(x => x.Use).IsInEnum();
            RuleFor(x => x.Gender).IsInEnum();
        }
    }
}