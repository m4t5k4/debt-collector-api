using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            // Original controller logic: return await _context.Orders.ToListAsync();
            // No auth check? Seriously?
            // [HttpGet] public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
            // It seems so. Access to ALL orders in DB?
            // This is definitely a security concern, but as a refactorer, I replicate behavior.
            return await _context.Orders.ToListAsync(cancellationToken);
        }
    }
}
