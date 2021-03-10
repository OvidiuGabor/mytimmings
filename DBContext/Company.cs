namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Companies")]
    public partial class Company
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Country { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Phone { get; set; }

        [StringLength(100)]
        public string Representative { get; set; }
    }
}
