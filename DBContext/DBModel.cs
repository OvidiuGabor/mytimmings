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
        public virtual DbSet<Companies_Assigned_Items> Companies_Assigned_Items { get; set; }
        public virtual DbSet<User_Leaves_Status> User_Leaves_Status { get; set; }
        public virtual DbSet<User_Login_Logout> User_Login_Logout { get; set; }
        public virtual DbSet<Leave_Requests> Leave_Requests { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<LiveNotification> LiveNotifications { get; set; }
        public virtual DbSet<PartialTime_Requests> PartialTime_Requests { get; set; }
        public virtual DbSet<Notification_Comunication_History> Notification_Comunication_History { get; set; }
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<Public_Holidays> Public_Holidays { get; set; }
        public virtual DbSet<Leaves_Saved_Not_Submited> Leaves_Saved_Not_Submited { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Leaves_Saved_Not_Submited>()
               .Property(e => e.Id)
               .HasPrecision(18, 0);

            modelBuilder.Entity<Alert>()
               .Property(e => e.Id)
               .HasPrecision(18, 0);

            modelBuilder.Entity<Public_Holidays>()
                .Property(e => e.Holiday_Id)
                .HasPrecision(18, 0);

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
                .Property(e => e.Project_ID)
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

            modelBuilder.Entity<Companies_Assigned_Items>()
               .Property(e => e.Id)
               .HasPrecision(18, 0);

            modelBuilder.Entity<User_Leaves_Status>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Leaves_Status>()
                .Property(e => e.Year)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Leaves_Status>()
                .Property(e => e.Entitled)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Leaves_Status>()
                .Property(e => e.Accrued)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Leaves_Status>()
                .Property(e => e.Carried_Over)
                .HasPrecision(18, 0);

            modelBuilder.Entity<User_Login_Logout>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Leave_Requests>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Leave>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Leave>()
                .Property(e => e.RequestId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<LiveNotification>()
               .Property(e => e.ID)
               .HasPrecision(18, 0);

            modelBuilder.Entity<PartialTime_Requests>()
              .Property(e => e.Id)
              .HasPrecision(18, 0);

            modelBuilder.Entity<PartialTime_Requests>()
                .Property(e => e.ProjectId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Notification_Comunication_History>()
              .Property(e => e.Id)
              .HasPrecision(18, 0);

            modelBuilder.Entity<Notification_Comunication_History>()
                .Property(e => e.NotificationId);
        }

        private static string GetConnectionString(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            
        }
    }
}
