using Domain.Entities;

namespace Domain.Interfaces;
public interface IProducto : IGenericRepo<Producto>
{
    Task<IEnumerable<object>> ProductosSinPedir();
    Task<IEnumerable<object>> TotalConIva();
Task<IEnumerable<object>> ProductoMasVendido();
    Task<IEnumerable<object>> VenteMasVendidos();
}
