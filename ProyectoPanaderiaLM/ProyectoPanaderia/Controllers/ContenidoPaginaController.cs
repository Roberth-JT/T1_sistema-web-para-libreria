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
    public class ContenidoPaginaController : Controller
    {
        private readonly AppDbContext _context;

        public ContenidoPaginaController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var contenidos = _context.ContenidoPagina.ToList();
            return View(contenidos);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var contenido = _context.ContenidoPagina.Find(id);
            if (contenido == null) return NotFound();
            return View(contenido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(ContenidoPagina model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contenido = _context.ContenidoPagina.Find(model.Id);
            if (contenido == null) return NotFound();

            contenido.Seccion = model.Seccion;
            contenido.Titulo = model.Titulo;
            contenido.Contenido = model.Contenido;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Contenido actualizado correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var contenido = await _context.ContenidoPagina.FindAsync(id);
            if (contenido == null)
            {
                return NotFound();
            }

            try
            {
                _context.ContenidoPagina.Remove(contenido);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["ErrorMessage"] = "No se pudo eliminar el contenido.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
