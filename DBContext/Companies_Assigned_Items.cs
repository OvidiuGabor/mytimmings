namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Companies_Assigned_Items")]
    public partial class Companies_Assigned_Items
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(15)]
        public string Company_ID { get; set; }

        [StringLength(550)]
        public string Productive_Actions { get; set; }

        [StringLength(550)]
        public string Non_Productive_Actions { get; set; }
        [StringLength(550)]
        public string Other { get; set; }

        [StringLength(500)]
        public string Leaves_Types { get; set; }
    }
}
