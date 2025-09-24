using System.ComponentModel.DataAnnotations;

namespace ProyectoPanaderia.Models
{
    public class ContenidoPagina
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Seccion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }
    }
}