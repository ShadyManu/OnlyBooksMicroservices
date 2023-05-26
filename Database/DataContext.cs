namespace AuthSrv.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDetails> UsersDetails { get; set; }
        public virtual DbSet<Book> Books { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Model Builder per l'entità User
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(true);
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired(true);
            modelBuilder.Entity<User>().Property(u => u.PasswordSalt).IsRequired(true);
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired(true);
            modelBuilder.Entity<User>().HasOne(u => u.UserDetails)
            .WithOne()
            .HasForeignKey<User>(u => u.UserDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

            // Model Builder per l'entità UserDetails
            modelBuilder.Entity<UserDetails>().HasKey(u => u.Id);
            modelBuilder.Entity<UserDetails>().Property(u => u.Name).IsRequired(false);
            modelBuilder.Entity<UserDetails>().Property(u => u.Surname).IsRequired(false);
            modelBuilder.Entity<UserDetails>().HasIndex(u => u.Username).IsUnique(true);
            modelBuilder.Entity<UserDetails>().Property(u => u.Username).IsRequired(false);
            modelBuilder.Entity<UserDetails>().Property(u => u.Address).IsRequired(false);
            modelBuilder.Entity<UserDetails>().Property(u => u.FavouriteGenre).IsRequired(false);
            //modelBuilder.Entity<UserDetails>().Property(u => u.MyBooks).IsRequired(false);
            modelBuilder.Entity<UserDetails>().HasMany(u => u.MyBooks).WithOne(u => u.Author);

            // Model Builder per l'entità Book
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Genre).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Description).IsRequired(true);
            modelBuilder.Entity<Book>().Property(b => b.Price).IsRequired(true);
            //modelBuilder.Entity<Book>().HasOne(b => b.Author).WithMany();
            //modelBuilder.Entity<Book>().Property(b => b.Author).IsRequired(true);

        }

    }
}