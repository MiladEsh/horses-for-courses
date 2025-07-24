using System.Net;
using System.Net.Http.Json;


public class CoachApiTests
{
    private readonly HttpClient _client;

    public CoachApiTests()
    {
        var factory = new CustomWebAppFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_Coach()
    {
        var newCoach = new
        {
            name = "Milad Eshaghzey",
            email = "eshaghzey_milad@hotmail.com"
        };

        var response = await _client.PostAsJsonAsync("/coaches", newCoach);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCoach = await response.Content.ReadFromJsonAsync<CoachResponse>();
        Assert.Equal("Milad Eshaghzey", createdCoach!.Name);
        Assert.Equal("eshaghzey_milad@hotmail.com", createdCoach.Email);
    }

    [Fact]
    public async Task Should_Update_Coach_Skills()
    {
        var newCoach = new
        {
            name = "Milad Eshaghzey",
            email = "eshaghzey_milad@hotmail.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/coaches", newCoach);
        var createdCoach = await createResponse.Content.ReadFromJsonAsync<CoachResponse>();

        var skillsUpdate = new
        {
            skills = new List<string> { "C#", "ASP.NET Core" }
        };

        var response = await _client.PostAsJsonAsync($"/coaches/{createdCoach!.Id}/skills", skillsUpdate);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedCoach = await response.Content.ReadFromJsonAsync<CoachResponse>();
        Assert.Contains("C#", updatedCoach!.Competences);
        Assert.Contains("ASP.NET Core", updatedCoach.Competences);
    }

    private class CoachResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Competences { get; set; } = new();
    }
}