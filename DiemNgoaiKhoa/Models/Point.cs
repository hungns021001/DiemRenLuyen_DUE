using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Point
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }

        public int SemesterId { get; set; }

        public Semester Semester { get; set; }

        public int PointFrameId { get; set; }

        public PointFrame PointFrame { get; set; }

        public int PointStudent { get; set; }

        public int PointMonitor { get; set;}

        public int PointLecturer { get; set; }

    }

    public class PointRequest
    {
        public int StudentId { get; set; }

        public int SemesterId { get; set; }

        public int PointFrameId { get; set; }

        public int PointStudent { get; set; }

        public int PointMonitor { get; set; }

        public int PointLecturer { get; set; }

    }
}
