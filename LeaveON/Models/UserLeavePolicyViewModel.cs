using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class UserLeavePolicyViewModel
  {

    public List<AnnualLeaveModel> AnnualLeaves { get; set; }
    public UserLeavePolicy UserLeavePolicies { get; set; }

  }

}
