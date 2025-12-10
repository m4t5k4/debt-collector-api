using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException("Invalid token");

            var order = await _context.Orders
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null) throw new KeyNotFoundException("Order not found");

            // Authorization? Original controller didn't check explicity but it's implied by access to Order ID which is semi-public?
            // Wait, original controller UpdateOrder checks:
            /*
             var personId = _AuthorizationService.GetCurrentPersonId();
             if (personId == null) return Unauthorized(new { message = "Invalid token" });
             var order = await _context.Orders...
            */
            // It doesn't check if user is in the group of the expense of the order.
            // But GetOrder(id) DOES check:
            /*
             var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == order.ExpenseId);
             var isPersonInGroup = ...
            */
            // Ideally Update should also check. I will add the check for safety as I did in GetOrder logic, 
            // OR stick to strict original fidelity.
            // Original code:
            // UpdateOrder: No group check explicitly.
            // But for DeleteOrder: No group check explicitly.
            // Check CreateOrder: Checks group.
            // Check GetOrder: Checks group.
            // It seems inconsistent. Create and Get check, Update/Delete don't?
            // This allows anyone to update/delete any order if they guess ID? That's a security hole.
            // I should PROBABLY fix it, but my mandate is refactoring. 
            // However, since I am touching it, I will add the check if it's easy.
            // But `UpdateOrder` doesn't load Expense to check GroupId.
            // I'll stick to original behavior to avoid regressions/side effects (maybe admins edit?).
            // Actually, better safe than sorry, but I will stick to exact original logic to guarantee "refactor only".

            // Update fields
            order.Name = request.OrderDto.Name;
            order.TotalCost = request.OrderDto.TotalCost;
            order.ModifiedOn = DateTime.UtcNow;
            order.ModifiedByPersonId = personId ?? 0;

            // Update Payers/Debtors
            // Remove existing
            _context.Payers.RemoveRange(order.Payers);
            _context.Debtors.RemoveRange(order.Debtors);
            
            // Add new
            order.Payers = request.OrderDto.Payers.Select(p => new Payer
            {
                Id = 0,
                PersonId = p.PersonId,
                OrderId = p.OrderId, // Should be order.Id or p.OrderId? Original used p.OrderId which assumes DTO has it right.
                Value = p.Value,
                CreatedByPersonId = personId ?? 0,
                CreatedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow
            }).ToList();

            order.Debtors = request.OrderDto.Debtors.Select(d => new Debtor
            {
                Id = 0,
                PersonId = d.PersonId,
                OrderId = d.OrderId,
                Value = d.Value,
                HasPaid = d.HasPaid,
                CreatedByPersonId = personId ?? 0,
                CreatedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow
            }).ToList();

            await _context.SaveChangesAsync(cancellationToken);

            // Recalculate
             var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == order.ExpenseId, cancellationToken);
            if (expense != null)
            {
                expense.TotalOrdersCost = await _context.Orders
                    .Where(o => o.ExpenseId == order.ExpenseId)
                    .SumAsync(o => o.TotalCost, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return order;
        }
    }
}
