using System.Globalization;
using NetCa.Application.Common.Mappings;

namespace NetCa.Application.Common.Vms;


public record CryptoVm : IMapFrom<Crypto>
{
    public string Symbol { get; set; } = string.Empty; // Primary Key
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
    public DateTime LastUpdated { get; set; }

    public string Date => LastUpdated.ToString("yyyy-mm-dd", CultureInfo.InvariantCulture);

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Crypto, CryptoVm>();
    }
}