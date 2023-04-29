using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }

        public Role Roles { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Lecturer> Lecturers { get; set; }

    }
    public class AccountRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }
    }

    public class Login
    {
        [Required]
        
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        
        public string Password { get; set; }
    }
}
