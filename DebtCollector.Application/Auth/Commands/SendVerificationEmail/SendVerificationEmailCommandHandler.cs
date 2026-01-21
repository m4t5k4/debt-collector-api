using DebtCollector.Application.Common.Interfaces;
using MediatR;
using System.IO;

namespace DebtCollector.Application.Auth.Commands.SendVerificationEmail
{
    public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public SendVerificationEmailCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEmailService emailService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {
            var personId = _currentUserService.GetCurrentPersonId();
            if (personId == null) throw new UnauthorizedAccessException();

            var person = await _context.Persons.FindAsync(new object[] { personId }, cancellationToken);
            if (person == null || string.IsNullOrEmpty(person.Email))
                throw new ArgumentException("Invalid user or email.");

            var code = GenerateVerificationCode();

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "VerificationEmail.html");
            string body;
            
            if (File.Exists(templatePath))
            {
                body = await File.ReadAllTextAsync(templatePath, cancellationToken);
                body = body.Replace("{{CODE}}", code);
            }
            else
            {
                // Fallback if template is missing
                body = $"Your verification code is: {code}";
            }

            var subject = "Jouw verificatiecode";

            await _emailService.SendEmailAsync(person.Email, subject, body);

            person.EmailVerificationToken = code;
            await _context.SaveChangesAsync(cancellationToken);
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
