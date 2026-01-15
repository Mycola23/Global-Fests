# GlobalFests - Project Tasks

## ✅ Completed Tasks

### Event Genre Selection
- [x] Add genre selection dropdown in create event form
- [x] Support multiple genre selection per event
- [x] Validate genre associations

### Performance Optimization
- [x] Implement cursor pagination with Raw SQL
- [x] Compare efficiency: Raw SQL vs LINQ
- [x] Benchmark and document results

### Database Cascade Rules
- [x] Configure `ON DELETE CASCADE` for many-to-many relationships
- [x] Exception: `RolePermissions.PermissionId` - set to `NO ACTION`
- [x] Test cascade deletion behavior

---

## 🚧 In Progress

### Plans
- [ ] clear all code
- ✅ редагування евенту доступно до першої продажі(поле кількості квитків) або до дати початку евенту після настання дати евенту взагалі не можна редагувати подію
- ✅ delete functionality for organizer panel (btn + method in controller with all checks)
- ✅ make possibility by click in Events details on performer go to PerformerDetails page
- ✅ make interactive World Map
- ✅ Add performer selection interface in event creation form
- ✅ Allow multiple performers per event
- ✅ Display selected performers with preview cards
- ✅ fix when you on admin account and use edit Events or Performers = you must return to admin Events
- ✅ add possibility to choose genre of event in create event form
- ✅ where access has a admin - must have the same and more access SuperAdmin - in progress 🫡
- ✅ fix troubles in redirections(create,edit performers&events) when you are admin ,
- ✅ fix showing of events page (a lot of sliders , more beatiful readible),
- [ ] replace all css code in cshtml on css classes (more professional with bem)
- ✅ add adaptive to all pages 
- [ ] add more stats for admin (especially for superadmin(God⚡))
- [ ] make dropdown (guest- button) customazible to each of role
- [ ] add travel back in cursor paginations where it has used
- ✅ add all filters and sorting for search panel in progress(need sorting)
- ✅ make profile page
- ✅ make wishList page
- ✅ add reviews system for autorized users
- ✅ make home page similar to start designs

---

## 📋 Backlog

### check efficient of cursor pagination realized by Raw-SQL and LINQ

### Advanced Search Features
- [ ] **Search performers during event creation**
  - [ ] Real-time search by name
  - [ ] Filter by genre
  - [ ] Filter by country
  - [ ] Paginated search results
  
- [ ] **Search performers during event editing**
  - [ ] Add new performers to existing events
  - [ ] Remove performers from events
  - [ ] Reorder performer lineup

---

## 🎯 Features
- [ ] Bulk performer management
- [ ] Performer availability calendar
- [ ] Auto-suggest performers based on event genre
- [ ] add search in adding performers in create,edit form for events
- ✅ made part of editing of events and performers
  





