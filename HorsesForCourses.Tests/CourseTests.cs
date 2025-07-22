namespace HorsesForCourses.Tests;
public class CourseTests
{
    [Fact]
    public void Should_Add_Timeslot_When_PendingForTimeslots()
    {
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var slot = new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));

        course.AddTimeslot(slot);

        Assert.Contains(slot, course.Timeslots);
    }

    [Fact]
    public void Should_Throw_When_Adding_Timeslot_After_Confirmation()
    {
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddTimeslot(new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.Confirm();

        Assert.Throws<InvalidOperationException>(() => course.AddTimeslot(new Timeslot(Weekday.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)))
        );
    }

    [Fact]
    public void Confirm_Should_Throw_When_No_Timeslots()
    {
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        Assert.Throws<InvalidOperationException>(() => course.Confirm());
    }

    [Fact]
    public void Confirm_Should_Throw_When_TotalDuration_Under_OneHour()
    {
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddTimeslot(new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(9, 30)));

        Assert.Throws<InvalidOperationException>(() => course.Confirm());
    }

    [Fact]
    public void Confirm_Should_Set_Status_To_PendingForCoach_When_Valid()
    {
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddTimeslot(new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0)));

        course.Confirm();

        Assert.Equal(CourseStatus.PendingForCoach, course.Status);
    }

    [Fact]
    public void AssignCoach_Should_Set_Status_To_Confirmed_When_Valid()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        coach.AddCompetence("C#");
        var slot = new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        coach.AddAvailability(slot);

        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddRequiredCompetence("C#");
        course.AddTimeslot(slot);
        course.Confirm();

        course.AssignCoach(coach);

        Assert.Equal(CourseStatus.Confirmed, course.Status);
        Assert.Equal(coach, course.AssignedCoach);
    }

    [Fact]
    public void AssignCoach_Should_Throw_When_Not_Confirmed()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");

        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
    }
}