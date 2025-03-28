using NetCa.Domain.Common;
using System;

namespace NetCa.Domain.Entities
{
    public record Crypto : BaseAuditableEntity
    {
        public string Symbol { get; set; } = string.Empty; // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal MarketCap { get; set; }
        public decimal Volume24h { get; set; }
        public decimal Change24h { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
