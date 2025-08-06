using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class SessionPersistencyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>( )
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        var coachId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        using (var context = new AppDbContext(options))
        {
            var session = new Session(coachId, courseId, new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)));
            context.Sessions.Add(session);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var session = await context.Sessions.FirstOrDefaultAsync();
            Assert.NotNull(session);
            Assert.Equal(coachId, session!.CoachId);
            Assert.Equal(courseId, session.CourseId);
        }
    }
}