using HorsesForCourses.Core;
public class CoachTests
{
    [Fact]
    public void Should_Add_Competence()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");

        coach.AddCompetence("C#");

        Assert.Contains("C#", coach.Competences);
    }

    [Fact]
    public void Should_Not_Add_Duplicate_Competence()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        coach.AddCompetence("C#");

        coach.AddCompetence("C#");

        Assert.Single(coach.Competences);
    }

    [Fact]
    public void Should_Add_Availability()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        var slot = new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));

        coach.AddAvailability(slot);

        Assert.Contains(slot, coach.Availabilities);
    }

    [Fact]
    public void CanTeach_Returns_False_When_Missing_Competence()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddRequiredCompetence("C#");
        course.AddTimeslot(new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.Confirm();

        Assert.False(coach.CanTeach(course));
    }

    [Fact]
    public void CanTeach_Returns_True_When_All_Requirements_Met()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        coach.AddCompetence("C#");
        var slot = new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        coach.AddAvailability(slot);

        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddRequiredCompetence("C#");
        course.AddTimeslot(slot);
        course.Confirm();

        Assert.True(coach.CanTeach(course));
    }

    [Fact]
    public void AssignCourse_Should_Add_Course_When_Eligible()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        coach.AddCompetence("C#");
        var slot = new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        coach.AddAvailability(slot);

        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddRequiredCompetence("C#");
        course.AddTimeslot(slot);
        course.Confirm();

        coach.AssignCourse(course);

        Assert.Contains(course.Id, coach.AssignedCourseIds);
    }

    [Fact]
    public void AssignCourse_Should_Throw_When_Not_Eligible()
    {
        var coach = new Coach("Milad Eshaghzey", "eshaghzey_milad@hotmail.com");
        var course = new Course("C# Basics", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddRequiredCompetence("C#");
        course.AddTimeslot(new Timeslot(Weekday.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.Confirm();

        Assert.Throws<InvalidOperationException>(() => coach.AssignCourse(course));
    }
}