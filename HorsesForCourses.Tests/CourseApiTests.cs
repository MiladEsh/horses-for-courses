using System.Net;
using System.Net.Http.Json;
using HorsesForCourses.Core;

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
}