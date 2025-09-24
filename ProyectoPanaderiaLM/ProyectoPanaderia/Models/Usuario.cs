using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPanaderia.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Required, DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Required]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        public ICollection<Compra> Compras { get; set; }
        public ICollection<DireccionEnvio> Direcciones { get; set; }
        public ICollection<Carrito> Carritos { get; set; }
        public ICollection<HistorialCompra> Historial { get; set; }
    }
}
