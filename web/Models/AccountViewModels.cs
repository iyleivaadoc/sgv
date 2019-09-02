using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name ="User Name")]
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage ="El email es requerido")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage ="El usuario es requerido.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellidos { get; set; }
        //[Required]
        [StringLength(100, ErrorMessage = "La {0} debe por lo menos tener {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        //[Compare("Password", ErrorMessage = "La contraseña no coincide.")]
        public string ConfirmPassword { get; set; }

        public string RoleName { get; set; }
        [Display(Name = "Número de teléfono"),Required(ErrorMessage = "El número de teléfono es requerido.")]
        public string PhoneNumber { get; set; }
        [Display(Name ="Código de empleado"), Required(ErrorMessage ="El código de empleado es requerido.")]
        public string CodigoEmpleado { get; set; }
        [Display(Name ="Cargo/Jerarquía")]
        public Cargo Cargo { get; set; }
        [Display(Name ="Dirección reporta")]
        public int IdDepartamento { get; set; }
        [Display(Name ="Centro de costos asociado"), Required(ErrorMessage = "El centro de costos es requerido.")]
        public string CentroCosto { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
