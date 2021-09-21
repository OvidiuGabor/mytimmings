namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Notification_Comunication_History")]
    public partial class Notification_Comunication_History
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(500)]
        public string NotificationId { get; set; }

        public DateTime UpdatedDate { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(10)]
        public string SubmitedBy { get; set; }
    }
}
