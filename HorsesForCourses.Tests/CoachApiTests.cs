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
        // Arrange
        var newCoach = new
        {
            name = "Milad Eshaghzey",
            email = "eshaghzey_milad@hotmail.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/coaches", newCoach);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCoach = await response.Content.ReadFromJsonAsync<CoachDtoResponse>();
        Assert.NotNull(createdCoach);
        Assert.Equal("Milad Eshaghzey", createdCoach!.Name);
        Assert.Equal("eshaghzey_milad@hotmail.com", createdCoach.Email);
        Assert.NotEqual(Guid.Empty, createdCoach.Id);
    }

    [Fact]
    public async Task Should_Update_Coach_Skills()
    {
        // Arrange: maak eerst een coach aan
        var newCoach = new
        {
            name = "Milad Eshaghzey",
            email = "eshaghzey_milad@hotmail.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/coaches", newCoach);
        var createdCoach = await createResponse.Content.ReadFromJsonAsync<CoachDtoResponse>();
        Assert.NotNull(createdCoach);

        // Act: skills toevoegen
        var skillsUpdate = new
        {
            skills = new List<string> { "C#", "ASP.NET Core" }
        };

        var response = await _client.PostAsJsonAsync($"/coaches/{createdCoach!.Id}/skills", skillsUpdate);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedCoach = await response.Content.ReadFromJsonAsync<CoachDtoResponse>();
        Assert.NotNull(updatedCoach);
        Assert.Contains("C#", updatedCoach!.Competences);
        Assert.Contains("ASP.NET Core", updatedCoach.Competences);
    }

    private class CoachDtoResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Competences { get; set; } = new();
    }
}