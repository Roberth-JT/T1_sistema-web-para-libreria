using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPanaderia.Models
{
    public class HistorialCompra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required, StringLength(30)]
        public string MetodoPago { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal MontoTotal { get; set; }
    }
}
