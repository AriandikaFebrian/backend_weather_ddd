using System.Data;
using MediatR;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Entities;

namespace NetCa.Application.Cryptos.Commands;

/// <summary>
/// Command untuk membuat Crypto baru
/// </summary>
public class CreateCryptoCommand : IRequest<Unit>
{
    public string Symbol { get; set; } = string.Empty; // Primary Key
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Handler untuk menangani CreateCryptoCommand
/// </summary>
public class CreateCryptoCommandHandler : IRequestHandler<CreateCryptoCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CreateCryptoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateCryptoCommand request, CancellationToken cancellationToken)
    {
        // Validasi sederhana
        if (string.IsNullOrWhiteSpace(request.Symbol) || string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Symbol dan Name harus diisi.");
        }

        // Buat objek Crypto baru
        var crypto = new Crypto
        {
            Symbol = request.Symbol,
            Name = request.Name,
            Price = request.Price,
            MarketCap = request.MarketCap,
            Volume24h = request.Volume24h,
            Change24h = request.Change24h,
            LastUpdated = request.LastUpdated
        };

        // Tambahkan ke database
        _context.Cryptos.Add(crypto);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class CreateCryptoCommandValidator : AbstractValidator<CreateCryptoCommand>
{
    public CreateCryptoCommandValidator()
    {
       RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol tidak boleh kosong.")
            .MaximumLength(10).WithMessage("Symbol tidak boleh lebih dari 10 karakter.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama tidak boleh kosong.")
            .MaximumLength(50).WithMessage("Nama tidak boleh lebih dari 50 karakter.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Harga harus lebih besar dari 0.");

        RuleFor(x => x.MarketCap)
            .GreaterThanOrEqualTo(0).WithMessage("Market Cap tidak boleh negatif.");

        RuleFor(x => x.Volume24h)
            .GreaterThanOrEqualTo(0).WithMessage("Volume perdagangan 24 jam tidak boleh negatif.");

        RuleFor(x => x.Change24h)
            .InclusiveBetween(-100, 100).WithMessage("Perubahan harga dalam 24 jam harus antara -100% dan 100%.");

        RuleFor(x => x.LastUpdated)
            .NotEmpty().WithMessage("Tanggal pembaruan tidak boleh kosong.");
    }
}
