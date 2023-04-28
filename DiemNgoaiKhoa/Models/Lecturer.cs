using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Lecturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        public int GenderId { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public int Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        public virtual ICollection<Class> Classes { get; set; }  

    }

    public class LecturerRequest
    {
        public string Fullname { get; set; }

        public DateTime Birthday { get; set; }

        public int GenderId { get; set; }

        public int Phone { get; set; }

        public string Address { get; set; }

        public int AccountId { get; set; }

    }
}
