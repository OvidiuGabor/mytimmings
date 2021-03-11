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
        }


        //public static string Decrypt(string cypherString, string Securitykey, bool useHasing)
        //{
        //    byte[] keyArray;
        //    byte[] toEncryptArray = Convert.FromBase64String(cypherString);

        //    string key = Securitykey;

        //    if (useHasing)
        //    {
        //        //if hashing was used get the hash code with regards to your key  
        //        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        //        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        //        //release any resource held by the MD5CryptoServiceProvider  

        //        hashmd5.Clear();
        //    }
        //    else
        //    {
        //        keyArray = UTF8Encoding.UTF8.GetBytes(key);
        //    }

        //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //    //set the secret key for the tripleDES algorithm  
        //    tdes.Key = keyArray;
        //    //mode of operation. there are other 4 modes.   
        //    //We choose ECB(Electronic code Book)  

        //    tdes.Mode = CipherMode.ECB;
        //    //padding mode(if any extra byte added)  
        //    tdes.Padding = PaddingMode.PKCS7;

        //    ICryptoTransform cTransform = tdes.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(
        //                         toEncryptArray, 0, toEncryptArray.Length);
        //    //Release resources held by TripleDes Encryptor                  
        //    tdes.Clear();
        //    //return the Clear decrypted TEXT  
        //    return UTF8Encoding.UTF8.GetString(resultArray);
        //}


        private static string GetConnectionString(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            
        }
    }
}
