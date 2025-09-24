using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPanaderia.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80)]
        public string Nombre { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.1, 1000, ErrorMessage = "El precio debe estar entre 0.10 y 1000.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }


        [Required(ErrorMessage = "Debes proporcionar una ruta de imagen.")]
        [StringLength(255, ErrorMessage = "La ruta es demasiado larga.")]
        [Display(Name = "Ruta de imagen (ej. /img_prod/img_001.png)")]
        public string Imagen { get; set; }


        public bool Disponible { get; set; } = true;

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
    }
}
