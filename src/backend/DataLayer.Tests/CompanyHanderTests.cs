using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Tests
{
    public class CompanyHandlerTests
    {
        private CompanyHandler _companyHander;
        private DataContext _testContext;
        private string _dbPath;

        [SetUp]
        public void Setup()
        {
            _dbPath = @"c:\temp\testdb.db";
            var builder = new DbContextOptionsBuilder<DataContext>().UseSqlite($"Data Source={_dbPath}");
            _testContext = new DataContext(builder.Options);
            _testContext.Database.Migrate();
            _companyHander = new CompanyHandler(_testContext);
        }

        [TearDown]
        public async Task Teardown()
        {
            await _testContext.Database.EnsureDeletedAsync();
            await _testContext.DisposeAsync();
            var file = new FileInfo(_dbPath);
            file.Delete();
        }

        static Models.Company[] TestCompanies =
        {
            new Models.Company { Name = "Company"},
            new Models.Company { Name = "Vooruitzicht", City = "Drachten", Phone = "+31 (6) 10203040", PostalCode = "8926 AB", Street = "Stationstraat"},
            new Models.Company { Name = "PGGM", City = "Zeist", Phone = "088-12345678", PostalCode = "1020 ZZ", Street = "Pensioenweg 5"},
            new Models.Company { Name = "Björn Borg", City = "Los Sánchez", Phone = "0900-1234", PostalCode = "90210", Street = "Street"},
        };

        [TestCaseSource(nameof(TestCompanies))]
        public async Task TestCase_Can_Add_Company(Models.Company company)
        {
            await _companyHander.Add(company);
            var result = _companyHander.Get("Id", true);
            result.Should().HaveCount(1);
            result.First().Should().Be(company);
        }

        static Models.Company[] DefaultTestCompanies =
        {
            new Models.Company { Id = "1", Name = "Nike", City = "Parijs", Phone = "0800-3040", PostalCode = "12345", },
            new Models.Company { Id = "2", Name = "Adidass", City = "Londen", Phone = "1020-4444", PostalCode = "54321", Street = "Downing Street 8"},
            new Models.Company { Id = "3", Name = "Puma", City = "Rome", Phone = "+33 (6) 100 200", PostalCode = "123123", Street = "Straat"},
        };

        static Models.Company[] UpdateTestCompanies =
{
            new Models.Company { Id = "1", Name = "Nike", City = "Paris", Phone = "0800-3040", PostalCode = "12345", Street = "Nike place 1" },
            new Models.Company { Id = "2", Name = "Adidas", City = "London", PostalCode = "54321", Street = "Downing Street 1"},
            new Models.Company { Id = "3", Name = "Puma", City = "Roma", Phone = "+33 (6) 100 201", PostalCode = "333" },
        };

        [TestCaseSource(nameof(UpdateTestCompanies))]
        public async Task Test_Can_Update_Company(Models.Company updateCompany)
        {
            foreach (var company in DefaultTestCompanies)
            {
                await _companyHander.Add(company);
            }
            await _companyHander.Update(updateCompany);

            var result = await _companyHander.GetById(updateCompany.Id);
            result.Should().BeEquivalentTo(updateCompany);
        }

        [Test]
        public async Task Test_Can_Delete_Company()
        {
            foreach (var company in DefaultTestCompanies)
            {
                await _companyHander.Add(company);
            }
            await _companyHander.Delete(DefaultTestCompanies[1].Id);

            var result = _companyHander.Get(null, true);
            result.Should().NotContainEquivalentOf(DefaultTestCompanies[1]);
            result.Should().ContainEquivalentOf(DefaultTestCompanies[0]);
            result.Should().ContainEquivalentOf(DefaultTestCompanies[2]);
        }
    }
}