namespace Models
{
    public sealed class CompanyContact : BaseEntity
    {
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Function { get; set; }

    }
}
