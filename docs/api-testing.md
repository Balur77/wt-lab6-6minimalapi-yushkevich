# Результаты локального тестирования CargoFlow API

Все запросы выполнены к http://localhost:10000 после запуска приложения через dotnet run.

`	ext
GET / -> 200 (text/plain; charset=utf-8)
Minimal API работает!

GET /api/info -> 200 (application/json; charset=utf-8)
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

GET /api/cargoes -> 200 (application/json; charset=utf-8)
[
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
  },
  {
    "id": 2,
    "name": "Офисные кресла ErgoPro",
    "category": "Мебель",
    "quantity": 18,
    "location": "Сектор B-2, зона сборки",
    "weight": 12.5,
    "description": "Эргономичные кресла с регулируемой поддержкой поясницы. Поставка в разобранном виде.",
    "isAvailable": true,
    "totalWeight": 225.0,
    "status": "На складе"
  },
  {
    "id": 3,
    "name": "Кофемашины Jura E8",
    "category": "Бытовая техника",
    "quantity": 6,
    "location": "Сектор C-1, стеллаж 1",
    "weight": 9.8,
    "description": "Профессиональные кофемашины для переговорных комнат и клиентских зон.",
    "isAvailable": true,
    "totalWeight": 58.8,
    "status": "На складе"
  },
  {
    "id": 4,
    "name": "Наборы строительного инструмента",
    "category": "Инструменты",
    "quantity": 45,
    "location": "Сектор D-3, паллета 12",
    "weight": 25,
    "description": "Универсальные наборы инструментов для ремонтных бригад: дрели, шуруповёрты и расходники.",
    "isAvailable": true,
    "totalWeight": 1125,
    "status": "На складе"
  },
  {
    "id": 5,
    "name": "Медицинские маски",
    "category": "Медицина",
    "quantity": 1200,
    "location": "Сектор E-2, стеллаж 5",
    "weight": 0.005,
    "description": "Одноразовые трёхслойные маски в индивидуальной упаковке. Поставка коробами по 50 штук.",
    "isAvailable": true,
    "totalWeight": 6.0,
    "status": "На складе"
  },
  {
    "id": 6,
    "name": "Серверное оборудование Supermicro",
    "category": "Электроника",
    "quantity": 4,
    "location": "Сектор A-3, климат-зона",
    "weight": 18.6,
    "description": "Стоечные серверы для модернизации дата-центра. Ожидают таможенного оформления.",
    "isAvailable": false,
    "totalWeight": 74.4,
    "status": "Нет в наличии"
  }
]

GET /api/cargoes/1 -> 200 (application/json; charset=utf-8)
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

GET /api/cargoes/999 -> 404 ()

GET /api/cargoes?search=%D0%BD%D0%BE%D1%83%D1%82 -> 200 (application/json; charset=utf-8)
[
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
]

GET /api/cargoes?category=%D0%AD%D0%BB%D0%B5%D0%BA%D1%82%D1%80%D0%BE%D0%BD%D0%B8%D0%BA%D0%B0&available=true -> 200 (application/json; charset=utf-8)
[
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
]

POST /api/cargoes -> 201 (application/json; charset=utf-8)
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

POST /api/cargoes -> 400 (application/problem+json)
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "Введите название груза"
    ],
    "Category": [
      "Укажите категорию"
    ],
    "Quantity": [
      "Количество должно быть от 1 до 1 000 000"
    ],
    "Location": [
      "Укажите место хранения"
    ],
    "Weight": [
      "Вес должен быть положительным числом"
    ],
    "Description": [
      "Укажите описание груза"
    ]
  }
}

PUT /api/cargoes/7 -> 200 (application/json; charset=utf-8)
{
  "id": 7,
  "name": "Тестовый груз (изменён)",
  "category": "Проверка",
  "quantity": 12,
  "location": "Сектор T-2",
  "weight": 2,
  "description": "Груз обновлён через PUT.",
  "isAvailable": false,
  "totalWeight": 24,
  "status": "Нет в наличии"
}

PUT /api/cargoes/999 -> 404 ()

GET /api/cargoes/7 -> 200 (application/json; charset=utf-8)
{
  "id": 7,
  "name": "Тестовый груз (изменён)",
  "category": "Проверка",
  "quantity": 12,
  "location": "Сектор T-2",
  "weight": 2,
  "description": "Груз обновлён через PUT.",
  "isAvailable": false,
  "totalWeight": 24,
  "status": "Нет в наличии"
}

DELETE /api/cargoes/7 -> 204 ()

GET /api/cargoes/7 -> 404 ()

DELETE /api/cargoes/7 -> 404 ()

GET /openapi/v1.json -> 200 (application/json;charset=utf-8)

OpenAPI paths: /, /api/cargoes, /api/cargoes/{id}, /api/info
OpenAPI title: CargoFlow API; version: v1

GET /scalar/v1 -> 200 (text/html)

GET /swagger -> 200 (text/html;charset=utf-8)
`
