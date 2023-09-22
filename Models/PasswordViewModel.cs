using System.ComponentModel.DataAnnotations;

namespace Auction.Models
{
    public class PasswordViewModel
    {
        [Required]
       public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
    
        public string NewPassowrd { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
