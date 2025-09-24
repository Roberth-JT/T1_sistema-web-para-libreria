using Microsoft.EntityFrameworkCore;
using ProyectoPanaderia.Models;

namespace ProyectoPanaderia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetallesCompra { get; set; }
        public DbSet<DireccionEnvio> DireccionesEnvio { get; set; }
        public DbSet<HistorialCompra> HistorialCompras { get; set; }
        public DbSet<ContenidoPagina> ContenidoPagina { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UsuarioId);

            modelBuilder.Entity<Compra>()
                .Property(c => c.MontoTotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Carrito>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Carritos)
                .HasForeignKey(c => c.UsuarioId);

            modelBuilder.Entity<Carrito>()
                .HasOne(c => c.Producto)
                .WithMany()
                .HasForeignKey(c => c.ProductoId);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraId);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId);

            modelBuilder.Entity<DireccionEnvio>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.UsuarioId);

            modelBuilder.Entity<HistorialCompra>()
                .HasOne(h => h.Usuario)
                .WithMany(u => u.Historial)
                .HasForeignKey(h => h.UsuarioId);
        }
        public void SeedContenidoPagina()
        {
            if (ContenidoPagina.Any()) return;

            ContenidoPagina.AddRange(
                new ContenidoPagina
                {
                    Seccion = "Carrusel1",
                    Titulo = "Pan artesanal",
                    Contenido = "/img/pan_tradicional.png"
                },
                new ContenidoPagina
                {
                    Seccion = "Carrusel2",
                    Titulo = "Dulces tradicionales",
                    Contenido = "/img/dulces.png"
                },
                new ContenidoPagina
                {
                    Seccion = "Carrusel3",
                    Titulo = "Pan horneado",
                    Contenido = "/img/pan_horneado.png"
                },
                new ContenidoPagina
                {
                    Seccion = "BienvenidaTexto",
                    Titulo = "¡Bienvenido a Panadería La Marquinita!",
                    Contenido = "Desde hace más de 20 años, en el corazón de Cajamarca, horneamos con amor productos que celebran nuestras raíces y sabores tradicionales."
                },
                new ContenidoPagina
                {
                    Seccion = "BienvenidaImagen",
                    Titulo = "Fachada de Panadería",
                    Contenido = "/img/Bienvenida_LM.png"
                },
                new ContenidoPagina
                {
                    Seccion = "PoliticaPrivacidad",
                    Titulo = "Política de Privacidad",
                    Contenido = "Respetamos tu información personal. Esta política detalla cómo protegemos tus datos y garantizamos tu privacidad."
                }
            );

            SaveChanges();
        }
        public void SeedRoles()
        {
            if (Roles.Any()) return;

            Roles.AddRange(
                new Rol { Nombre = "Administrador" },
                new Rol { Nombre = "Cliente" },
                new Rol { Nombre = "Empleado" }
            );

            SaveChanges();
        }
    }
}
