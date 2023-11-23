using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class PagoRepository: GenericRepo<Pago>, IPago
{
        private readonly ApiContext _context;
    
    public PagoRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<Pago>> GetAllAsync()
    {
        return await _context.Pagos
            .ToListAsync();
    }

    public async Task<Pago> GetByIdAsync(int id)
    {
        return await _context.Pagos
        .FirstOrDefaultAsync(pa =>  pa.CodigoCliente == id);
    }

}