using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// 注册用户表单模型验证�?
    /// </summary>
    public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
    {
        public RegisterFormModelFluentValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Length(2, 100);
            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(255)
                .EmailAddress();
            RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                      .MinimumLength(6).WithMessage("Your password length must be at least 6.")
                      .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                      .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                      .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                      .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                      .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x => x.ConfirmPassword)
                 .Equal(x => x.Password);
            RuleFor(x => x.AgreeToTerms)
                .Equal(true);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<RegisterFormModel>.CreateWithOptions((RegisterFormModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

