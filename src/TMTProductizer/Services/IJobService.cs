using CodeGen.Api.Testbed.Model;
using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IJobService
{
    Task<IReadOnlyList<JobPosting>> Find(JobsRequest query);
}
