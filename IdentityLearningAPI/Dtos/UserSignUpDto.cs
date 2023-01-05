using System.ComponentModel.DataAnnotations;

namespace IdentityLearningAPI.Dtos
{
    public class UserSignUpDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
