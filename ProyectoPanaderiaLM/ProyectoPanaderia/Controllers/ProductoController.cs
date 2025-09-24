using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Extensions;
using ProyectoPanaderia.Models;
using ProyectoPanaderia.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoPanaderia.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var productos = _context.Productos.Include(p => p.Categoria).ToList();
            return View(productos);
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Crear()
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");
            return View(new ProductoVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Crear(ProductoVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", vm.CategoriaId);
                return View(vm);
            }

            // Validar que la categoría exista
            var categoriaExiste = _context.Categorias.Any(c => c.Id == vm.CategoriaId);
            if (!categoriaExiste)
            {
                ModelState.AddModelError("CategoriaId", "La categoría seleccionada no existe.");
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", vm.CategoriaId);
                return View(vm);
            }

            var producto = new Producto
            {
                Nombre = vm.Nombre,
                Descripcion = vm.Descripcion,
                Precio = vm.Precio,
                Imagen = vm.Imagen,
                CategoriaId = vm.CategoriaId,
                Disponible = vm.Disponible
            };

            _context.Productos.Add(producto);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Editar(int id)
        {
            var producto = _context.Productos.Find(id);
            if (producto == null) return NotFound();

            var vm = new ProductoVM
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Imagen = producto.Imagen,
                Disponible = producto.Disponible,
                CategoriaId = producto.CategoriaId
            };

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Editar(ProductoVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", vm.CategoriaId);
                return View(vm);
            }

            var producto = _context.Productos.Find(vm.Id);
            if (producto == null) return NotFound();

            producto.Nombre = vm.Nombre;
            producto.Descripcion = vm.Descripcion;
            producto.Precio = vm.Precio;
            producto.Imagen = vm.Imagen;
            producto.Disponible = vm.Disponible;
            producto.CategoriaId = vm.CategoriaId;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "No se pudo eliminar el producto. Podría estar asociado a una compra.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Agregar(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                TempData["ErrorMessage"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();

            var itemExistente = carrito.FirstOrDefault(item => item.ProductoId == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Add(new ItemCarritoVM
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    PrecioUnitario = producto.Precio,
                    Cantidad = 1
                });
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            TempData["SuccessMessage"] = $"{producto.Nombre} agregado al carrito.";
            return RedirectToAction(nameof(Index));
        }
    }
}
