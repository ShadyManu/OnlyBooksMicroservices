namespace AuthSrv.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<Author> UsersDetails { get; set; }
        public virtual DbSet<Book> Books { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Model Builder per l'entità Book
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Genre).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Description).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Price).IsRequired(true);
            //modelBuilder.Entity<Book>().HasOne(b => b.Author).WithMany(a => a.MyBooks).HasForeignKey(u => u.AuthorId);
            //modelBuilder.Entity<Book>().Property(b => b.Author).IsRequired(true);
        }

    }
}