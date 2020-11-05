using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;


namespace Adsbility.Appilication.Test.Commands
{
    public class CreateTestValidator : AbstractValidator<CreateTest>
    {
        public CreateTestValidator()
        {
            RuleFor(v => v.Name).NotEmpty().MaximumLength(5).WithMessage("custom message from createTestValidator");
        }
    }
}
