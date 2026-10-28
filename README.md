# WT-6. CargoFlow API — REST API на ASP.NET Core Minimal API

Учебный проект по дисциплине «Web-технологии»: REST API для учёта грузов на складе,
выполненный на **ASP.NET Core Minimal API** с хранением данных в памяти приложения.

Предметная область и основная сущность `Cargo` перенесены из лабораторных работ
WT-4 (MVC) и WT-5 (Razor Pages).

## Ссылки

- **GitHub:** https://github.com/Balur77/wt-lab6-6minimalapi-yushkevich
- **Render:** https://wt-lab6-6minimalapi-yushkevich.onrender.com

> Публичный адрес Render становится доступен после создания Web Service
> (см. раздел [«Публикация на Render»](#публикация-на-render)).

## Предметная область

**Склад и логистика.** Основная сущность — `Cargo` (груз на складе).

| Свойство | Тип | Назначение |
| --- | --- | --- |
| `Id` | `int` | Идентификатор, назначается сервером |
| `Name` | `string` | Название груза |
| `Category` | `string` | Категория |
| `Quantity` | `int` | Количество единиц |
| `Location` | `string` | Место хранения на складе |
| `Weight` | `decimal` | Вес одной единицы, кг |
| `Description` | `string` | Описание груза |
| `IsAvailable` | `bool` | Признак наличия на складе |
| `TotalWeight` | `decimal` | Общий вес партии, вычисляется |
| `Status` | `string` | Текстовый статус, вычисляется |

Модель содержит атрибуты `DataAnnotations` (`[Required]`, `[StringLength]`, `[Range]`),
поэтому правила валидации описаны в одном месте и используются API при создании
и изменении объектов.

## Итоговый набор endpoint'ов

| Метод | URL | Назначение | Успешный статус |
| --- | --- | --- | --- |
| GET | `/` | Проверить работу приложения | `200 OK` |
| GET | `/api/info` | Получить информацию о приложении | `200 OK` |
| GET | `/api/cargoes` | Получить список грузов | `200 OK` |
| GET | `/api/cargoes/{id}` | Получить груз по идентификатору | `200 OK`, `404 Not Found` |
| GET | `/api/cargoes?search=...` | Выполнить поиск | `200 OK` |
| POST | `/api/cargoes` | Создать груз | `201 Created`, `400 Bad Request` |
| PUT | `/api/cargoes/{id}` | Изменить груз | `200 OK`, `400 Bad Request`, `404 Not Found` |
| DELETE | `/api/cargoes/{id}` | Удалить груз | `204 No Content`, `404 Not Found` |

Query-параметры списка:

- `search` — поиск по названию, категории, месту хранения и описанию
  (без учёта регистра);
- `category` — фильтрация по категории;
- `available` — фильтрация по наличию (`true` / `false`).

Параметры можно комбинировать:

```http
GET /api/cargoes?search=ноут
GET /api/cargoes?category=Электроника&available=true
```

## HTTP-статусы

| Ситуация | Статус |
| --- | --- |
| Успешный GET | `200 OK` |
| Успешный POST | `201 Created` |
| Успешный DELETE | `204 No Content` |
| Некорректные данные | `400 Bad Request` |
| Объект не найден | `404 Not Found` |

Для ошибок валидации возвращается `400` в формате `application/problem+json`
с перечнем полей и сообщений:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Введите название груза"],
    "Quantity": ["Количество должно быть от 1 до 1 000 000"]
  }
}
```

## Примеры JSON-запросов и ответов

### GET `/api/info`

```json
{
  "application": "Web Technologies",
  "technology": "ASP.NET Core Minimal API",
  "version": "1.0",
  "domain": "Склад и логистика",
  "entity": "Cargo",
  "storage": "In-memory (CargoStore)",
  "documentation": "/scalar/v1",
  "swagger": "/swagger"
}
```

### GET `/api/cargoes/1`

```json
{
  "id": 1,
  "name": "Ноутбуки Dell Latitude 5440",
  "category": "Электроника",
  "quantity": 24,
  "location": "Сектор A-1, стеллаж 3",
  "weight": 2.4,
  "description": "Партия офисных ноутбуков для оснащения новых рабочих мест. Гарантия производителя — 24 месяца.",
  "isAvailable": true,
  "totalWeight": 57.6,
  "status": "На складе"
}
```

### POST `/api/cargoes`

Тело запроса (без `id` — идентификатор назначает сервер):

```json
{
  "name": "Тестовый груз",
  "category": "Проверка",
  "quantity": 10,
  "location": "Сектор T-1",
  "weight": 1.5,
  "description": "Объект для проверки полного цикла CRUD.",
  "isAvailable": true
}
```

Ответ `201 Created`:

```json
{
  "id": 7,
  "name": "Тестовый груз",
  "category": "Проверка",
  "quantity": 10,
  "location": "Сектор T-1",
  "weight": 1.5,
  "description": "Объект для проверки полного цикла CRUD.",
  "isAvailable": true,
  "totalWeight": 15.0,
  "status": "На складе"
}
```

Заголовок `Location` ответа: `/api/cargoes/7`.

### PUT `/api/cargoes/7`

```json
{
  "name": "Тестовый груз (изменён)",
  "category": "Проверка",
  "quantity": 12,
  "location": "Сектор T-2",
  "weight": 2,
  "description": "Груз обновлён через PUT.",
  "isAvailable": false
}
```

Ответ — `200 OK` с обновлённым объектом; `TotalWeight` пересчитывается
(`24`), `Status` меняется на `"Нет в наличии"`.

### DELETE `/api/cargoes/7`

Ответ — `204 No Content` без тела. Повторный `DELETE` этого же объекта — `404 Not Found`.

## OpenAPI и документация

Добавлены пакеты `Microsoft.AspNetCore.OpenApi` и
`Swashbuckle.AspNetCore.SwaggerUI`.

- OpenAPI-документ: **`/openapi/v1.json`**
- Swagger UI: **`/swagger`**
- Интерактивная документация Scalar UI: **`/scalar/v1`**

Описание API генерируется по endpoint'ам и их метаданным (`WithSummary`,
`WithDescription`, `Produces`, `Accepts`). В документ включены схемы `Cargo`
и `HttpValidationProblemDetails`, параметры маршрута и query-параметры,
а также все используемые HTTP-статусы.

OpenAPI, Swagger UI и Scalar доступны в любом окружении, чтобы документацию
можно было проверить на опубликованном сервисе Render.

## Запуск локально

Требуется .NET 9 SDK.

```bash
dotnet restore
dotnet run
```

Приложение слушает `http://localhost:10000` (порт по умолчанию из `Program.cs`
или значение переменной окружения `PORT`).

Проверка:

```bash
curl http://localhost:10000/
curl http://localhost:10000/api/info
curl http://localhost:10000/api/cargoes
curl http://localhost:10000/api/cargoes/1
```

Для ручной проверки всех операций можно использовать файл
[`requests.http`](requests.http) в VS Code (расширение REST Client), Postman,
Scalar UI или `curl`.

## Docker

Многоэтапная сборка: SDK-образ используется только для restore/publish,
в итоговый образ попадают лишь опубликованные файлы.

```bash
docker build -t wt-lab6-6minimalapi .
docker run --rm -p 10000:10000 wt-lab6-6minimalapi
```

Проверка после запуска контейнера:

```bash
curl http://localhost:10000/
curl http://localhost:10000/api/cargoes
```

Dockerfile находится в корне проекта. Приложение слушает `0.0.0.0`, а порт
читается из переменной окружения `PORT`, как требуется для Render.

## Публикация на Render

1. Открыть Render Dashboard → **New → Web Service**.
2. Подключить GitHub и выбрать репозиторий
   `Balur77/wt-lab6-6minimalapi-yushkevich`.
3. Указать параметры:
   - Branch: `main`;
   - Runtime: **Docker**;
   - Dockerfile Path: `./Dockerfile`;
   - Health Check Path: `/api/info`.
4. Запустить deployment и дождаться статуса **Live**.
5. Проверить публичные адреса:
   - `https://<имя-сервиса>.onrender.com/`
   - `https://<имя-сервиса>.onrender.com/api/info`
   - `https://<имя-сервиса>.onrender.com/api/cargoes`
   - `https://<имя-сервиса>.onrender.com/api/cargoes/1`
   - `https://<имя-сервиса>.onrender.com/swagger`
   - `https://<имя-сервиса>.onrender.com/scalar/v1`
6. Выполнить `POST`, `PUT`, `DELETE` через Postman / Scalar / `curl`.
7. Вписать фактический адрес в раздел [«Ссылки»](#ссылки).

Переменную `PORT` Render передаёт автоматически. При каждом `git push` в ветку
`main` Render выполняет новый deployment.

## Git

```bash
git init
git add .
git commit -m "feat: create CargoFlow Minimal API with in-memory store"
git commit -m "feat: implement CRUD endpoints, validation and OpenAPI"
git commit -m "build: add Dockerfile, Render config and documentation"
git branch -M main
git remote add origin https://github.com/Balur77/wt-lab6-6minimalapi-yushkevich.git
git push -u origin main
```

## Тестирование

Локально проверены все операции; полный лог запросов и ответов —
[`docs/api-testing.md`](docs/api-testing.md).

| Запрос | Ожидаемый статус | Результат |
| --- | --- | --- |
| `GET /` | `200 OK` | ✅ |
| `GET /api/info` | `200 OK` | ✅ |
| `GET /api/cargoes` | `200 OK`, 6 объектов | ✅ |
| `GET /api/cargoes/1` | `200 OK` | ✅ |
| `GET /api/cargoes/999` | `404 Not Found` | ✅ |
| `GET /api/cargoes?search=ноут` | `200 OK`, 1 объект | ✅ |
| `GET /api/cargoes?category=Электроника&available=true` | `200 OK` | ✅ |
| `POST /api/cargoes` (корректные данные) | `201 Created`, `id = 7` | ✅ |
| `POST /api/cargoes` (пустые поля) | `400 Bad Request` | ✅ |
| `PUT /api/cargoes/7` | `200 OK`, данные обновлены | ✅ |
| `PUT /api/cargoes/999` | `404 Not Found` | ✅ |
| `DELETE /api/cargoes/7` | `204 No Content` | ✅ |
| `GET /api/cargoes/7` после удаления | `404 Not Found` | ✅ |
| `DELETE /api/cargoes/7` повторно | `404 Not Found` | ✅ |
| `GET /openapi/v1.json` | `200 OK`, документ OpenAPI 3.0.4 | ✅ |
| `GET /swagger` | `200 OK`, страница Swagger UI | ✅ |
| `GET /scalar/v1` | `200 OK`, HTML-страница Scalar | ✅ |

Полный цикл CRUD: `POST → GET → PUT → GET → DELETE → GET` — состояние коллекции
изменяется после каждой операции.

## Структура проекта

```text
wt-lab6-6minimalapi-yushkevich/
├── Data/
│   └── CargoStore.cs                  # единое хранилище в памяти
├── Models/
│   └── Cargo.cs                       # модель предметной области
├── docs/
│   └── api-testing.md                 # лог локального тестирования API
├── Program.cs                         # endpoints, OpenAPI, запуск приложения
├── requests.http                      # готовые HTTP-запросы для REST Client
├── Dockerfile                         # многоэтапная сборка образа
├── .dockerignore
├── .gitignore
├── README.md
└── wt-lab6-6minimalapi-yushkevich.csproj
```

## Хранение данных

Все endpoint'ы работают с одной коллекцией `CargoStore.Cargoes`:

```csharp
public static List<Cargo> Cargoes { get; } = new() { /* 6 начальных грузов */ };

public static int GetNextId() =>
    Cargoes.Count == 0 ? 1 : Cargoes.Max(cargo => cargo.Id) + 1;
```

`CargoStore` — временное хранилище, а не база данных:

- данные находятся в оперативной памяти процесса;
- после перезапуска приложения и после нового deployment на Render коллекция
  восстанавливается из начального набора;
- несколько экземпляров приложения не разделяют данные между собой.

Для перехода на PostgreSQL потребуется заменить статический список на
`DbContext` Entity Framework Core (или другой DAL), вынести строку подключения
в конфигурацию/переменные окружения и добавить миграции. Endpoint'ы при этом
сохранят контракт API.

### Потокобезопасность

ASP.NET Core обрабатывает запросы параллельно, а `List<T>` не потокобезопасен.
Поэтому чтения выполняются по снимку коллекции, а изменения (`POST`, `PUT`,
`DELETE`) — внутри `lock (CargoStore.SyncRoot)`.

## Используемые технологии

- C# 13 / .NET 9;
- ASP.NET Core Minimal API;
- `Microsoft.AspNetCore.OpenApi` — OpenAPI 3.0;
- `Scalar.AspNetCore` — интерактивная документация;
- `Swashbuckle.AspNetCore.SwaggerUI` — графический интерфейс Swagger UI;
- `System.Text.Json` — сериализация JSON (camelCase по умолчанию);
- `DataAnnotations` — валидация модели;
- Docker (многоэтапная сборка);
- Render Web Service.
