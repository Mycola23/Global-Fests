using GlobalFests.EFModels;
using GlobalFests.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFests.Services
{
    public interface ILookupService
    {
        Task<List<Country>> GetAllCountriesAsync();
        Task<List<EventType>> GetAllEventTypesAsync();
        Task<List<Genre>> GetAllGenresAsync();
        Task<List<Role>> GetAllRolesAsync();
        Task<Country?> GetCountryByIdAsync(int id);
        Task<EventType?> GetEventTypeByIdAsync(int id);
        Task<Genre?> GetGenreByIdAsync(int id);
        Task<Role?> GetRoleByIdAsync(int id);
    }

    public class LookupService : ILookupService
    {
        private readonly IMemoryCache _cache;
        private readonly ICRUD<Country> _countryRepository;
        private readonly ICRUD<EventType> _eventTypeRepository;
        private readonly ICRUD<Genre> _genreRepository;
        private readonly ICRUD<Role> _roleRepository;

        public LookupService(IMemoryCache cache,
            ICRUD<Country> countryRepository,
            ICRUD<EventType> eventTypeRepository,
            ICRUD<Genre> genreRepository,
            ICRUD<Role> roleRepository)
        {
            _countryRepository = countryRepository;
            _eventTypeRepository = eventTypeRepository;
            _genreRepository = genreRepository;
            _roleRepository = roleRepository;
            _cache = cache;
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _cache.GetOrCreateAsync("all_countries", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _countryRepository.GetAllAsync();
            }) ?? new List<Country>();
          ;
        }

        public async Task<List<EventType>> GetAllEventTypesAsync()
        {
            return await _cache.GetOrCreateAsync("all_event_types", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _eventTypeRepository.GetAllAsync();
            }) ?? new List<EventType>();
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _cache.GetOrCreateAsync("all_genres", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _genreRepository.GetAllAsync();
            }) ?? new List<Genre>();
            
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _cache.GetOrCreateAsync("all_roles", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _roleRepository.GetAllAsync();
            }) ?? new List<Role>();
        }

        public async Task<Country?> GetCountryByIdAsync(int id)
        {
            return await _countryRepository.GetByIdAsync(id);
        }

        public async Task<EventType?> GetEventTypeByIdAsync(int id)
        {
            return await _eventTypeRepository.GetByIdAsync(id);
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _genreRepository.GetByIdAsync(id);
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }
    }
}
