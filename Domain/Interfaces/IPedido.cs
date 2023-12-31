using Domain.Entities;

namespace Domain.Interfaces;
public interface IPedido : IGenericRepo<Pedido>
{
    Task<IEnumerable<object>> PedidosRetrasados();
}
