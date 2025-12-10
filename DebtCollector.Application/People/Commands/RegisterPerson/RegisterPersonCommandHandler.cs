using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DebtCollector.Application.People.Commands.RegisterPerson
{
    public class RegisterPersonCommandHandler : IRequestHandler<RegisterPersonCommand, Person>
    {
        private readonly IApplicationDbContext _context;

        public RegisterPersonCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> Handle(RegisterPersonCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            var existingPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Email.ToLower() == request.Email.ToLower(), cancellationToken);

            if (existingPerson != null)
                throw new InvalidOperationException("Email is already taken.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            var person = new Person
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Username = request.Username
            };

            // Assign dummy avatar (ID 1)
            var dummyAvatar = await _context.Images.FirstOrDefaultAsync(i => i.Id == 1, cancellationToken);
            if (dummyAvatar != null)
            {
                person.ImageId = dummyAvatar.Id;
                person.Image = dummyAvatar;
            }

            _context.Persons.Add(person);
            await _context.SaveChangesAsync(cancellationToken);

            return person;
        }
    }
}
