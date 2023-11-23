using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class ProductoRepository: GenericRepo<Producto>, IProducto
{
        private readonly ApiContext _context;
    
    public ProductoRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _context.Productos
            .ToListAsync();
    }

    public async Task<Producto> GetByIdAsync(string id)
    {
        return await _context.Productos
        .FirstOrDefaultAsync(pr =>  pr.CodigoProducto == id);
    }
    public async Task<IEnumerable<object>> ProductosSinPedir()
    {
        var data = await (
            from p in _context.Productos
            join op in _context.DetallePedidos on p.CodigoProducto equals op.CodigoProducto into oj
            from subop in oj.DefaultIfEmpty()
            where subop == null
            select new
            {
                NombreProducto = p.Nombre,
                Descripcion = p.Descripcion
            }
        ).ToListAsync();

        return data;
    }
    public async Task<IEnumerable<object>> TotalConIva()
    {
        var dato = await (
            from detallePedido in _context.DetallePedidos
            join producto in _context.Productos on detallePedido.CodigoProducto equals producto.CodigoProducto
            let totalFacturado = detallePedido.PrecioUnidad * detallePedido.Cantidad
            let totalConIVA = totalFacturado * 1.21M
            group new { detallePedido, producto } by new { producto.Nombre } into grp
            let totalVenta = grp.Sum(x => x.detallePedido.Cantidad)
            let totalFacturadoProductos = grp.Sum(x => x.detallePedido.PrecioUnidad * x.detallePedido.Cantidad)
            where totalFacturadoProductos > 3000
            select new
            {
                NombreProducto = grp.Key.Nombre,
                TotalFacturado = totalFacturadoProductos,
                UnidadesTotales = totalVenta,
                TotalFacturadoConIVA = totalFacturadoProductos * 1.21M
            }
        ).ToListAsync();

        return dato;
    }
    public async Task<object> ProductoMasVendido()
    {
        var dato = await (
            from dp in _context.DetallePedidos
            group dp by dp.CodigoProducto into grp
            orderby grp.Sum(p => p.Cantidad) descending
            join p in _context.Productos on grp.Key equals p.CodigoProducto
            select p.Nombre
        ).FirstOrDefaultAsync();

        return dato;
    }

    public async Task<IEnumerable<object>> VenteMasVendidos()
    {
        var data = await (
            from detallePedido in _context.DetallePedidos
            group detallePedido by detallePedido.CodigoProducto into grp
            join producto in _context.Productos on grp.Key equals producto.CodigoProducto
            orderby grp.Sum(dp => dp.Cantidad) descending
            select new
            {
                NombreProducto = producto.Nombre,
                Codigo = grp.Key,
                TotalUnidadesVendidas = grp.Sum(dp => dp.Cantidad)
            }
        ).Take(20).ToListAsync();
    
        return data;
    }

}