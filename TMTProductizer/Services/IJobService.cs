using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IJobService
{
    Task<IReadOnlyList<Job>> Find(JobsRequest query, int pageNumber, int pagerTake);
}