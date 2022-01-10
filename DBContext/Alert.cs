namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Alerts")]
    public partial class Alert
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(50)]
        public string ManagerId { get; set; }

        [StringLength(10)]
        public string UserID { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsForEveryone { get; set; }
        public bool? Important { get; set; }
    }
}
