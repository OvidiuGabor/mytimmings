namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Params")]
    public partial class Params
    {
        [Column(TypeName = "numeric")]
        public decimal Id { get; set; }

        [StringLength(50)]
        public string Identifier { get; set; }

        [StringLength(50)]
        public string Param1 { get; set; }
        [StringLength(50)]
        public string Param2{ get; set; }
        [StringLength(50)]
        public string Param3 { get; set; }
        public bool Active { get; set; }
    }
}
