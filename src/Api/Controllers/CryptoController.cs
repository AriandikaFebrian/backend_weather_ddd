using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Entities;
using NetCa.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace NetCa.Api.Controllers;

/// <summary>
/// Represents RESTful API for Cryptocurrency data.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/crypto")]
[ApiController]
public class CryptoController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoController"/> class.
    /// </summary>
    public CryptoController(HttpClient httpClient, IApplicationDbContext context, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// Retrieves cryptocurrency data from CoinGecko API or database.
    /// </summary>
    [HttpGet("public")]
    [Produces(HeaderConstants.Json)]
    public async Task<IActionResult> GetCryptoFromPublicApiAsync(
        [FromQuery] string symbol = "bitcoin",
        CancellationToken cancellationToken = default)
    {
        // Cek cache dulu (agar tidak selalu call API)
        if (_cache.TryGetValue(symbol, out Crypto cachedCrypto))
        {
            return Ok(cachedCrypto);
        }

        // Cek database
        var existingCrypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (existingCrypto != null)
        {
            // Simpan ke cache
            _cache.Set(symbol, existingCrypto, TimeSpan.FromMinutes(5));
            return Ok(existingCrypto);
        }

        // Fetch dari CoinGecko
        // Fetch dari CoinGecko
string apiKey = "CG-oKMRChPDnDiczNkvW3FiKJtL";
string url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={symbol}&x_cg_demo_api_key={apiKey}";
_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyCryptoApp/1.0)");

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, $"Error: {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();
            var cryptoData = JArray.Parse(responseBody);

            if (cryptoData.Count == 0)
                return NotFound("Cryptocurrency not found");

            var data = cryptoData[0];

            var crypto = new Crypto
            {
                Symbol = data["symbol"]?.ToString(),
                Name = data["name"]?.ToString(),
                Price = data["current_price"]?.ToObject<decimal>() ?? 0,
                MarketCap = data["market_cap"]?.ToObject<decimal>() ?? 0,
                Volume24h = data["total_volume"]?.ToObject<decimal>() ?? 0,
                Change24h = data["price_change_percentage_24h"]?.ToObject<decimal>() ?? 0,
                LastUpdated = DateTime.UtcNow
            };

            // Simpan ke database (update jika sudah ada)
            if (existingCrypto == null)
            {
                _context.Cryptos.Add(crypto);
            }
            else
            {
                existingCrypto.Price = crypto.Price;
                existingCrypto.MarketCap = crypto.MarketCap;
                existingCrypto.Volume24h = crypto.Volume24h;
                existingCrypto.Change24h = crypto.Change24h;
                existingCrypto.LastUpdated = crypto.LastUpdated;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Simpan ke cache
            _cache.Set(symbol, crypto, TimeSpan.FromMinutes(5));

            return Ok(crypto);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error retrieving cryptocurrency data: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves all cryptocurrency records from the database.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCryptosAsync(CancellationToken cancellationToken)
    {
        var cryptoList = await _context.Cryptos.ToListAsync(cancellationToken);
        return Ok(cryptoList);
    }

    /// <summary>
    /// Retrieves a cryptocurrency record by symbol.
    /// </summary>
    [HttpGet("{symbol}")]
    [Produces(HeaderConstants.Json)]
    public async Task<IActionResult> GetCryptoBySymbolAsync(string symbol, CancellationToken cancellationToken)
    {
        var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (crypto == null) return NotFound("Cryptocurrency data not found");

        return Ok(crypto);
    }

    /// <summary>
    /// Creates a new cryptocurrency record.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCrypto([FromBody] Crypto crypto, CancellationToken cancellationToken)
    {
        if (crypto == null)
        {
            return BadRequest("Cryptocurrency data is required.");
        }

        try
        {
            _context.Cryptos.Add(crypto);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetCryptoBySymbolAsync), new { symbol = crypto.Symbol }, crypto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing cryptocurrency record.
    /// </summary>
    [HttpPut("{symbol}")]
    [Produces(HeaderConstants.Json)]
    public async Task<IActionResult> UpdateCryptoAsync(string symbol, [FromBody] Crypto cryptoUpdate, CancellationToken cancellationToken)
    {
        var existingCrypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (existingCrypto == null) return NotFound("Cryptocurrency data not found");

        existingCrypto.Name = cryptoUpdate.Name;
        existingCrypto.Price = cryptoUpdate.Price;
        existingCrypto.MarketCap = cryptoUpdate.MarketCap;
        existingCrypto.Volume24h = cryptoUpdate.Volume24h;
        existingCrypto.Change24h = cryptoUpdate.Change24h;
        existingCrypto.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(existingCrypto);
    }

    /// <summary>
    /// Deletes a cryptocurrency record by symbol.
    /// </summary>
    [HttpDelete("{symbol}")]
    [Produces(HeaderConstants.Json)]
    public async Task<IActionResult> DeleteCryptoAsync(string symbol, CancellationToken cancellationToken)
    {
        var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == symbol, cancellationToken);
        if (crypto == null) return NotFound("Cryptocurrency data not found");

        _context.Cryptos.Remove(crypto);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("history")]
public async Task<IActionResult> GetCryptoHistoryAsync(
    [FromQuery] string symbol = "bitcoin",
    [FromQuery] string interval = "daily", // daily, hourly, monthly
    CancellationToken cancellationToken = default)
{
    string url = $"https://api.coingecko.com/api/v3/coins/{symbol}/market_chart?vs_currency=usd&days=30&interval={interval}";

    var response = await _httpClient.GetAsync(url, cancellationToken);
    if (!response.IsSuccessStatusCode)
        return StatusCode((int)response.StatusCode, $"Error: {response.StatusCode}");

    var responseBody = await response.Content.ReadAsStringAsync();
    return Ok(JObject.Parse(responseBody));
}

}
