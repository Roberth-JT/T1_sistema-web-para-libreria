using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPanaderia.Data;
using ProyectoPanaderia.Models;
using ProyectoPanaderia.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BCrypt.Net;

[AllowAnonymous]
public class CuentaController : Controller
{
    private readonly AppDbContext _context;

    public CuentaController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (ModelState.IsValid)
        {
            var usuario = _context.Usuarios.Include(u => u.Rol).FirstOrDefault(u => u.Correo == model.EmailOrUsername);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    public IActionResult Registro()
    {
        var adminExistente = _context.Usuarios.Include(u => u.Rol).Any(u => u.Rol.Nombre == "Administrador");

        ViewBag.PermitirAdmin = !adminExistente;
        ViewBag.Roles = _context.Roles.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(RegistroVM model)
    {
        var adminExistente = _context.Usuarios.Include(u => u.Rol).Any(u => u.Rol.Nombre == "Administrador");

        if (!ModelState.IsValid)
        {
            ViewBag.PermitirAdmin = !adminExistente;
            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        if (model.RolId == 1 && adminExistente)
        {
            ModelState.AddModelError("RolId", "Ya existe un administrador registrado.");
            ViewBag.PermitirAdmin = false;
            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        if (_context.Usuarios.Any(u => u.Correo == model.Correo))
        {
            ModelState.AddModelError("Correo", "Este correo ya está registrado.");
            ViewBag.PermitirAdmin = !adminExistente;
            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        var nuevo = new Usuario
        {
            Nombre = model.Nombre,
            Correo = model.Correo,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Contraseña),
            RolId = model.RolId
        };

        _context.Usuarios.Add(nuevo);
        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    [Route("Cuenta/AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
