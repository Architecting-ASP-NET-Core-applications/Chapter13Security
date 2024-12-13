using Microsoft.AspNetCore.DataProtection;

namespace Chapter13Security.Models;

public class Encript
{
    public void EncriptFile()
    {
        var provider = DataProtectionProvider.Create("MyApp");
        var protector = provider.CreateProtector("ConfigurationProtector");
        var plainText = File.ReadAllText("appsettings.json");
        var encryptedText = protector.Protect(plainText);
        File.WriteAllText("appsettings.encrypted.json", encryptedText);
    }
}
