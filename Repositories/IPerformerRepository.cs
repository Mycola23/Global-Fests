using GlobalFests.DTOs;
using GlobalFests.EFModels;

namespace GlobalFests.Repositories
{
    public interface IPerformerRepository :ICRUD<Performer>
    {
        Task<PerformerWithDetailsDto?> GetPerformerWithDetailsAsync(int id);
        Task<CursorResult<PerformerDto>> GetAllPerformersByOrganizerAsync(
           int organizerId,
           DateTime? cursorDate,
           int? cursorId,
           int pageSize,
           CancellationToken cancellationToken = default);
    }
}
