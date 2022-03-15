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
        public string User_ID { get; set; }

        [StringLength(100)]
        public string User_Time_Zone { get; set; }
        public DateTime CurrentDate { get; set; }

        public DateTime Status_Start_Time { get; set; }

        public DateTime Status_End_Time { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Project_ID { get; set; }

        [StringLength(50)]
        public string Project_Name { get; set; }
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Tags { get; set; }

        [StringLength(200)]
        public string TagsColors { get; set; }
    }
}
