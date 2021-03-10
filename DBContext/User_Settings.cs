namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.User_Settings")]
    public partial class User_Settings
    {
        [Key]
        [StringLength(10)]
        public string ID { get; set; }
        [Column(TypeName = "numeric")]
        public decimal ShiftTime { get; set; }
        [Column(TypeName = "numeric")]
        public decimal BreakTime { get; set; }
        [Column(TypeName = "numeric")]
        public decimal TotalTime { get; set; }

    }
}
