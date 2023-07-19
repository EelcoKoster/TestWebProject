using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Tests
{
    public class CompanyContactHanderTests
    {
        private CompanyContactHandler _companyContactHander;
        private DataContext _testContext;
        private string _dbPath;

        [SetUp]
        public void Setup()
        {
            _dbPath = @"c:\temp\testdb.db";
            var builder = new DbContextOptionsBuilder<DataContext>().UseSqlite($"Data Source={_dbPath}");
            _testContext = new DataContext(builder.Options);
            _testContext.Database.Migrate();
            _companyContactHander = new CompanyContactHandler(_testContext);
        }

        [TearDown]
        public async Task Teardown()
        {
            await _testContext.Database.EnsureDeletedAsync();
            await _testContext.DisposeAsync();
            var file = new FileInfo(_dbPath);
            file.Delete();
        }

        static Models.CompanyContact[] TestCompanies =
        {
            new Models.CompanyContact { Name = "Contact"},
            new Models.CompanyContact { Name = "Jan de Vries", Function = "Leidinggevende", Phone = "+31 (6) 10203040", EmailAddress = "jdv@gmail.com"},
            new Models.CompanyContact { Name = "Piet Boersma", Function = "Arbeidsdeskundige", Phone = "088-12345678", CompanyId = "4"},
            new Models.CompanyContact { Name = "Björn Borg", Function = "", Phone = "0900-1234"},
        };

        [TestCaseSource(nameof(TestCompanies))]
        public async Task TestCase_Can_Add_Company(Models.CompanyContact companyContact)
        {
            await _companyContactHander.Add(companyContact);
            var result = await _companyContactHander.Get("Id", true);
            result.Should().HaveCount(1);
            result.Should().Contain(_ => _.Name == companyContact.Name);
            result.Should().Contain(_ => _.Function == companyContact.Function);
            result.Should().Contain(_ => _.Phone == companyContact.Phone);
            result.Should().Contain(_ => _.EmailAddress == companyContact.EmailAddress);
        }

        static Models.CompanyContact[] DefaultTestCompanyContact =
        {
            new Models.CompanyContact { Id = "1", Name = "Jeroen", Function = "Leidinggevende", Phone = "0800-3040",  },
            new Models.CompanyContact { Id = "2", Name = "Karin", Function = "Arbeidsdeskundig", Phone = "1020-4444", },
            new Models.CompanyContact { Id = "7", Name = "René Hoekstra", Function = "", Phone = "+33 (6) 100 200", EmailAddress = ""},
        };

        static Models.CompanyContact[] UpdateTestCompanyContacts =
{
            new Models.CompanyContact { Id = "1", Name = "Jeroen van der Veen", Function = "Baas", Phone = "0800-3040" },
            new Models.CompanyContact { Id = "2", Name = "Carin", Function = "Arbeidsdeskundige",  EmailAddress = "Carin@gmail.com"},
            new Models.CompanyContact { Id = "7", Name = "René Hoekstra", Function = "HR Manager", Phone = "+33 (6) 100 201", CompanyId = "333" },
        };

        [TestCaseSource(nameof(UpdateTestCompanyContacts))]
        public async Task Test_Can_Update_Company(Models.CompanyContact updateCompanyContact)
        {
            foreach (var companyContact in DefaultTestCompanyContact)
            {
                await _companyContactHander.Add(companyContact);
            }
            await _companyContactHander.Update(updateCompanyContact);

            var result = await _companyContactHander.GetById(updateCompanyContact.Id);
            result.Should().BeEquivalentTo(updateCompanyContact);
        }

        [Test]
        public async Task Test_Can_Delete_Company()
        {
            foreach (var company in DefaultTestCompanyContact)
            {
                await _companyContactHander.Add(company);
            }
            await _companyContactHander.Delete(DefaultTestCompanyContact[1].Id);

            var result = await _companyContactHander.Get(null, true);
            result.Should().NotContain(_ => _.Id == DefaultTestCompanyContact[1].Id);
            result.Should().Contain(_ => _.Id == DefaultTestCompanyContact[0].Id);
            result.Should().Contain(_ => _.Id == DefaultTestCompanyContact[2].Id);
        }
    }
}