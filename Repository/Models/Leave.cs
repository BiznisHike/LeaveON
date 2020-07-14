//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Leave
    {
        public decimal Id { get; set; }
        public string UserId { get; set; }
        public int LeaveTypeId { get; set; }
        public string Reason { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<decimal> TotalDays { get; set; }
        public string EmergencyContact { get; set; }
        public Nullable<System.DateTime> ResponseDate1 { get; set; }
        public Nullable<System.DateTime> ResponseDate2 { get; set; }
        public Nullable<int> IsAccepted1 { get; set; }
        public Nullable<int> IsAccepted2 { get; set; }
        public string LineManager1Id { get; set; }
        public string LineManager2Id { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual LeaveType LeaveType { get; set; }
    }
}
