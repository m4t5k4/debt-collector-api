using DebtCollector.Application.DTOs;
using MediatR;

namespace DebtCollector.Application.People.Queries.GetPerson
{
    public class GetPersonQuery : IRequest<PersonDTO>
    {
        public int Id { get; set; }
    }
}
