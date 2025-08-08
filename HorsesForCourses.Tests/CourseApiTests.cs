using System.Net;
using System.Net.Http.Json;

public class CourseApiTests
{
    private static readonly CustomWebAppFactory _factory = new();
    private readonly HttpClient _client;

    public CourseApiTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_Course()
    {
        var newCourse = new
        {
            name = "C# Basics",
            startDate = DateOnly.FromDateTime(DateTime.Today),
            endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };

        var response = await _client.PostAsJsonAsync("/courses", newCourse);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCourse = await response.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(createdCourse);
        Assert.Equal("C# Basics", createdCourse!.Name);
        Assert.Equal(newCourse.startDate, createdCourse.StartDate);
        Assert.Equal(newCourse.endDate, createdCourse.EndDate);
    }

    [Fact]
    public async Task Should_Confirm_Course_When_Valid()
    {
        var newCourse = new
        {
            name = "C# Masterclass",
            startDate = DateOnly.FromDateTime(DateTime.Today),
            endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };

        var createResponse = await _client.PostAsJsonAsync("/courses", newCourse);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdCourse = await createResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(createdCourse);

        var timeslot = new
        {
            day = Weekday.Monday,
            start = new TimeOnly(9, 0),
            end = new TimeOnly(10, 30)
        };

        var tsResponse = await _client.PostAsJsonAsync($"/courses/{createdCourse!.Id}/timeslots", timeslot);
        Assert.Equal(HttpStatusCode.OK, tsResponse.StatusCode);

        var confirmResponse = await _client.PostAsync($"/courses/{createdCourse.Id}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        var confirmedCourse = await confirmResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(confirmedCourse);
        Assert.Equal(CourseStatus.PendingForCoach, confirmedCourse!.Status);
    }

    [Fact]
    public async Task Should_Assign_Coach_When_Eligible()
    {
        var newCoach = new
        {
            name = "Milad Eshaghzey",
            email = "eshaghzey_milad@hotmail.com"
        };

        var coachResponse = await _client.PostAsJsonAsync("/coaches", newCoach);
        Assert.Equal(HttpStatusCode.Created, coachResponse.StatusCode);

        var createdCoach = await coachResponse.Content.ReadFromJsonAsync<CoachResponse>();
        Assert.NotNull(createdCoach);

        var newCourse = new
        {
            name = "C# Advanced",
            startDate = DateOnly.FromDateTime(DateTime.Today),
            endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };

        var courseResponse = await _client.PostAsJsonAsync("/courses", newCourse);
        Assert.Equal(HttpStatusCode.Created, courseResponse.StatusCode);

        var createdCourse = await courseResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(createdCourse);

        var timeslot = new
        {
            day = Weekday.Monday,
            start = new TimeOnly(9, 0),
            end = new TimeOnly(10, 30)
        };

        var tsResponse = await _client.PostAsJsonAsync($"/courses/{createdCourse!.Id}/timeslots", timeslot);
        Assert.Equal(HttpStatusCode.OK, tsResponse.StatusCode);

        var updateSkills = new { skills = new List<string> { "C#" } };
        var skillsResponse = await _client.PostAsJsonAsync($"/courses/{createdCourse.Id}/skills", updateSkills);
        Assert.Equal(HttpStatusCode.OK, skillsResponse.StatusCode);

        var coachSkills = new { skills = new List<string> { "C#" } };
        var coachSkillsResponse = await _client.PostAsJsonAsync($"/coaches/{createdCoach!.Id}/skills", coachSkills);
        Assert.Equal(HttpStatusCode.OK, coachSkillsResponse.StatusCode);

        var availability = new
        {
            day = Weekday.Monday,
            start = new TimeOnly(9, 0),
            end = new TimeOnly(10, 30)
        };
        var availabilityResponse = await _client.PostAsJsonAsync($"/coaches/{createdCoach.Id}/availability", availability);
        Assert.Equal(HttpStatusCode.OK, availabilityResponse.StatusCode);

        var confirmResponse = await _client.PostAsync($"/courses/{createdCourse.Id}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        var assignCoach = new { coachId = createdCoach.Id };
        var assignResponse = await _client.PostAsJsonAsync($"/courses/{createdCourse.Id}/assign-coach", assignCoach);
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

        var assignedCourse = await assignResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(assignedCourse);
        Assert.Equal(CourseStatus.Confirmed, assignedCourse!.Status);
    }

    private class CourseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public CourseStatus Status { get; set; }
        public List<string> RequiredCompetences { get; set; } = new();
        public List<TimeslotResponse> Timeslots { get; set; } = new();
    }

    private class TimeslotResponse
    {
        public Weekday Day { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }

    private class CoachResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Competences { get; set; } = new();
    }
    [Fact]
    public async Task CoachCanAssignSelfToEligibleCourse()
    {
        var coach = new { name = "Milad", email = "eshaghzey_milad@hotmail.com" };
        var coachResp = await _client.PostAsJsonAsync("/coaches", coach);
        var createdCoach = await coachResp.Content.ReadFromJsonAsync<CoachResponse>();

        var course = new
        {
            name = "C# OOP",
            startDate = DateOnly.FromDateTime(DateTime.Today),
            endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        };
        var courseResp = await _client.PostAsJsonAsync("/courses", course);
        var createdCourse = await courseResp.Content.ReadFromJsonAsync<CourseResponse>();

        var timeslot = new { day = Weekday.Tuesday, start = new TimeOnly(9, 0), end = new TimeOnly(12, 0) };
        await _client.PostAsJsonAsync($"/courses/{createdCourse!.Id}/timeslots", timeslot);

        var courseSkills = new { skills = new List<string> { "C#" } };
        await _client.PostAsJsonAsync($"/courses/{createdCourse.Id}/skills", courseSkills);

        var coachSkills = new { skills = new List<string> { "C#" } };
        await _client.PostAsJsonAsync($"/coaches/{createdCoach!.Id}/skills", coachSkills);

        var availability = new { day = Weekday.Tuesday, start = new TimeOnly(9, 0), end = new TimeOnly(12, 0) };
        await _client.PostAsJsonAsync($"/coaches/{createdCoach.Id}/availability", availability);

        await _client.PostAsync($"/courses/{createdCourse.Id}/confirm", null);

        var assignDto = new { courseId = createdCourse.Id };
        var assignResp = await _client.PostAsJsonAsync($"/coaches/{createdCoach.Id}/assign", assignDto);

        Assert.Equal(HttpStatusCode.OK, assignResp.StatusCode);
        var assignedCourse = await assignResp.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.Equal(CourseStatus.Confirmed, assignedCourse!.Status);
    }
    [Fact]
    public async Task CoachCannotAssignSelfToIneligibleCourse()
    {
        var coach = new { name = "Inelegible Coach", email = "inelegible@example.com" };
        var coachResp = await _client.PostAsJsonAsync("/coaches", coach);
        var createdCoach = await coachResp.Content.ReadFromJsonAsync<CoachResponse>();

        var course = new
        {
            name = "Java Fundamentals",
            startDate = DateOnly.FromDateTime(DateTime.Today),
            endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        };
        var courseResp = await _client.PostAsJsonAsync("/courses", course);
        var createdCourse = await courseResp.Content.ReadFromJsonAsync<CourseResponse>();

        var timeslot = new { day = Weekday.Wednesday, start = new TimeOnly(14, 0), end = new TimeOnly(16, 0) };
        await _client.PostAsJsonAsync($"/courses/{createdCourse!.Id}/timeslots", timeslot);

        var courseSkills = new { skills = new List<string> { "Java" } };
        await _client.PostAsJsonAsync($"/courses/{createdCourse.Id}/skills", courseSkills);

        await _client.PostAsync($"/courses/{createdCourse.Id}/confirm", null);

        var assignDto = new { courseId = createdCourse.Id };
        var assignResp = await _client.PostAsJsonAsync($"/coaches/{createdCoach!.Id}/assign", assignDto);

        Assert.Equal(HttpStatusCode.BadRequest, assignResp.StatusCode);
    }
}