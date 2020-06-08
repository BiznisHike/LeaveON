using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Models;
using Microsoft.AspNet.Identity;
using LMS.Models;

namespace LeaveON.Controllers
{
  
  [Authorize(Roles = "Admin,Manager,User")]
  public class LeavesRequestController : Controller
  {
    private LeaveONEntities db = new LeaveONEntities();

    // GET: Leaves
    public async Task<ActionResult> Index()
    {
      //var leaves = db.Leaves.Include(l => l.LeaveType).Include(l => l.UserLeavePolicy);
      string LoggedInUserId = User.Identity.GetUserId();
      List<Leave> leaves1 = db.Leaves.Where(x => x.UserId == LoggedInUserId).ToList();
      IQueryable<Leave> leaves = db.Leaves.Where(x=>x.UserId== LoggedInUserId).AsQueryable<Leave>();
      int c= leaves1.Count();

      return View(await leaves.ToListAsync());
    }

    // GET: Leaves/Details/5
    public async Task<ActionResult> Details(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Leave leave = await db.Leaves.FindAsync(id);
      if (leave == null)
      {
        return HttpNotFound();
      }
      return View(leave);
    }

    // GET: Leaves/Create
    public ActionResult Create()
    {
      //ViewBag.UserId = "d0c9d0b1-d0e8-4d56-a410-72e74af3ced8";
      ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "Id", "Name");
      ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId");
      List<AspNetUser> Seniors = GetSeniorStaff();

      ViewBag.LineManagers = new SelectList(Seniors, "Id", "UserName");
      ViewBag.UserName = User.Identity.Name;//"LoggedIn User";
      //UserLeavePoliciesController UserLeavePolicies = new UserLeavePoliciesController();//.FileUploadMsgView("some string");
      //var result= UserLeavePolicies.Edit(7);
      //ViewBag.UserLeavePolicy= UserLeavePolicies.Edit(7);
      return View();
    }
    
    public List<AspNetUser> GetSeniorStaff()
    {
      List<AspNetUser> Seniors = new List<AspNetUser>();
      foreach (AspNetUser user in db.AspNetUsers)
      {
        foreach (AspNetRole role in user.AspNetRoles)
        {
          if (role.Name == "Admin" || role.Name == "Manager")
          {
            AspNetUser userFound = Seniors.Find(x => x.Id == user.Id);
            if (userFound == null)
            {
              Seniors.Add(user);
            }
          }
        }
      }
      return Seniors;
    }
    // POST: Leaves/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,UserId,LeaveTypeId,Reason,StartDate,EndDate,TotalDays,EmergencyContact,LineManager1Id,LineManager2Id")] Leave leave, string StartDate)
    {
      leave.UserId = User.Identity.GetUserId();
      leave.DateCreated = DateTime.UtcNow;
      if (ModelState.IsValid)
      {


        db.Leaves.Add(leave);
        await db.SaveChangesAsync();
        AspNetUser admin1 = db.AspNetUsers.FirstOrDefault(x => x.Id == leave.LineManager1Id);
        SendEmail.SendEmailUsingLeavON(SendEmail.LeavON_Email, SendEmail.LeavON_Password, leave.AspNetUser, admin1, "LeaveRequest");
        AspNetUser admin2 = db.AspNetUsers.FirstOrDefault(x => x.Id == leave.LineManager2Id);
        SendEmail.SendEmailUsingLeavON(SendEmail.LeavON_Email, SendEmail.LeavON_Password, leave.AspNetUser, admin2, "LeaveRequest");
        return RedirectToAction("Index");
      }

      ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
      //ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", leave.UserLeavePolicyId);


      var itmes = db.AspNetUsers.Include(x => x.AspNetRoles.Select(rl => rl.Name)).ToList();

      ViewBag.LineManagers = new SelectList(db.AspNetUsers, "Id", "UserName");
      return View(leave);
    }

    // GET: Leaves/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Leave leave = await db.Leaves.FindAsync(id);
      if (leave == null)
      {
        return HttpNotFound();
      }
      List<AspNetUser> Seniors = GetSeniorStaff();
      ViewBag.LineManagers = new SelectList(Seniors, "Id", "UserName");
      ViewBag.UserName = User.Identity.Name;//"LoggedIn User";
      ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
      //ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", leave.UserLeavePolicyId);
      return View(leave);
    }

    // POST: Leaves/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,LeaveTypeId,Reason,StartDate,EndDate,TotalDays,EmergencyContact,ResponseDate1,ResponseDate2,IsAccepted1,IsAccepted2,LineManager1Id,LineManager2Id,Remarks1,Remarks2,DateCreated,DateModified,UserLeavePolicyId")] Leave leave)
    {
      leave.DateModified = DateTime.UtcNow;
      if (ModelState.IsValid)
      {
        db.Entry(leave).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
      //ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", leave.UserLeavePolicyId);
      return View(leave);
    }

    // GET: Leaves/Delete/5
    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Leave leave = await db.Leaves.FindAsync(id);
      if (leave == null)
      {
        return HttpNotFound();
      }
      return View(leave);
    }

    // POST: Leaves/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      Leave leave = await db.Leaves.FindAsync(id);
      db.Leaves.Remove(leave);
      await db.SaveChangesAsync();
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
