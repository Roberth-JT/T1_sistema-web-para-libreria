using System.ComponentModel.DataAnnotations;

namespace ProyectoPanaderia.ViewModels
{
    public class RegistroVM
    {
        [Required, StringLength(60)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Required, DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Required, Compare("Contraseña")]
        public string ConfirmarContraseña { get; set; }

        [Required]
        public int RolId { get; set; }
    }
}
