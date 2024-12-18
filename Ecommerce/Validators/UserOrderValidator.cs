using Ecommerce.DTO;
using FluentValidation;

namespace Ecommerce.Validators
{
    public class UserOrderValidator : AbstractValidator<OrderDTO>
    {
        public UserOrderValidator()
        {
            RuleFor(order=>order.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }
}
