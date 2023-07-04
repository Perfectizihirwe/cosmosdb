using Newtonsoft.Json;

namespace Lunar.Domain.CandidateManager
{
    public class Candidate : BaseEntity
    {
        public string Last { get; set; }

        public string Email { get; set; }

        public string CohortId { get; set; } = Guid.NewGuid().ToString();

        public string CohortName { get; set; }

        public Status Status { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class Status
    {
        public int CountCreditsAssigned { get; set; }

        public bool HasConsumedCredits { get; set; }

        public bool HasCredit { get; set; }

        public bool IsRegistered { get; set; }
    }
}
