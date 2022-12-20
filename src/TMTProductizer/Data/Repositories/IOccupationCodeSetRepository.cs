
using TMTProductizer.Models.Repositories;

namespace TMTProductizer.Data.Repositories;

public interface IOccupationCodeSetRepository
{
    Task<List<OccupationCodeSet.Occupation>> GetAllOccupations();
}