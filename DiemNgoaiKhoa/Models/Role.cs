using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
    public class RoleRequest
    {
        public string Name { get; set; }
    }
}
