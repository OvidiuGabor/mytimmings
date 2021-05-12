namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Main_Data")]
    public partial class Main_Data
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(10)]
        public string userID { get; set; }

        public DateTime CurrentDate { get; set; }

        public DateTime Status_Start_Time { get; set; }

        public DateTime? Status_End_Time { get; set; }

        [StringLength(100)]
        public string Current_Status { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ProjectID { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }
    }
}
