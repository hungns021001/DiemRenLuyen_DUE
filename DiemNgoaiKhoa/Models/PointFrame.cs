using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class PointFrame
    {
        [Key]
        public int Id { get; set; }

        public int ItemDetailId { get; set; }

        public ItemDetail ItemDetail { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int MaxPoint { get; set; }

        public virtual ICollection<Point> Points { get; set;}
    }

    public class PointFrameRequest
    {
        public int ItemDetailId { get; set; }

        public string Name { get; set; }

        public int MaxPoint { get; set; }

    }
}
