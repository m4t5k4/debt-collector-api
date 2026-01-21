using DebtCollector.Domain.Entities;
using MediatR;

namespace DebtCollector.Application.People.Queries.GetPeople
{
    public class GetPeopleQuery : IRequest<IEnumerable<Person>>
    {
    }
}
