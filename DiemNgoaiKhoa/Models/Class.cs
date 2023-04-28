using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int LecturerId { get; set; }

        public Lecturer Lecturer { get;set; }

        public virtual ICollection<Student> Students { get; set; }
    }

    public class ClassRequest
    {
        public string Name { get; set; }

        public int LecturerId { get; set; }
    }
}
