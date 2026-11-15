using System.Text;
using System.Text.Json;
using SmartLibrary.Users;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
// Stabilna lokacija, nezavisna od direktorija iz kojeg se aplikacija pokreće.
var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "SmartLibrary", "users.json");
try
{
    new UserMenu(new UserService(new JsonUserRepository(dataPath))).Run();
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or UserValidationException)
{
    Console.Error.WriteLine($"Aplikacija nije pokrenuta: {ex.Message}");
    Console.Error.WriteLine($"Provjerite datoteku: {dataPath}. Podaci nisu automatski obrisani.");
    Environment.ExitCode = 1;
}
