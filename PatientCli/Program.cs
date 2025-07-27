using Bogus;
using PatientCli.Models.Dto;
using PatientCli.Models.Enums;
using System.Net.Http.Json;
using System.Net;

class Program
{
    static async Task WaitForApiAsync(string apiUrl, int timeoutSeconds = 60)
    {
        var client = new HttpClient();
        var start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < timeoutSeconds)
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Ошибка при проверке доступности API: {ex.Message}");
            }
            await Task.Delay(2000);
            Console.WriteLine("Ожидание запуска patientapi...");
        }
        throw new Exception("patientapi не запустился вовремя.");
    }

    static async Task Main(string[] args)
    {
        int count = args.Length > 0 && int.TryParse(args[0], out var c) ? c : 10;
        string apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000/api/patients";

        await WaitForApiAsync(apiUrl.Replace("/api/patient", "/swagger"), 60);

        var faker = new Bogus.Faker<PatientDto>()
            .RuleFor(p => p.Use, f => f.PickRandom<UseTypeEnum>())
            .RuleFor(p => p.Family, f => f.Name.LastName())
            .RuleFor(p => p.Given, f => new List<string> { f.Name.FirstName() })
            .RuleFor(p => p.Gender, f => f.PickRandom<GenderTypeEnum>())
            .RuleFor(p => p.BirthDate, f => f.Date.Past(40, DateTime.Today.AddYears(-18)))
            .RuleFor(p => p.Active, f => f.Random.Bool());

        using var http = new HttpClient();

        for (int i = 0; i < count; i++)
        {
            var patient = faker.Generate();
            var response = await http.PostAsJsonAsync(apiUrl, patient);
            Console.WriteLine($"[{i + 1}/{count}] {(response.IsSuccessStatusCode ? "OK" : $"Ошибка: {response.StatusCode}")}");
        }

        Console.WriteLine($"Добавлено {count} записей.");
    }
}
