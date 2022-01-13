namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Leaves_Saved_Not_Submited")]
    public partial class Leaves_Saved_Not_Submited
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(10)]
        public string UserID { get; set; }

        [StringLength(50)]
        public string LeaveType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }
    }
}
