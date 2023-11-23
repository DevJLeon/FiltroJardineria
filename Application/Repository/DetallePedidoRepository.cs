using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class DetallePedidoRepository: GenericRepo<DetallePedido>, IDetallePedido
{
        private readonly ApiContext _context;
    
    public DetallePedidoRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<DetallePedido>> GetAllAsync()
    {
        return await _context.DetallePedidos
            .ToListAsync();
    }

    public async Task<DetallePedido> GetByIdAsync(int id)
    {
        return await _context.DetallePedidos
        .FirstOrDefaultAsync(p =>  p.CodigoPedido == id);
    }

}