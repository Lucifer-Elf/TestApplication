using System.ComponentModel.DataAnnotations;

namespace Servize.Domain.Model.Account
{
    public class ServizeLoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
