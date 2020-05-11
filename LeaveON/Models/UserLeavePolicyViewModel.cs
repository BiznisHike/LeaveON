using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class UserLeavePolicyViewModel
  {

    public UserLeavePolicy userLeavePolicy { get; set; }
    public IQueryable<UserLeavePolicyDetail> userLeavePolicyDetail { get; set; }
    public IQueryable<Department> departments { get; set; }
    
    
  }

}
