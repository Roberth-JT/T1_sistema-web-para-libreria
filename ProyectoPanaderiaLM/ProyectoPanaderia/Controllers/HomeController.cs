using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Models;

namespace ProyectoPanaderia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var contenidos = _context.ContenidoPagina.ToList();
            return View(contenidos);
        }

        public IActionResult Privacy()
        {
            var politica = _context.ContenidoPagina.FirstOrDefault(c => c.Seccion == "PoliticaPrivacidad");
            return View(politica);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
