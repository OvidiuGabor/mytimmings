namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.LiveNotification")]
    public partial class LiveNotification
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(10)]
        public string UserId { get; set; }

        public DateTime? DateReceived { get; set; }

        public DateTime? DateClosed { get; set; }

        [StringLength(150)]
        public string Message { get; set; }

        [StringLength(50)]
        public string Sender { get; set; }

        [StringLength(10)]
        public string Type { get; set; }
    }
}
