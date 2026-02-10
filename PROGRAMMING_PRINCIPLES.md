# Принципи програмування в проєкті GlobalFests

---

## 1. SOLID Принципи

### SRP (Single Responsibility Principle) — Принцип єдиної відповідальності
Кожен клас або метод виконує лише одну задачу.
- **Приклад:**Методи контролера `EventsController`,`AccountController` не містять логіки доступу до БД. Вони лише приймають запити і викликають потрібні їм сервіси. Вся логіка розміщена на рівні сервісів і репозиторіїв
- **Код:** [EventsController.cs: Index метод](https://github.com/Mycola23/Global-Fests/blob/e404e1b7c5939cbd2f018dc9ce1bfda9819dccab/Controllers/EventsController.cs#L38C9-L71C10) 
- **Код:** [AccountController.cs:](https://github.com/Mycola23/Global-Fests/blob/234a158e02f63e7ad9646d42edad57431e6fdf74/Controllers/AccountController.cs#L29C9-L160C10) 

### OCP (Open/Closed Principle) — Принцип відкритості/закритості
Система спроєктована так, щоб її можна було розширювати без зміни існуючого коду.
- **Приклад:** Використання `Enum SortState` дозволяє додавати нові види сортування (наприклад, "За рейтингом"), просто додавши новий кейс у `switch`, не переписуючи весь метод пошуку.
- **Код:** [EventRepository.cs: switch(sortOrder)](GlobalFests/Repositories/EventRepository.cs#L55)

### DIP (Dependency Inversion Principle) — Принцип інверсії залежностей
Модулі вищого рівня не залежать від модулів нижчого рівня. Обидва залежать від абстракцій.
- **Приклад:** Контролери залежать від інтерфейсів (`IEventService`, `ILookupService`), а не від конкретних класів. Це дозволяє легко підміняти реалізацію (наприклад, для тестування).
- **Код:** [EventsController.cs: Constructor](GlobalFests/Controllers/EventsController.cs#L15)

---


## 2. Якість коду та інші принципи

### DRY (Don't Repeat Yourself) — Не повторюй себе
Повторювані фрагменти коду винесені в окремі компоненти.
- **Приклад:** Картка події (`Event Card`) використовується на Головній, у Пошуку та у WishList. Вона винесена в Partial View.
- **Код:** [Views/Shared/_EventCardPartial.cshtml](GlobalFests/Views/Shared/_EventCardPartial.cshtml)

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
