﻿using System.ComponentModel.DataAnnotations;

namespace ProyectoPanaderia.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Correo o Usuario")]
        public string EmailOrUsername { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
