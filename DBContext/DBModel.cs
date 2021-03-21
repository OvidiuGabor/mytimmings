namespace DBContext
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Configuration;

    public partial class DBModel : DbContext
    {
        public DBModel()
            : base(PasswordProtect.Decrypt.DecryptText(GetConnectionString("DBModel"), true))
        {
            
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Main_Data> Main_Data { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Params> Params { get; set; }
        public virtual DbSet<User_Settings> User_Settings { get; set; }
        public virtual DbSet<User_Assigned_Project> User_Assigned_Projects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Company>()
                .Property(e => e.Phone)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Main_Data>()
               .Property(e => e.ID)
               .HasPrecision(18, 0);

            modelBuilder.Entity<Main_Data>()
                .Property(e => e.ProjectID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Project>()
               .Property(e => e.Id)
               .HasPrecision(18, 0);

            modelBuilder.Entity<Params>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Assigned_Project>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);
        }

        private static string GetConnectionString(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            
        }
    }
}
