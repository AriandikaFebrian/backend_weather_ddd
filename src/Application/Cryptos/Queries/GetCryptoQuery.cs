using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using NetCa.Application.Common.Mappings;
using NetCa.Application.Common.Vms;
using NetCa.Application.Common.Mappings;
using NetCa.Domain.Entities;

namespace NetCa.Application.Cryptos.Queries.GetCrypto;

/// <summary>
/// Query untuk mendapatkan daftar cryptocurrency
/// </summary>
public record GetCryptoQuery : IRequest<DocumentRootJson<List<CryptoVm>>> { }

/// <summary>
/// Handler untuk GetCryptoQuery
/// </summary>
public class GetCryptoHandler : IRequestHandler<GetCryptoQuery, DocumentRootJson<List<CryptoVm>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCryptoHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DocumentRootJson<List<CryptoVm>>> Handle(GetCryptoQuery request, CancellationToken cancellationToken)
    {
        var cryptos = await _context.Cryptos.ToListAsync(cancellationToken);
        var cryptoVms = _mapper.Map<List<CryptoVm>>(cryptos);

        return new DocumentRootJson<List<CryptoVm>>
        {
            Data = cryptoVms,
        };
    }
}
