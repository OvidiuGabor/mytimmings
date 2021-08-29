namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.PartialTime_Requests")]
    public partial class PartialTime_Requests
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(10)]
        public string UserId { get; set; }


        public TimeSpan RequestTime { get; set; }

        [Column(TypeName = "date")]
        public DateTime RequestDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        [Column(TypeName = "numeric")]
        public decimal ProjectId { get; set; }

        [StringLength(10)]
        public string Approver { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}
