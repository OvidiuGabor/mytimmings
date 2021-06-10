namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Users")]
    public partial class User
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(8)]
        public string ID { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [StringLength(10)]
        public string ManagerID { get; set; }

        [StringLength(100)]
        public string Password { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        [StringLength(100)]
        public string PhotoPath { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool isUser { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool isManager { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool isAdmin { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool isDeveloper { get; set; }
    }
}
