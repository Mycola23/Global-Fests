using GlobalFests.EFModels;
using GlobalFests.Repositories;

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
        private readonly ICRUD<Country> _countryRepository;
        private readonly ICRUD<EventType> _eventTypeRepository;
        private readonly ICRUD<Genre> _genreRepository;
        private readonly ICRUD<Role> _roleRepository;

        public LookupService(
            ICRUD<Country> countryRepository,
            ICRUD<EventType> eventTypeRepository,
            ICRUD<Genre> genreRepository,
            ICRUD<Role> roleRepository)
        {
            _countryRepository = countryRepository;
            _eventTypeRepository = eventTypeRepository;
            _genreRepository = genreRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _countryRepository.GetAllAsync();
        }

        public async Task<List<EventType>> GetAllEventTypesAsync()
        {
            return await _eventTypeRepository.GetAllAsync();
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
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
