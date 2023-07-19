using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Tests
{
    public class ClientHanderTests
    {
        private ClientHandler _clientHander;
        private DataContext _testContext;
        private string _dbPath;

        [SetUp]
        public void Setup()
        {
            _dbPath = @"c:\temp\testdb.db";
            var builder = new DbContextOptionsBuilder<DataContext>().UseSqlite($"Data Source={_dbPath}");
            _testContext = new DataContext(builder.Options);
            _testContext.Database.Migrate();
            _clientHander = new ClientHandler(_testContext);
        }

        [TearDown]
        public async Task Teardown()
        {
            await _testContext.Database.EnsureDeletedAsync();
            await _testContext.DisposeAsync();
            var file = new FileInfo(_dbPath);
            file.Delete();
        }

        static Models.Client[] TestClients =
        {
            new Models.Client{ Firstname = "Test" },
            new Models.Client{ Firstname = "Test", Lastname = "von Tester"},
            new Models.Client{
                 Firstname = "Test",
                 Lastname = "von Tester",
                 DurationMonths = 1,
                 StartDate = new DateTime(2021,5,23),
                 EndDate = new DateTime(2021,11,25),
                 Emailaddress = "test@test.com",
                 IssuedBy = "IssuedBy1",
                 Phone = "+31 (0)6 123 456 78",
                 Status = Models.ClientStatus.Active,
                 TrajectType = Models.TrajectType.Standard
             },
            new Models.Client{ Firstname = "Björn", Lastname = "Sánchez", Description = "éëíïüäáêè" },
        };

        [TestCaseSource(nameof(TestClients))]
        public async Task TestCase_Can_Add_Client(Models.Client client)
        {
            await _clientHander.Add(client);
            object result = await _clientHander.Get(null, null);
            Assert.NotNull(result);

            //result.Should().HaveCount(1);
            //result[0].Should().Be(client);
        }

        static Models.Client[] DefaultTestClients =
{
            new Models.Client{ Id = "1", Firstname = "Lucie", Lastname = "van het Oosten", TrajectType = Models.TrajectType.Premium },
            new Models.Client{ Id = "2", Firstname = "Test", Lastname = "von Tester", StartDate = DateTime.Now },
            new Models.Client{ Id = "3", Firstname = "Björn", Lastname = "Sánchez", Status = Models.ClientStatus.Onhold },
        };

        static Models.Client[] UpdateTestClients =
{
            new Models.Client{ Id = "1", Firstname = "Lucie-Josèphe-François-Lucienne", Lastname = "van ’t Oosten", TrajectType = Models.TrajectType.Free },
            new Models.Client{ Id = "2", Firstname = "Kees", Lastname = "van Teststraat", EndDate = DateTime.Now.AddDays(20) },
            new Models.Client{ Id = "3", Firstname = "Björn", Lastname = "Sánchez", Status = Models.ClientStatus.Done },
        };

        [TestCaseSource(nameof(UpdateTestClients))]
        public async Task Test_Can_Update_Client(Models.Client updateClient)
        {
            foreach (var client in DefaultTestClients)
            {
                await _clientHander.Add(client);
            }
            await _clientHander.Update(updateClient);

            var result = await _clientHander.GetById(updateClient.Id);
            result.Should().BeEquivalentTo(updateClient);
        }

        [Test]
        public async Task Test_Can_Delete_Client()
        {
            foreach (var client in DefaultTestClients)
            {
                await _clientHander.Add(client);
            }
            await _clientHander.Delete(DefaultTestClients[1].Id);

            var result = await _clientHander.Get(null, true);
            result.Should().NotContain(_ => _.Id == DefaultTestClients[1].Id);
            result.Should().Contain(_ => _.Id == DefaultTestClients[0].Id);
            result.Should().Contain(_ => _.Id == DefaultTestClients[2].Id);
        }
    }
}