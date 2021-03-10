namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Projects")]
    public partial class Project
    {
        [Column(TypeName = "numeric")]
        public decimal Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        public bool isActive { get; set; }
        [StringLength(10)]
        public string Company { get; set; }
    }
}
