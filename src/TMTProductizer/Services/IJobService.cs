using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IJobService
{
    Task<(List<Job> jobs, long IlmoituksienMaara)> Find(JobsRequest query);
}
