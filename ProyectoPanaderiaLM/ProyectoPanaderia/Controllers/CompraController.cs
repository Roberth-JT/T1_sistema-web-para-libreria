using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Models;
using ProyectoPanaderia.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;

namespace ProyectoPanaderia.Controllers
{
    
    public class CompraController : Controller
    {
        private readonly AppDbContext _context;

        public CompraController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Index()
        {
            // Obtener todas las compras, incluyendo los usuarios y los detalles de compra
            // Para cada detalle de compra, incluir el producto
            var compras = await _context.Compras
                .Include(c => c.Usuario) // Para mostrar el nombre del usuario
                .Include(c => c.Detalles)
                    .ThenInclude(dc => dc.Producto) // Para obtener el nombre y otros detalles del producto
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return View(compras);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Historial()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Error de autenticación: No se pudo obtener un ID de usuario válido.";
                return RedirectToAction("Login", "Account");
            }

            var misCompras = await _context.Compras
                .Where(c => c.UsuarioId == userId)
                .Include(c => c.Detalles)
                    .ThenInclude(dc => dc.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return View(misCompras);
        }
    }
}
