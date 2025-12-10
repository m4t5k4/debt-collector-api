using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Groups.Queries.GetGroupById
{
    public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GroupDTO> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException();

            var userIsInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.GroupId == request.Id && pg.PersonId == personId, cancellationToken);

            if (!userIsInGroup) throw new KeyNotFoundException("Group not found or access denied");

            var groupDto = await _context.Groups
                .Where(g => g.Id == request.Id)
                .Include(g => g.Image)
                .Include(g => g.Expenses)
                .Include(g => g.PersonGroups)
                    .ThenInclude(pg => pg.Person)
                        .ThenInclude(p => p.Image)
                .Select(g => new GroupDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Password = g.Password,
                    Image = g.Image,
                    CreatedOn = g.CreatedOn,
                    CreatedByPersonId = g.CreatedByPersonId,
                    ModifiedOn = g.ModifiedOn,
                    ModifiedByPersonId = g.ModifiedByPersonId,
                    Expenses = g.Expenses.Select(e => new ExpenseDTO
                    {
                        Id = e.Id,
                        GroupId = e.GroupId,
                        Name = e.Name,
                        Image = e.Image,
                        Currency = e.Currency,
                        TotalOrdersCost = e.TotalOrdersCost,
                        CreatedOn = e.CreatedOn,
                        CreatedByPersonId = e.CreatedByPersonId,
                        ModifiedOn = e.ModifiedOn,
                        ModifiedByPersonId = e.ModifiedByPersonId,
                        Orders = e.Orders.Select(o => new OrderDTO
                        {
                            Id = o.Id,
                            ExpenseId = o.ExpenseId,
                            Name = o.Name,
                            TotalCost = o.TotalCost,
                            CreatedOn = o.CreatedOn,
                            CreatedByPersonId = o.CreatedByPersonId,
                            ModifiedOn = o.ModifiedOn,
                            ModifiedByPersonId = o.ModifiedByPersonId,
                            Debtors = o.Debtors.Select(d => new DebtorDTO
                            {
                                Id = d.Id,
                                PersonId = d.PersonId,
                                OrderId = d.OrderId,
                                Value = d.Value,
                                HasPaid = d.HasPaid,
                                CreatedOn = d.CreatedOn,
                                CreatedByPersonId = d.CreatedByPersonId,
                                ModifiedOn = d.ModifiedOn,
                                ModifiedByPersonId = d.ModifiedByPersonId,
                            }).ToList(),
                            Payers = o.Payers.Select(p => new PayerDTO
                            {
                                Id = p.Id,
                                PersonId = p.PersonId,
                                OrderId = p.OrderId,
                                Value = p.Value,
                                CreatedOn = p.CreatedOn,
                                CreatedByPersonId = p.CreatedByPersonId,
                                ModifiedOn = p.ModifiedOn,
                                ModifiedByPersonId = p.ModifiedByPersonId,
                            }).ToList()
                        }).ToList(),
                    }).ToList(),
                    People = g.PersonGroups.Select(pg => new PersonDTO
                    {
                        Id = pg.Person.Id,
                        Username = pg.Person.Username,
                        Image = pg.Person.Image,
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (groupDto == null) throw new KeyNotFoundException("Group not found");

            return groupDto;
        }
    }
}
