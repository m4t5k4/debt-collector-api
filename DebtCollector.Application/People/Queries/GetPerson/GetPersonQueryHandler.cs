using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.People.Queries.GetPerson
{
    public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetPersonQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PersonDTO> Handle(GetPersonQuery request, CancellationToken cancellationToken)
        {
            var currentPersonId = _currentUserService.GetCurrentPersonId();

            if (currentPersonId == null || currentPersonId != request.Id)
                throw new UnauthorizedAccessException("Invalid token");

            var person = await _context.Persons
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person == null) throw new KeyNotFoundException("Person not found");

            return new PersonDTO
            {
                Id = person.Id,
                Username = person.Username,
                Image = person.Image,
                PrimaryCurrency = person.PrimaryCurrency,
                IsVerified = person.IsVerified
            };
        }
    }
}
