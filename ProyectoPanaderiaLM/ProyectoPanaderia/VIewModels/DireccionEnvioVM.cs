using System.ComponentModel.DataAnnotations;

namespace ProyectoPanaderia.ViewModels
{
    public class DireccionEnvioVM
    {
        [Required, StringLength(120)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Referencia { get; set; }

        public int UsuarioId { get; set; }
        public string Rol { get; set; }
    }
}
