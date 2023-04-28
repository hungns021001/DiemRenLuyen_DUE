using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class ItemDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }

        public virtual ICollection<PointFrame> PointFrame { get; set; }
    }

    public class ItemDetailRequest
    {
        public string Name { get; set; }

        public int ItemId { get; set; }
    }
}
