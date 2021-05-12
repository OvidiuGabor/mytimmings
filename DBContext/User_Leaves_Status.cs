namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.User_Leaves_Status")]
    public partial class User_Leaves_Status
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(10)]
        public string UserId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Year { get; set; }

        [StringLength(50)]
        public string Leave_type { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Entitled { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Accrued { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Carried_Over { get; set; }
    }
}
