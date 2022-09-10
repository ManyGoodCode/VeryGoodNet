using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// 注册用户表单的验证器
    /// </summary>
    public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
    {
        public RegisterFormModelFluentValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Length(min: 2, max: 100);
            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(maximumLength: 255)
                .EmailAddress();
            RuleFor(p => p.Password).NotEmpty().WithMessage(errorMessage: "Your password cannot be empty")
                      .MinimumLength(6).WithMessage(errorMessage: "Your password length must be at least 6.")
                      .MaximumLength(16).WithMessage(errorMessage: "Your password length must not exceed 16.")
                      .Matches(@"[A-Z]+").WithMessage(errorMessage: "Your password must contain at least one uppercase letter.")
                      .Matches(@"[a-z]+").WithMessage(errorMessage: "Your password must contain at least one lowercase letter.")
                      .Matches(@"[0-9]+").WithMessage(errorMessage: "Your password must contain at least one number.")
                      .Matches(@"[\!\?\*\.]+").WithMessage(errorMessage: "Your password must contain at least one (!? *.).");
            RuleFor(x => x.ConfirmPassword)
                 .Equal(x => x.Password);
            RuleFor(x => x.AgreeToTerms)
                .Equal(true);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<RegisterFormModel>
                .CreateWithOptions((RegisterFormModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

