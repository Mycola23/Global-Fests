# Принципи програмування в проєкті GlobalFests

---

## 1. SOLID Принципи

### SRP (Single Responsibility Principle) — Принцип єдиної відповідальності
Кожен клас або метод виконує лише одну задачу.
- Методи контролера `EventsController`,`AccountController` не містять логіки доступу до БД. Вони лише приймають запити і викликають потрібні їм сервіси. Вся логіка розміщена на рівні сервісів і репозиторіїв
- **Код:** [EventsController.cs: Index метод](https://github.com/Mycola23/Global-Fests/blob/e404e1b7c5939cbd2f018dc9ce1bfda9819dccab/Controllers/EventsController.cs#L38C9-L71C10) 
- **Код:** [AccountController.cs:](https://github.com/Mycola23/Global-Fests/blob/234a158e02f63e7ad9646d42edad57431e6fdf74/Controllers/AccountController.cs#L29C9-L160C10)

### DIP (Dependency Inversion Principle) — Принцип інверсії залежностей
Модулі вищого рівня не залежать від модулів нижчого рівня. Обидва залежать від абстракцій.
- Приклади: Контролери залежать від інтерфейсів (`IEventService`, `ILookupService`,`IUserService`,`IOrganizerStatsService`), а не від конкретних класів. Це дозволяє легко підміняти реалізацію (наприклад, для тестування) чи змінювати реалізацію бізнес-логіки без змін у контролерах.
- **Код:** [EventsController.cs: Constructor](https://github.com/Mycola23/Global-Fests/blob/cecfcae1ac535200713584457a556b779eed7979/Controllers/EventsController.cs#L13C5-L35C10)
- **Код:** [OrganizerController.cs: Constructor](https://github.com/Mycola23/Global-Fests/blob/cecfcae1ac535200713584457a556b779eed7979/Controllers/OrganizerController.cs#L13C5-L27C10)
---

## 2. Інші принципи

### DRY (Don't Repeat Yourself) — Не повторюй себе
Повторювані фрагменти коду винесені в окремі компоненти.
- **Приклад:** Картка події (`Event Card`) використовується на Головній сторінці, у пошуку подій та у списку бажаного (WishList). Щоб уникнути дублювання HTML-розмітки та логіки відображення, вона реалізована як Partial View.
- **Код:** [Views/Shared/_EventCardPartial.cshtml](Views/Shared/EventCardPartial.cshtml)
- **Приклад:** Generic-метод  `SearchEventsAsync<T>` сервісу EventService дозволяє виконувати складну фільтрацію, сортування та пагінацію, повертаючи різні типи даних ( повну модель події або спрощений DTO для сторінки WorldMap) через один універсальний механізм.
- **Код:** [SearchEventsAsync<T>](https://github.com/Mycola23/Global-Fests/blob/cecfcae1ac535200713584457a556b779eed7979/Services/EventService.cs#L62C9-L71C10)

### KISS (Keep It Simple, Stupid) — Роби простіше
Де це можливо, складні операції C# замінені на оптимізовані SQL-запити або збережені процедури для підвищення продуктивності.
- **Приклад:** Статистика для організатора рахується через збережені процедури SQL, а не витягуванням всіх даних у пам'ять.
- **Код:** [OrganizerStatsService.cs: GetEventTypeSalesDataAsync](GlobalFests/Services/OrganizerStatsService.cs#L60)

### Async/Await (Асинхронне програмування)
Всі операції вводу-виводу (запити до БД) виконуються асинхронно, щоб не блокувати головний потік виконання веб-сервера.
- **Приклад:** `await _context.Events.ToListAsync()`
- **Код:** [EventsController.cs: Index](GlobalFests/Controllers/EventsController.cs#L35)

---

## Як додати посилання (Інструкція для тебе):

1.  Залий свій код на **GitHub** (якщо ще не залив).
2.  Відкрий файл на GitHub, знайди потрібний рядок коду.
3.  Клікни на номер рядка (зліва), натисни `Shift` і клікни на кінцевий рядок (якщо треба виділити блок).
4.  Натисни на `...` (три крапки) зліва від рядка -> "Copy permalink".
5.  Встав це посилання в файл `PROGRAMMING_PRINCIPLES.md` замість моїх прикладів шляхів.

**Приклад як це має виглядати в Markdown:**
`- **Код:** [EventsController.cs](https://github.com/YourName/GlobalFests/blob/main/Controllers/EventsController.cs#L15-L25)`
