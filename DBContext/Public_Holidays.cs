namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Public_Holidays")]
    public partial class Public_Holidays
    {
        [Key]
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Holiday_Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Holiday_date { get; set; }

        [StringLength(150)]
        public string Holiday_name { get; set; }

        [StringLength(50)]
        public string Holiday_country { get; set; }

        [StringLength(50)]
        public string Holiday_country_region { get; set; }
    }
}
