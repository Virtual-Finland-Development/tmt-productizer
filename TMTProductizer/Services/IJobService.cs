using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IJobService
{
    Task<IReadOnlyList<Job>> Find(int pageNumber, int pagerTake);
}