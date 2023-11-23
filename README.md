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

5. las ventas totales de los producto sque hayan facturado más de 3000 euros. Se mostrará el nombre, unidades vendidas, total falcurado y total facturado con impuestos (21% IVA)
`http://localhost:5184/api/producto/TotalConIva`

```
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
```

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

7. Devuelve el nombre del producto del que se han vendido más unidades (Tenga en cuenta que tendrá que calcular cuál es el número total de unidades que se han vendido de cada producto a partir de los datos de la tabla detalle_pedido)
`http://localhost:5184/api/producto/ProductoMasVendido`

```
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
```

8. Devuelve un listado de los 20 productos más vendidos y el número total de unidades que se han vendido de cada uno. El listado deberá estar ordenado por el número total de unidades vendidas.
`http://localhost:5184/api/producto/VenteMasVendidos`
```
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
```

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
