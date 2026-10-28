using wt_lab6_6minimalapi_yushkevich.Models;

namespace wt_lab6_6minimalapi_yushkevich.Data;

/// <summary>
/// Единое хранилище грузов в памяти приложения.
/// Одна и та же коллекция используется всеми endpoint'ами API,
/// поэтому изменения сразу видны при последующих запросах.
/// После перезапуска приложения данные восстанавливаются из начального набора.
/// </summary>
public static class CargoStore
{
    /// <summary>
    /// Объект синхронизации: ASP.NET Core обрабатывает запросы параллельно,
    /// а <see cref="List{T}"/> не является потокобезопасным.
    /// </summary>
    public static object SyncRoot { get; } = new();

    /// <summary>Коллекция грузов — временное хранилище вместо базы данных.</summary>
    public static List<Cargo> Cargoes { get; } = new()
    {
        new Cargo
        {
            Id = 1,
            Name = "Ноутбуки Dell Latitude 5440",
            Category = "Электроника",
            Quantity = 24,
            Location = "Сектор A-1, стеллаж 3",
            Weight = 2.4m,
            Description = "Партия офисных ноутбуков для оснащения новых рабочих мест. Гарантия производителя — 24 месяца.",
            IsAvailable = true
        },
        new Cargo
        {
            Id = 2,
            Name = "Офисные кресла ErgoPro",
            Category = "Мебель",
            Quantity = 18,
            Location = "Сектор B-2, зона сборки",
            Weight = 12.5m,
            Description = "Эргономичные кресла с регулируемой поддержкой поясницы. Поставка в разобранном виде.",
            IsAvailable = true
        },
        new Cargo
        {
            Id = 3,
            Name = "Кофемашины Jura E8",
            Category = "Бытовая техника",
            Quantity = 6,
            Location = "Сектор C-1, стеллаж 1",
            Weight = 9.8m,
            Description = "Профессиональные кофемашины для переговорных комнат и клиентских зон.",
            IsAvailable = true
        },
        new Cargo
        {
            Id = 4,
            Name = "Наборы строительного инструмента",
            Category = "Инструменты",
            Quantity = 45,
            Location = "Сектор D-3, паллета 12",
            Weight = 25m,
            Description = "Универсальные наборы инструментов для ремонтных бригад: дрели, шуруповёрты и расходники.",
            IsAvailable = true
        },
        new Cargo
        {
            Id = 5,
            Name = "Медицинские маски",
            Category = "Медицина",
            Quantity = 1200,
            Location = "Сектор E-2, стеллаж 5",
            Weight = 0.005m,
            Description = "Одноразовые трёхслойные маски в индивидуальной упаковке. Поставка коробами по 50 штук.",
            IsAvailable = true
        },
        new Cargo
        {
            Id = 6,
            Name = "Серверное оборудование Supermicro",
            Category = "Электроника",
            Quantity = 4,
            Location = "Сектор A-3, климат-зона",
            Weight = 18.6m,
            Description = "Стоечные серверы для модернизации дата-центра. Ожидают таможенного оформления.",
            IsAvailable = false
        }
    };

    /// <summary>Возвращает следующий свободный идентификатор.</summary>
    public static int GetNextId() =>
        Cargoes.Count == 0 ? 1 : Cargoes.Max(cargo => cargo.Id) + 1;
}
