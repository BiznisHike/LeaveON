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
    
    public partial class UserLeavePolicy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserLeavePolicy()
        {
            this.AnnualOffDays = new HashSet<AnnualOffDay>();
            this.AspNetUsers = new HashSet<AspNetUser>();
            this.UserLeavePolicyDetails = new HashSet<UserLeavePolicyDetail>();
        }
    
        public decimal Id { get; set; }
        public string WeeklyOffDays { get; set; }
        public Nullable<System.DateTime> FiscalYearStart { get; set; }
        public Nullable<System.DateTime> FiscalYearEnd { get; set; }
        public string FiscalYearPeriod { get; set; }
        public Nullable<bool> DepartmentPolicy { get; set; }
        public string Description { get; set; }
        public Nullable<bool> DefaultPolicy { get; set; }
        public Nullable<int> CountryId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnnualOffDay> AnnualOffDays { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLeavePolicyDetail> UserLeavePolicyDetails { get; set; }
    }
}
