namespace DBContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mytimngs_admin.Leave_Requests")]
    public partial class Leave_Requests
    {
        [Column(TypeName = "numeric")]
        public decimal Id { get; set; }

        [StringLength(25)]
        public string RequestType { get; set; }

        [StringLength(10)]
        public string Requestor { get; set; }

        [StringLength(10)]
        public string Approver { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestEndDate { get; set; }

        public bool? Approved { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ApprovedDate { get; set; }

        [StringLength(500)]
        public string RequestorComments { get; set; }

        [StringLength(500)]
        public string ApproverComments { get; set; }
    }
}
