﻿namespace Models
{
    public sealed class Company : BaseEntity
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
}
