using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Extensions;
using ProyectoPanaderia.Models;
using ProyectoPanaderia.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace ProyectoPanaderia.Controllers
{
    [Authorize(Roles = "Cliente")]

    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();
            return View(carrito);
        }

        [HttpPost]
        public IActionResult EliminarDelCarrito(int productoId)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();
            var itemARemover = carrito.FirstOrDefault(item => item.ProductoId == productoId);

            if (itemARemover != null)
            {
                carrito.Remove(itemARemover);
                HttpContext.Session.SetObjectAsJson("Carrito", carrito);
                //TempData["SuccessMessage"] = "Producto eliminado del carrito.";
            }
            return RedirectToAction(nameof(Index));
        }

        //
        
        public IActionResult Checkout()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();

            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "Tu carrito está vacío. Agrega productos antes de realizar un pedido.";
                return RedirectToAction(nameof(Index)); // Redirige de vuelta a Mi Carrito si está vacío
            }

            var viewModel = new ResumenCompraVM
            {
                Items = carrito,
                Total = carrito.Sum(item => item.Subtotal)
            };

            return View(viewModel);
        }
        // En CarritoController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewOrder(ResumenCompraVM model)
        {
            // Recupera el carrito de la sesión, ya que no viene en el modelo directamente
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();

            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "Tu carrito está vacío. Agrega productos antes de finalizar un pedido.";
                return RedirectToAction(nameof(Index));
            }

            var rolClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);

            if (rolClaim != null)
            {
                model.RolComprador = rolClaim.Value; // Asigna el valor del rol encontrado
            }
            else
            {
                model.RolComprador = "ClientePorDefecto";
            }

            // Asegura que el ViewModel tenga los datos del carrito para mostrar en el resumen

            ModelState.Remove("Items");
            ModelState.Remove("RolComprador");

            model.Items = carrito;
            model.Total = carrito.Sum(item => item.Subtotal);
            
            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, vuelve a la misma vista de Checkout para mostrarlos
                return View("Checkout", model);
            }
            
            // Si todo es válido, pasa el modelo a la vista de resumen
            return View(model);
        }

        // 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarCompra(ResumenCompraVM model)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<ItemCarritoVM>>("Carrito") ?? new List<ItemCarritoVM>();

            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío. No se puede realizar la compra.";
                return RedirectToAction(nameof(Index));
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Error de autenticación: No se pudo obtener un ID de usuario válido.";
                return RedirectToAction("Login", "Account");
            }

            // Crear la nueva Compra
            var nuevaCompra = new Compra
            {
                UsuarioId = userId,
                Fecha = DateTime.Now,
                MontoTotal = carrito.Sum(item => item.Subtotal),
                DireccionEntrega = model.DireccionEntrega, 
                Referencia = model.Referencia,             
                MetodoPago = model.MetodoPago,
                Detalles = new List<DetalleCompra>()
            };

            // Crear los Detalles de Compra
            foreach (var item in carrito)
            {
                nuevaCompra.Detalles.Add(new DetalleCompra
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    PrecioTotal = item.Subtotal
                });
            }

            try
            {
                _context.Compras.Add(nuevaCompra);
                await _context.SaveChangesAsync();

                // Limpiar el carrito después de la compra exitosa
                HttpContext.Session.Remove("Carrito");

                TempData["SuccessMessage"] = "¡Compra realizada con éxito!";
                // Redirige a la pantalla de confirmación de compra
                return RedirectToAction("ConfirmacionCompra"); // Nueva acción
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al finalizar compra: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                TempData["ErrorMessage"] = "Hubo un error al procesar tu compra. Por favor, inténtalo de nuevo.";
                // Si hay un error, puedes redirigir de nuevo a la vista de resumen o al carrito
                // Para que se mantenga el flujo, mejor redirigir al Checkout de nuevo con el modelo para que muestre errores si hay
                // return View("ReviewOrder", model); // Esto no funcionará bien con TempData, mejor RedirectToAction.
                return RedirectToAction("Checkout"); // Volver a la pantalla de detalles de compra
            }
        }

        public IActionResult ConfirmacionCompra()
        {
            return View();
        }
    }

    /*
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
                return RedirectToAction("Login", "Cuenta");

            var carrito = _context.Carritos
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => new
                {
                    c.Id,
                    c.ProductoId,
                    c.Cantidad,
                    Producto = c.Producto
                })
                .ToList();

            return View(carrito);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agregar(int productoId, int cantidad)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
                return RedirectToAction("Login", "Cuenta");

            var existente = _context.Carritos.FirstOrDefault(c =>
                c.UsuarioId == usuarioId && c.ProductoId == productoId);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                _context.Carritos.Add(new Carrito
                {
                    UsuarioId = usuarioId,
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    AgregadoEl = DateTime.Now
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(int id, int cantidad)
        {
            var item = _context.Carritos.Find(id);
            if (item == null) return NotFound();

            item.Cantidad = cantidad;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            var item = _context.Carritos.Find(id);
            if (item == null) return NotFound();

            _context.Carritos.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        */
}

