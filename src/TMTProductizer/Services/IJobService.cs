using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IJobService
{
    Task<(List<Job> jobs, long totalCount)> Find(JobsRequest query);
    Task WakeUp();
}
