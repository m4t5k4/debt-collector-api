using MediatR;

namespace DebtCollector.Application.People.Commands.SetCurrency
{
    public class SetCurrencyCommand : IRequest
    {
        public int PersonId { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
