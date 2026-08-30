using System.Text.Json.Serialization;

namespace PBug.Controllers;

public class PushEvent
{
    [JsonPropertyName("commits")]
    public required List<PushEventCommit> Commits { get; set; }
    
    [JsonPropertyName("repository")]
    public required PushEventRepository Repository { get; set; }
    
    [JsonPropertyName("pusher")]
    public required PushEventUser Pusher { get; set; }
}

public class PushEventRepository
{
    [JsonPropertyName("full_name")]
    public required string FullName { get; set; }
    
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; }
}

public class PushEventUser
{
    [JsonPropertyName("login")]
    public required string Login { get; set; }
    
    [JsonPropertyName("full_name")]
    public required string FullName { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; } 
    
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; } 
}

public class PushEventCommit
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("message")]
    public required string Message { get; set; }
    
    [JsonPropertyName("url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("author")]
    public required PushEventCommitAuthor Author { get; set; }
}

public class PushEventCommitAuthor
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}
