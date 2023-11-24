1. Devuelve el listado de clientes indicando el nombre del cliente y cuantos pedidos ha relizado. tenga en cuenta que pueden existir clientes que no han realizado ningún pedido
`http://localhost:5184/api/cliente/ClientesPedidos`

```
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
```
**Se realiza llama la tabla Cliente, y se realiza el filtro de los pedidos en "_context.Pedidos" cuyo códigoCliente es igual a el CodigoCliente de la tabla Cliente. Y con .Count se realiza el conteo de cuántas similitudes se han encontrado en el filtro para ser almacenadas en "Pedidos Realizados"**

2. devuelve un listado con el codigo de pedido, codigo de cliente, fecha esperada y fecha de entrega de los pedidos que no han sido entregados a tiempo
`http://localhost:5184/api/pedido/PedidosRetrasados`

```
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
```
**Se busca las entradas de la tabla pedidos, y se realiza el filtro con un JOIN a la tabla Clientes, donde corresponda el CodigoCliente, se realiza el filtro con el estado de "pendiente" en pedido y que la fecha de entrega sea mayor que la esperada, es decir, que no ha cumplido con la fecha esperada.**

3. Devuelve un listado de los productos que nunca han aparecido en un pedido. El resultado debe mostrar el nombre, la descripción y la imagen del producto.
`http://localhost:5184/api/producto/ProductosSinPedir`

```
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
```
**Buscando en productos, se hace un JOIN que trae los correspondientes en DetallePedido. se realiza una subconsulta y se le pide que solo traiga aquellos que no tengan entradas en pedidos.**

4. Devuelve las oficinas donde no trabaja ninguno de los empleados que hayan sido los representantes de ventas de algún cliente que haya realizado la compra de algún producto de la gama frutales
`http://localhost:5184/api/oficina/OficinasSinRepresentantesVenta`

```
    public async Task<IEnumerable<object>> OficinasSinRepresentantesVenta()
    {
        var dato = await (
            from of in _context.Oficinas
            where !_context.Clientes.Any(c => _context.Empleados
                    .Where(em => em.CodigoEmpleado == c.CodigoEmpleadoRepVentas)
                    .Any(salesRep => _context.DetallePedidos.Any(dp =>
                    
                        dp.CodigoProductoNavigation.Gama == "Frutales" && dp.CodigoPedidoNavigation.CodigoCliente == c.CodigoCliente
                    )
                )
            )
        select new
        {
            NombreOficina = of.CodigoOficina,
            Ciudad = of.Ciudad,
            Pais = of.Pais
        }
    ).ToListAsync();

    return dato;
    }
```
**Se traen los datos de Oficina, y se filtra por aquellos cuyos clientes no tengan un id de representante de venta asociado**

5. las ventas totales de los producto sque hayan facturado más de 3000 euros. Se mostrará el nombre, unidades vendidas, total falcurado y total facturado con impuestos (21% IVA)
`http://localhost:5184/api/producto/TotalConIva`

```
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
```
**De la tabla DetallePedido se hace un JOIN para traer los productos que correspondan, y se filtra por el precio indicado. Esto se almacena en un grupo para realizar operaciones con los datos almacenados, como la suma de cantidad, total de venta, y total de venta con IVA.**

6. Devuelve el nombre, apellidos, puesto y teléfono de la oficina de aquellos empleados que no sean representante de ventas de ningún cliente.
`http://localhost:5184/api/empleado/EmpleadosNoRepresentante`

```
    public async Task<IEnumerable<object>> EmpleadosNoRepresentante()
    {
        var dato = await (
            from em in _context.Empleados
            join cl in _context.Clientes on em.CodigoEmpleado equals cl.CodigoEmpleadoRepVentas into cj
            from subcon in cj.DefaultIfEmpty()
            where subcon == null
            join of in _context.Oficinas on em.CodigoOficina equals of.CodigoOficina
            select new
            {
                NombreEmpleado = em.Nombre,
                ApellidosEmpleado = em.Apellidol + " " + em.Apellidol,
                PuestoEmpleado = em.Puesto,
                TelefonoOficina = of.Telefono
            }
        ).ToListAsync();

        return dato;
    }
```
**Se traen los datos de Empleado, con JOIN en Clientes. posteriormente se realiza un filtro donde muestre solo los empleados que no tienen asociado un CodigoRepVentas de Cliente**

7. Devuelve el nombre del producto del que se han vendido más unidades (Tenga en cuenta que tendrá que calcular cuál es el número total de unidades que se han vendido de cada producto a partir de los datos de la tabla detalle_pedido)
`http://localhost:5184/api/producto/ProductoMasVendido`

```
public async Task<IEnumerable<object>> ProductoMasVendido()
    {
        var result = await (
            from dp in _context.DetallePedidos
            join p in _context.Productos on dp.CodigoProducto equals p.CodigoProducto
            group dp by new { dp.CodigoProducto, p.Nombre } into g
            select new
            {
                IdProducto = g.Key.Nombre,
                TotalUnidadesVendidas = g.Sum(dp => dp.Cantidad)
            }
        )
        .OrderByDescending(p => p.TotalUnidadesVendidas)
        .Take(1)
        .ToListAsync();
        return result;
    }
```
**De Detalle producto se traen los productos para realizar una cuenta de cada producto, y su total. Posteriormente se le indica a la consulta que los ordene y nos arroje el primer resultado (mayorventas)**

8. Devuelve un listado de los 20 productos más vendidos y el número total de unidades que se han vendido de cada uno. El listado deberá estar ordenado por el número total de unidades vendidas.
`http://localhost:5184/api/producto/VenteMasVendidos`
```
public async Task<IEnumerable<object>> GetC40()
    {
        var result = await (
            from dp in _context.DetallesPedidos
            join p in _context.Productos on dp.IdProducto equals p.Id
            group dp by new { dp.IdProducto, p.Nombre } into g
            select new
            {
                IdProducto = g.Key.Nombre,
                TotalUnidadesVendidas = g.Sum(dp => dp.Cantidad)
            }
        )
        .OrderByDescending(p => p.TotalUnidadesVendidas)
        .Take(20)
        .ToListAsync();
        return result;
    }
```
**De Detalle producto se traen los productos para realizar una cuenta de cada producto, y su total. Posteriormente se le indica a la consulta que los ordene y nos arroje los primeros 20 resultados**

9. Devuelve el nombre de los clientes a los que no se les ha entregado a tiempo un pedido
`http://localhost:5184/api/cliente/ClientesConRetraso`

```
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
```
**De Clientes, mediante un JOIN traemos los pedidos que se asocian a los clientes y filtramos aquellos cuya fecha de entrega no ha sido cumplida como lo esperado y devolvemos solo el nombre de dicho cliente**

10. Devuelve un listado de las diferentes gamas de producto que ha comprado cada cliente
`http://localhost:5184/api/cliente/GamasDeCliente`

```
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
```
**Traemos todos los pedidos en la tabla DetallePedido, y mediante JOIN se trae el Producto asociado, el pedido asociado, y el cliente asociado. Para posteriormente almacenar estos datos en un grupo y listar las gamas que el resúmen de las gamas que el cliente ha comprado**
