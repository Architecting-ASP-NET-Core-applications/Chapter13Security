namespace Chapter13Security.Models;

public class Configuration
{
    public Logging Logging { get; set; }
    public string MySettingKey { get; set; }
    public string AllowedHosts { get; set; }
}

public class Logging
{
    public Loglevel LogLevel { get; set; }
}

public class Loglevel
{
    public string Default { get; set; }
    public string MicrosoftAspNetCore { get; set; }
}
