using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetOrderByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var order = await _context.Orders
                .Include(o => o.Image)
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null) throw new KeyNotFoundException("Order not found");

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == order.ExpenseId, cancellationToken);
            if (expense == null) throw new KeyNotFoundException("Order is not part of an expense");

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId, cancellationToken);

            if (!isPersonInGroup) throw new UnauthorizedAccessException("Access denied");

            return order;
        }
    }
}
