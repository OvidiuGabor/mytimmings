namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Leaves")]
    public partial class Leave
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal RequestId { get; set; }

        [StringLength(10)]
        public string UserId { get; set; }

        [StringLength(25)]
        public string Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public decimal TotalDays { get; set; }
    }
}
