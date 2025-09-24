using System.ComponentModel.DataAnnotations;

namespace ProyectoPanaderia.ViewModels
{
    public class ProductoVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80)]
        public string Nombre { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.1, 1000, ErrorMessage = "Debe ingresar un precio entre 0.10 y 1000.")]
        public decimal Precio { get; set; }


        [Required(ErrorMessage = "Debes proporcionar una ruta de imagen.")]
        [StringLength(255, ErrorMessage = "La ruta es demasiado larga.")]
        [Display(Name = "Ruta de imagen (ej. /img_prod/img_001.png)")]
        public string Imagen { get; set; }


        public bool Disponible { get; set; } = true;

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }
    }
}