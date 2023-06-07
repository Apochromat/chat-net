using System.Net.Http.Json;
using System.Text.Json;
using ChatNet.Common.DataTransferObjects;
using Microsoft.AspNetCore.SignalR.Client;

const string localNotificationHubUrl = "http://localhost:5253/api/notification/hub";
const string localLoginUrl = "http://localhost:5253/api/auth/login";

const string productionNotificationHubUrl = "http://chat.markridge.space/api/notification/hub";
const string productionLoginUrl = "http://chat.markridge.space/api/auth/login";

var loginCredentials = new Dictionary<string, string?>() {
    { "email", "user@example.com" },
    { "password", "P@ssw0rd" }
};

var notificationHubUrl = localNotificationHubUrl;
var loginUrl = localLoginUrl;

Console.WriteLine("Do you want to run in production mode? (y/n)");
var mode = Console.ReadKey().Key;
while (mode != ConsoleKey.N) {
    Console.WriteLine();
    if (mode != ConsoleKey.Y) {
        Console.WriteLine("Do you want to run in production mode? (y/n)");
        mode = Console.ReadKey().Key;
        continue;
    }
    
    notificationHubUrl = productionNotificationHubUrl;
    loginUrl = productionLoginUrl;
    break;
}

Console.WriteLine("Do you want to login with your credentials? (y/n)");
var key = Console.ReadKey().Key;
while (key != ConsoleKey.N) {
    Console.WriteLine();
    if (key != ConsoleKey.Y) {
        Console.WriteLine("Do you want to login with your credentials? (y/n)");
        key = Console.ReadKey().Key;
        continue;
    }
    Console.WriteLine();
    Console.Write("email >> ");
    loginCredentials["email"] = Console.ReadLine();
    Console.Write("password >> ");
    loginCredentials["password"] = Console.ReadLine();
}

Console.WriteLine();

var connection = new HubConnectionBuilder()
    .WithUrl(notificationHubUrl, options => { options.AccessTokenProvider = Login; })
    .WithAutomaticReconnect()
    .Build();

void Print(string message) {
    var obj = JsonSerializer.Deserialize<NotificationMessageDto>(message);
    if (obj == null) {
        Console.WriteLine("Error deserializing message");
        return;
    }

    Console.WriteLine();

    var type = obj.GetType();
    var properties = type.GetProperties();
    
    string separator = new string('-', properties.Select(x=>x.ToString()).MaxBy(x=>x?.Length)?.Length ?? 0 + 2);
    Console.WriteLine("New Message:");
    Console.WriteLine(separator);

    foreach (var property in properties) {
        var propertyName = property.Name;
        var propertyValue = (property.GetValue(obj) ?? "null").ToString();

        Console.WriteLine($"{propertyName, -12}:\t{propertyValue}");
    }
    Console.WriteLine(separator);
}

connection.On("ReceiveMessage", (Action<string>)Print);

async Task<string?> Login() {
    var client = new HttpClient();
    try {
        var loginResult = await client.PostAsJsonAsync(loginUrl, loginCredentials);
        if (loginResult.IsSuccessStatusCode) {
            var content = await loginResult.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(content)?["accessToken"];
            return token;
        }

        Console.WriteLine("Error logging in");
    }
    catch (Exception) {
        Console.WriteLine("Error logging in");
        return null;
    }

    return null;
}

try {
    await connection.StartAsync();
}
catch (Exception e) {
    Console.WriteLine(e.Message);
    return;
}

Console.WriteLine(connection.State == HubConnectionState.Connected
    ? "Connected to notification hub"
    : "Error connecting to notification hub");

while (true) {
    var _ = Console.ReadLine();
}