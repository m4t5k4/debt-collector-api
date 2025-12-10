using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.People.Queries.GetPeople
{
    public class GetPeopleQueryHandler : IRequestHandler<GetPeopleQuery, IEnumerable<Person>>
    {
        private readonly IApplicationDbContext _context;

        public GetPeopleQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
        {
            // Original controller: return await _context.Persons.ToListAsync();
            // No authorization check
            return await _context.Persons.ToListAsync(cancellationToken);
        }
    }
}
