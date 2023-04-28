using System.ComponentModel.DataAnnotations;

namespace DiemNgoaiKhoa.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ItemDetail> ItemDetails { get; set;}
    }

    public class ItemRequest
    {
        public string Name { get; set; }
    }
}
