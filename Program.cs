using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using wt_lab6_6minimalapi_yushkevich.Data;
using wt_lab6_6minimalapi_yushkevich.Models;

var builder = WebApplication.CreateBuilder(args);

// Описание API в формате OpenAPI (документ формируется автоматически по endpoint'ам).
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "CargoFlow API",
            Version = "v1",
            Description = "REST API для учёта грузов на складе. " +
                          "ASP.NET Core Minimal API, данные хранятся в памяти приложения."
        };

        document.Tags = new List<OpenApiTag>
        {
            new() { Name = "General", Description = "Служебные endpoint'ы приложения" },
            new() { Name = "Cargo", Description = "Операции с грузами на складе" }
        };

        // Относительный адрес сервера позволяет Swagger UI и Scalar обращаться к API
        // по тому же протоколу и хосту, на котором открыта документация.
        // Это важно на Render: снаружи используется HTTPS, а внутри контейнера — HTTP.
        document.Servers = new List<OpenApiServer>
        {
            new() { Url = "/" }
        };

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// OpenAPI и интерактивная документация Scalar доступны в любом окружении,
// чтобы описание API можно было проверить и на опубликованном сервисе Render.
app.MapOpenApi();
app.MapScalarApiReference(options => options.WithTitle("CargoFlow API — документация"));

// Классический Swagger UI работает поверх того же OpenAPI-документа.
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/openapi/v1.json", "CargoFlow API v1");
    options.DocumentTitle = "CargoFlow API — Swagger UI";
});

// Проверка работоспособности приложения.
app.MapGet("/", () => "Minimal API работает!")
    .WithName("GetRoot")
    .WithTags("General")
    .WithSummary("Проверить работу приложения")
    .WithDescription("Возвращает текстовое подтверждение того, что API запущено.");

// Информация о приложении.
app.MapGet("/api/info", () => Results.Ok(new
{
    Application = "Web Technologies",
    Technology = "ASP.NET Core Minimal API",
    Version = "1.0",
    Domain = "Склад и логистика",
    Entity = "Cargo",
    Storage = "In-memory (CargoStore)",
    Documentation = "/scalar/v1",
    Swagger = "/swagger"
}))
    .WithName("GetInfo")
    .WithTags("General")
    .WithSummary("Получить информацию о приложении")
    .Produces(StatusCodes.Status200OK);

// GET /api/cargoes — список грузов с поиском и фильтрацией.
app.MapGet("/api/cargoes", (string? search, string? category, bool? available) =>
{
    List<Cargo> snapshot;
    lock (CargoStore.SyncRoot)
    {
        snapshot = CargoStore.Cargoes.ToList();
    }

    IEnumerable<Cargo> cargoes = snapshot;

    if (!string.IsNullOrWhiteSpace(search))
    {
        var query = search.Trim();
        cargoes = cargoes.Where(cargo =>
            cargo.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            cargo.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            cargo.Location.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            cargo.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
        var query = category.Trim();
        cargoes = cargoes.Where(cargo =>
            string.Equals(cargo.Category, query, StringComparison.OrdinalIgnoreCase));
    }

    if (available is not null)
    {
        cargoes = cargoes.Where(cargo => cargo.IsAvailable == available);
    }

    return Results.Ok(cargoes.OrderBy(cargo => cargo.Id));
})
    .WithName("GetCargoes")
    .WithTags("Cargo")
    .WithSummary("Получить список грузов")
    .WithDescription("Возвращает все грузы. Query-параметр search ищет по названию, категории, " +
                     "месту хранения и описанию; category фильтрует по категории; " +
                     "available — по наличию на складе.")
    .Produces<IEnumerable<Cargo>>(StatusCodes.Status200OK);

// GET /api/cargoes/{id} — один груз по идентификатору.
app.MapGet("/api/cargoes/{id:int}", (int id) =>
{
    Cargo? cargo;
    lock (CargoStore.SyncRoot)
    {
        cargo = CargoStore.Cargoes.FirstOrDefault(item => item.Id == id);
    }

    return cargo is null
        ? Results.NotFound()
        : Results.Ok(cargo);
})
    .WithName("GetCargoById")
    .WithTags("Cargo")
    .WithSummary("Получить груз по идентификатору")
    .Produces<Cargo>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

// POST /api/cargoes — создание груза. Id генерируется на сервере.
app.MapPost("/api/cargoes", (Cargo input) =>
{
    Normalize(input);

    var errors = Validate(input);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    lock (CargoStore.SyncRoot)
    {
        input.Id = CargoStore.GetNextId();
        CargoStore.Cargoes.Add(input);
    }

    return Results.Created($"/api/cargoes/{input.Id}", input);
})
    .WithName("CreateCargo")
    .WithTags("Cargo")
    .WithSummary("Создать груз")
    .WithDescription("Принимает груз в формате JSON, назначает идентификатор и возвращает " +
                     "201 Created со ссылкой на созданный ресурс.")
    .Accepts<Cargo>("application/json")
    .Produces<Cargo>(StatusCodes.Status201Created)
    .ProducesValidationProblem();

// PUT /api/cargoes/{id} — полное обновление груза.
app.MapPut("/api/cargoes/{id:int}", (int id, Cargo input) =>
{
    Normalize(input);

    lock (CargoStore.SyncRoot)
    {
        var cargo = CargoStore.Cargoes.FirstOrDefault(item => item.Id == id);
        if (cargo is null)
        {
            return Results.NotFound();
        }

        var errors = Validate(input);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        cargo.Name = input.Name;
        cargo.Category = input.Category;
        cargo.Quantity = input.Quantity;
        cargo.Location = input.Location;
        cargo.Weight = input.Weight;
        cargo.Description = input.Description;
        cargo.IsAvailable = input.IsAvailable;

        return Results.Ok(cargo);
    }
})
    .WithName("UpdateCargo")
    .WithTags("Cargo")
    .WithSummary("Изменить груз")
    .WithDescription("Заменяет свойства существующего груза. Идентификатор из маршрута " +
                     "изменить нельзя. Возвращает 404, если груз не найден.")
    .Accepts<Cargo>("application/json")
    .Produces<Cargo>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .ProducesValidationProblem();

// DELETE /api/cargoes/{id} — удаление груза.
app.MapDelete("/api/cargoes/{id:int}", (int id) =>
{
    lock (CargoStore.SyncRoot)
    {
        var cargo = CargoStore.Cargoes.FirstOrDefault(item => item.Id == id);
        if (cargo is null)
        {
            return Results.NotFound();
        }

        CargoStore.Cargoes.Remove(cargo);
    }

    return Results.NoContent();
})
    .WithName("DeleteCargo")
    .WithTags("Cargo")
    .WithSummary("Удалить груз")
    .WithDescription("Удаляет груз из коллекции и возвращает 204 No Content.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

// Render передаёт порт через переменную окружения PORT.
// Приложение должно слушать 0.0.0.0, чтобы контейнер был доступен извне.
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

/// <summary>Удаляет лишние пробелы в строковых полях перед проверкой.</summary>
static void Normalize(Cargo cargo)
{
    cargo.Name = (cargo.Name ?? string.Empty).Trim();
    cargo.Category = (cargo.Category ?? string.Empty).Trim();
    cargo.Location = (cargo.Location ?? string.Empty).Trim();
    cargo.Description = (cargo.Description ?? string.Empty).Trim();
}

/// <summary>
/// Проверяет модель по атрибутам DataAnnotations и возвращает ошибки
/// в формате, который понимает Results.ValidationProblem (400 Bad Request).
/// </summary>
static Dictionary<string, string[]> Validate(Cargo cargo)
{
    var results = new List<ValidationResult>();
    var context = new ValidationContext(cargo);

    if (Validator.TryValidateObject(cargo, context, results, validateAllProperties: true))
    {
        return new Dictionary<string, string[]>();
    }

    return results
        .SelectMany(result =>
        {
            var members = result.MemberNames.Any()
                ? result.MemberNames
                : new[] { "cargo" };

            return members.Select(member => new
            {
                Member = member,
                Message = result.ErrorMessage ?? "Некорректное значение."
            });
        })
        .GroupBy(item => item.Member)
        .ToDictionary(
            group => group.Key,
            group => group.Select(item => item.Message).ToArray());
}
