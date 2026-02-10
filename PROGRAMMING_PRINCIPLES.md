# Принципи програмування в проєкті GlobalFests

---

## 1. SOLID Принципи

### SRP (Single Responsibility Principle) — Принцип єдиної відповідальності
Кожен клас або метод виконує лише одну задачу.
- **Приклад:** Контролер `EventsController` не містить логіки доступу до БД. Він лише приймає запит і викликає сервіс. Вся логіка пошуку інкапсульована в репозиторії.
- **Код:** [EventsController.cs: Index метод](GlobalFests/Controllers/EventsController.cs#L30) (тільки виклик сервісу)
- **Код:** [EventRepository.cs: SearchEventsSortedAsync](GlobalFests/Repositories/EventRepository.cs#L15) (логіка побудови запиту)

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
