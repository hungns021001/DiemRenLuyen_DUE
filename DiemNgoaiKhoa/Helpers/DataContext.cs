using Microsoft.EntityFrameworkCore;
using DiemNgoaiKhoa.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DiemNgoaiKhoa.Helpers
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }
        public DbSet<Role>? Role { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Class>? Classes { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemDetail> ItemDetails { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<PointFrame> PointFrames { get; set; }
        public DbSet<Lecturer> Lecturer { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach ( var foreign in modelBuilder.Model.GetEntityTypes().SelectMany( e=> e.GetForeignKeys()))
            {
                foreign.DeleteBehavior = DeleteBehavior.Restrict;   
            }
        }
    }
}
