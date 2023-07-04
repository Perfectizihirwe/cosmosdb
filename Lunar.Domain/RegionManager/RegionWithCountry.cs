using Newtonsoft.Json;

namespace Lunar.Domain.RegionManager
{
    public class RegionWithCountry : BaseEntity
    {
        public string ParentId { get; set; } = Guid.NewGuid().ToString();

        public string Type { get; set; }

        public Country[] Countries { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Country
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string ShortCode { get; set; }

        public string Type { get; set; }
    }
}
