using System.Threading.Tasks;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
public class CoachPersistencyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new AppDbContext(options))
        {
            context.Coaches.Add(new Coach("naam", "em@il"));
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches.FirstOrDefaultAsync();
            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
            Assert.Equal("em@il", coach.Email);
        }
    }
}