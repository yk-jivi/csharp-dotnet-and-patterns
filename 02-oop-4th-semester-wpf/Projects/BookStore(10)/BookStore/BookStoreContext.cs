using System.Data.Entity;

namespace BookStore
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext() : base("name=BookStoreContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BookStoreContext>());
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Связь Книги и Категории
            modelBuilder.Entity<Book>()
                .HasOptional(b => b.CategoryEntity)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

            // Связь Заказа и Пользователя
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Связь OrderItem с Order и Book
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany()
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId);
        }
    }
}