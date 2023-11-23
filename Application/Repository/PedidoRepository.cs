using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class PedidoRepository: GenericRepo<Pedido>, IPedido
{
        private readonly ApiContext _context;
    
    public PedidoRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos
            .ToListAsync();
    }

    public async Task<Pedido> GetByIdAsync(int id)
    {
        return await _context.Pedidos
        .FirstOrDefaultAsync(p =>  p.CodigoPedido == id);
    }
    public async Task<IEnumerable<object>> PedidosRetrasados()
    {
        var dato = await (
        from pe in _context.Pedidos
        join cl in _context.Clientes on pe.CodigoCliente equals cl.CodigoCliente
        where pe.Estado == "Pendiente"
        where pe.FechaEntrega > pe.FechaEsperada
        select new
        {
            CodigoPedido = pe.CodigoPedido,
            NombreCliente = cl.NombreCliente,
            FechaEspera = pe.FechaEsperada,
            FechaEntrega = pe.FechaEntrega

        }
        ).ToListAsync();

        return dato;
    }
}