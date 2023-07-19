using FluentValidation;

namespace Models
{
    public sealed class Client : BaseEntity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DurationMonths { get; set; }
        public int? DurationTalks { get; set; }
        public TrajectType TrajectType { get; set; }
        public ClientStatus Status { get; set; }
        public string? Description { get; set; }
        public string IssuedBy { get; set; }
        public string? Emailaddress { get; set; }
        public string? Phone { get; set; }
        public string? CompanyNumber { get; set; }
        public string CompanyContact { get; set; }
    }

    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(x => x.Lastname).NotNull();
            RuleFor(x => x.Emailaddress).EmailAddress().When(client => client.Emailaddress != "");
            RuleFor(x => x.StartDate).LessThan(DateTime.Today);
        }
    }
}
