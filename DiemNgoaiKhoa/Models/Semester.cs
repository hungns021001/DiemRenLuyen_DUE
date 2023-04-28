using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class SemesterRequest
    {
        public string Name { get; set; }
    }
}
