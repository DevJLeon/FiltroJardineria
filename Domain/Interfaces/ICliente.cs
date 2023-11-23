using Domain.Entities;

namespace Domain.Interfaces;
public interface ICliente : IGenericRepo<Cliente>
{
    Task<IEnumerable<object>> ClientesPedidos();
    Task<IEnumerable<object>> ClientesConRetraso();
    Task<IEnumerable<object>> GamasDeCliente();
}
