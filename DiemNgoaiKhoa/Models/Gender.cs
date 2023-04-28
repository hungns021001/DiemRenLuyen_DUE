using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Student> Students { get;set; }

        public virtual ICollection<Lecturer> Lecturers { get; set; }
    }

    public class GenderRequest
    {
        public string Name { get; set; }
    }
}
