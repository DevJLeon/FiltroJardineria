using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class ClienteRepository: GenericRepo<Cliente>, ICliente
{
        private readonly ApiContext _context;
    
    public ClienteRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .ToListAsync();
    }

    public async Task<Cliente> GetByIdAsync(int id)
    {
        return await _context.Clientes
        .FirstOrDefaultAsync(c =>  c.CodigoCliente == id);
    }

    public async Task<IEnumerable<object>> ClientesPedidos()
    {
        var dato = await (
            from c in _context.Clientes
            select new
            {
                Nombre_Cliente = c.NombreCliente,
                Apellido_Cliente = c.ApellidoContacto,
                Pedidos_Realizados = _context.Pedidos.Count(p => p.CodigoCliente == c.CodigoCliente)
            }
        ).ToListAsync();

        return dato;
    }
    public async Task<IEnumerable<object>> ClientesConRetraso()
    {
        var dato = await (
            from c in _context.Clientes
            join pe in _context.Pedidos on c.CodigoCliente equals pe.CodigoCliente
            where pe.FechaEntrega > pe.FechaEsperada && pe.Estado == "Pendiente"
            select new
            {
                NombreCliente = c.NombreCliente
            }
        ).Distinct().ToListAsync();

        return dato;
    }

    public async Task<IEnumerable<object>> GamasDeCliente()
    {
        var dato = await (
            from depe in _context.DetallePedidos
            join pr in _context.Productos on depe.CodigoProducto equals pr.CodigoProducto
            join pe in _context.Pedidos on depe.CodigoPedido equals pe.CodigoPedido
            join c in _context.Clientes on pe.CodigoCliente equals c.CodigoCliente
            group pr.Gama by c.NombreCliente into cgamas
            select new
            {
                Cliente = cgamas.Key,
                ListaGamas = cgamas.Distinct().ToList()
            }
            ).ToListAsync();

        return dato;
    }

}