namespace Models.ViewModels
{
    public sealed class ClientView
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DurationMonths { get; set; }
        public TrajectType TrajectType { get; set; }
        public ClientStatus Status { get; set; }
        public string IssuedBy { get; set; }
        public string? CompanyNumber { get; set; }
        public string CompanyName { get; set; }
    }
}
