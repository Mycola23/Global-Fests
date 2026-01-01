using GlobalFests.Data;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
using GlobalFests.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Services
{
    public class AdminManageItemsService
    {
        private readonly GlobalFestsContext _context;

        public AdminManageItemsService(GlobalFestsContext context)
        {
            _context = context;
        }

        public async Task<List<AdminManageItemViewModel>> GetAllAsync(AdminManageItemType type)
        {
            return type switch
            {
                AdminManageItemType.Genres => await _context.Genres
                    .Select(x => new AdminManageItemViewModel
                    {
                        Id = x.Id,
                        Name = x.Genre1,
                        EntityType = type,
                        EventsCount = x.Events.Count(),
                        PerformersCount = x.Performers.Count(),
                    }).ToListAsync(),

                AdminManageItemType.EventTypes => await _context.EventTypes
                    .Select(x => new AdminManageItemViewModel
                    {
                        Id = x.Id,
                        Name = x.Type,
                        EntityType = type,
                        EventsCount = x.Events.Count(),
                    }).ToListAsync(),

                AdminManageItemType.Roles => await _context.Roles
                    .Select(x => new AdminManageItemViewModel
                    {
                        Id = x.Id,
                        Name = x.Role1,
                        EntityType = type,
                        UsersCount = x.Users.Count()
                    }).ToListAsync(),

                _ => new List<AdminManageItemViewModel>()
            };
        }

        public async Task<AdminManageItemViewModel?> GetByIdAsync(AdminManageItemType type, int id)
        {
            string? name = type switch
            {
                AdminManageItemType.Genres => (await _context.Genres.FindAsync(id))?.Genre1,
                AdminManageItemType.EventTypes => (await _context.EventTypes.FindAsync(id))?.Type,
                AdminManageItemType.Roles => (await _context.Roles.FindAsync(id))?.Role1,
                _ => null
            };

            if (name == null) return null;

            return new AdminManageItemViewModel
            {
                Id = id,
                Name = name,
                EntityType = type
            };
        }

        public async Task SaveAsync(AdminManageItemViewModel model)
        {
            switch (model.EntityType)
            {
                case AdminManageItemType.Genres:
                    if (model.Id == 0)
                        _context.Genres.Add(new Genre { Genre1 = model.Name });
                    else
                    {
                        var e = await _context.Genres.FindAsync(model.Id);
                        if (e != null) e.Genre1 = model.Name;
                    }
                    break;

                case AdminManageItemType.EventTypes:
                    if (model.Id == 0)
                        _context.EventTypes.Add(new EventType { Type = model.Name });
                    else
                    {
                        var e = await _context.EventTypes.FindAsync(model.Id);
                        if (e != null) e.Type = model.Name;
                    }
                    break;

                case AdminManageItemType.Roles:
                    if (model.Id == 0)
                        _context.Roles.Add(new Role { Role1 = model.Name });
                    else
                    {
                        var e = await _context.Roles.FindAsync(model.Id);
                        if (e != null) e.Role1 = model.Name;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AdminManageItemType type, int id)
        {
            switch (type)
            {
                case AdminManageItemType.Genres:
                    var g = await _context.Genres.FindAsync(id);
                    if (g != null) _context.Genres.Remove(g);
                    break;
                case AdminManageItemType.EventTypes:
                    var t = await _context.EventTypes.FindAsync(id);
                    if (t != null) _context.EventTypes.Remove(t);
                    break;
                case AdminManageItemType.Roles:
                    var r = await _context.Roles.FindAsync(id);
                    if (r != null) _context.Roles.Remove(r);
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
