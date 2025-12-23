

## add possibility to choose genre of event in create event form -- completed
## check efficient of cursor pagination realized by Raw-SQL and LINQ

## made part of editing of events and performers
*  added on delete cascade for all many-to-many tables beside perimisionId in rolePermission has no action  

* add possibility to add performers in process of create new event,


****** features
add search in adding performers in create,edit form for events




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

### Event Creation - Performer Management
- [ ] Add performer selection interface in event creation form
- [ ] Allow multiple performers per event
- [ ] Display selected performers with preview cards

---

## 📋 Backlog

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

## 🎯 Future Enhancements
- [ ] Bulk performer management
- [ ] Performer availability calendar
- [ ] Auto-suggest performers based on event genre