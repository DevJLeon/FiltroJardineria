using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repository;
public class EmpleadoRepository: GenericRepo<Empleado>, IEmpleado
{
        private readonly ApiContext _context;
    
    public EmpleadoRepository(ApiContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .ToListAsync();
    }

    public async Task<Empleado> GetByIdAsync(int id)
    {
        return await _context.Empleados
        .FirstOrDefaultAsync(p =>  p.CodigoEmpleado == id);
    }
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

}