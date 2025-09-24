using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPanaderia.Models
{
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompraId { get; set; }

        [ForeignKey("CompraId")]
        public Compra Compra { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required, Range(1, 100)]
        public int Cantidad { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal PrecioTotal { get; set; }
    }
}
