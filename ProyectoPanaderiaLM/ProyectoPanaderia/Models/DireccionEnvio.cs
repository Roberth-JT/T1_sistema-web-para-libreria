using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPanaderia.Models
{
    public class DireccionEnvio
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Referencia { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
