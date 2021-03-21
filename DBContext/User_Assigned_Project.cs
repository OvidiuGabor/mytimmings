namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.User_Assigned_Project")]
    public partial class User_Assigned_Project
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(10)]
        public string UserID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ProjectId { get; set; }

        [StringLength(50)]
        public string ProjectName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? WorkedHours { get; set; }

        public bool? Active { get; set; }
    }
}
