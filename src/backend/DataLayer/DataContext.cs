using Microsoft.EntityFrameworkCore;
using Models;

namespace DataLayer
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "data.db");
            options.UseSqlite($"Data Source={dbPath}"); 
        }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
            Console.WriteLine("Init Db Context");
        }

    }
}