using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoPanaderia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var lista = _context.Categorias.ToList();
            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear(Categoria c)
        {
            if (!ModelState.IsValid) return View(c);

            _context.Categorias.Add(c);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var cat = _context.Categorias.Find(id);
            return cat == null ? NotFound() : View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(Categoria c)
        {
            if (!ModelState.IsValid) return View(c);

            var existente = _context.Categorias.Find(c.Id);
            if (existente == null) return NotFound();

            existente.Nombre = c.Nombre;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "No se pudo eliminar la categoría. Podría estar asociada a un producto.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
