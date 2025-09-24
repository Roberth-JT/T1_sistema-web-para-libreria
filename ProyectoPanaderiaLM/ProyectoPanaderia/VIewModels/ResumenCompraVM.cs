using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProyectoPanaderia.ViewModels
{
    public class ResumenCompraVM
    {
        public List<ItemCarritoVM> Items { get; set; }
        [Required(ErrorMessage = "Por favor, selecciona un método de pago.")]
        [Display(Name = "Método de Pago")]
        public string MetodoPago { get; set; }
        [Required(ErrorMessage = "Por favor, ingresa la dirección de entrega.")]
        [StringLength(200)]
        [Display(Name = "Dirección de Entrega")]
        public string DireccionEntrega { get; set; }
        [StringLength(500)]
        [Display(Name = "Referencia")]
        public string Referencia { get; set; }
        public string RolComprador { get; set; }
        public decimal Total { get; set; }
    }
}
