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

        public TimeSpan? ShiftTime { get; set; }

        public TimeSpan? BreakTime { get; set; }

        public TimeSpan? TotalTime { get; set; }

    }
}
