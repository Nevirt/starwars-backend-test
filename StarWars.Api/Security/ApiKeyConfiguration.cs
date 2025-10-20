namespace StarWars.Api.Security;

public class ApiKeyConfiguration
{
    public string Header { get; set; } = "ApiKey";
    public string Realm { get; set; } = "StarWars";
    public string Key { get; set; } = string.Empty;
}


