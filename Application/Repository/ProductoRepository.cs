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
            from dp in _context.DetallePedidos
            join produ in _context.Productos on dp.CodigoProducto equals produ.CodigoProducto
            where dp.PrecioUnidad * dp.Cantidad > 3000
            group new { dp, produ } by new { produ.Nombre } into grupo
            select new
            {
                NombreProducto = grupo.Key.Nombre,
                UnidadesVendidas = grupo.Sum(x => x.dp.Cantidad),
                TotalFacturado = Math.Round(grupo.Sum(x => x.dp.PrecioUnidad * x.dp.Cantidad), 2),
                TotalFacturadoConIVA = Math.Round(grupo.Sum(x => (decimal)(x.dp.PrecioUnidad * x.dp.Cantidad) * 1.21m), 2) //Ajusta los decimales a 2 depues del punto.
            })
            .ToListAsync();

        return dato;
    }

public async Task<IEnumerable<object>> ProductoMasVendido()
    {
        var result = await (
            from dp in _context.DetallePedidos
            join p in _context.Productos on dp.CodigoProducto equals p.CodigoProducto
            group dp by new { dp.CodigoProducto, p.Nombre } into g
            select new
            {
                CodigoProducto = g.Key.Nombre,
                TotalUnidadesVendidas = g.Sum(dp => dp.Cantidad)
            }
        )
        .OrderByDescending(p => p.TotalUnidadesVendidas)
        .Take(1)
        .ToListAsync();
        return result;
    }
public async Task<IEnumerable<object>> VenteMasVendidos()
    {
        var result = await (
            from dp in _context.DetallePedidos
            join p in _context.Productos on dp.CodigoProducto equals p.CodigoProducto
            group dp by new { dp.CodigoProducto, p.Nombre } into g
            select new
            {
                CodigoProducto = g.Key.Nombre,
                TotalUnidadesVendidas = g.Sum(dp => dp.Cantidad)
            }
        )
        .OrderByDescending(p => p.TotalUnidadesVendidas)
        .Take(20)
        .ToListAsync();
        return result;
    }

}