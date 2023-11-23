1. Devuelve el listado de clientes indicando el nombre del cliente y cuantos pedidos ha relizado. tenga en cuenta que pueden existir clientes que no han realizado ningún pedido

**cliente/ClientesPedidos** 

2. devuelve un listado con el codigo de pedido, codigo de cliente, fecha esperada y fecha de entrega de los pedidos que no han sido entregados a tiempo

PedidosTardios

3. Devuelve un listado de los productos que nunca han aparecido en un pedido. El resultado debe mostrar el nombre, la descripción y la imagen del producto.

ProductosSinPedir

4. Devuelve las oficinas donde no trabaja ninguno de los empleados que hayan sido los representantes de ventas de algún cliente que haya realizado la compra de algún producto de la gama frutales

OficinasSinRepresentantesVenta

5. las ventas totales de los producto sque hayan facturado más de 3000 euros. Se mostrará el nombre, unidades vendidas, total falcurado y total facturado con impuestos (21% IVA)

Task<IEnumerable<object>> TotalConIva()

6. Devuelve el nombre, apellidos, puesto y teléfono de la oficina de aquellos empleados que no sean representante de ventas de ningún cliente.

Task<IEnumerable<object>> EmpleadosNoRepresentante()

7. Devuelve el nombre del producto del que se han vendido más unidades (Tenga en cuenta que tendrá que calcular cuál es el número total de unidades que se han vendido de cada producto a partir de los datos de la tabla detalle_pedido)

Task<object> ProductoMasVendido()

8. Devuelve un listado de los 20 productos más vendidos y el número total de unidades que se han vendido de cada uno. El listado deberá estar ordenado por el número total de unidades vendidas.

Task<IEnumerable<object>> VenteMasVendidos()

9. Devuelve el nombre de los clientes a los que no se les ha entregado a tiempo un pedido

Task<IEnumerable<object>> ClientesConRetraso()

10. Devuelve un listado de las diferentes gamas de producto que ha comprado cada cliente

Task<IEnumerable<object>> GamasDeCliente()