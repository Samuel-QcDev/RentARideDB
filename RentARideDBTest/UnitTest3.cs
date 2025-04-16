using Xunit;
using SQLite;
using RentARideDB.Models;
using RentARideDB.Services;
using System;
using System.Threading.Tasks;

namespace RentARideDBTest
{
    public class SetWelcomeMessageTests
    {
        private readonly SQLiteAsyncConnection _dbConnection;
        private readonly ApplicationDbContext _dbContext;

        public SetWelcomeMessageTests()
        {
            _dbConnection = new SQLiteAsyncConnection(":memory:");
            _dbConnection.CreateTableAsync<Membre>().Wait();
            _dbConnection.CreateTableAsync<Session>().Wait();

            _dbContext = new ApplicationDbContext(_dbConnection);
        }

        [Fact]
        public async Task SetWelcomeMessageAsync_NoActiveSession_ReturnsBonjourInvite()
        {
            // Act
            var result = await _dbContext.SetWelcomeMessageAsync();

            // Assert
            Assert.True(result);
            Assert.Equal("Bonjour Invité", _dbContext.WelcomeMessage);
        }

        [Fact]
        public async Task SetWelcomeMessageAsync_ActiveSessionWithNoFirstName_ReturnsBienvenue()
        {
            // Arrange
            var member = new Membre { MemberID = 1, FirstName = null };
            var session = new Session { MemberID = 1, IsActive = true };

            await _dbConnection.InsertAsync(member);
            await _dbConnection.InsertAsync(session);

            // Act
            var result = await _dbContext.SetWelcomeMessageAsync();

            // Assert
            Assert.True(result);
            Assert.Equal("Bienvenue", _dbContext.WelcomeMessage);
        }

        [Fact]
        public async Task SetWelcomeMessageAsync_WhenActiveSessionExists_SetsWelcomeMessageToMemberName()
        {
            // Arrange
            var connection = new SQLiteAsyncConnection(":memory:");
            await connection.CreateTableAsync<Membre>();
            await connection.CreateTableAsync<Session>();

            var dbContext = new ApplicationDbContext(connection);

            var membre = new Membre
            {
                MemberID = 1,
                FirstName = "Samuel"
            };
            await connection.InsertAsync(membre);

            var session = new Session
            {
                SessionID = 1,
                MemberID = 1,
                IsActive = true
            };
            await connection.InsertAsync(session);

            // Act
            var result = await dbContext.SetWelcomeMessageAsync();

            // Assert
            Assert.True(result);
            Assert.Equal("Bonjour Samuel", dbContext.WelcomeMessage);
        }
    }
}

