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

namespace LeaveON.Controllers
{
  //[Authorize]
  [Authorize(Roles = "Admin,Manager")]
  public class LeavesResponseController : Controller
  {
    private LeaveONEntities db = new LeaveONEntities();

    // GET: Leaves
    public async Task<ActionResult> Index()
    {
      //var leaves = db.Leaves.Include(l => l.LeaveType).Include(l => l.UserLeavePolicy);
      var leaves = db.Leaves.Include(l => l.LeaveType);
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
      ViewBag.LineManagers = new SelectList(db.AspNetUsers, "Id", "UserName");
      ViewBag.UserName = "LoggedIn User";
      return View();
    }

    // POST: Leaves/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,UserId,LeaveTypeId,Reason,StartDate,EndDate,TotalDays,EmergencyContact,LineManager1Id,LineManager2Id")] Leave leave)
    {
      leave.DateCreated = DateTime.UtcNow;
      if (ModelState.IsValid)
      {
        db.Leaves.Add(leave);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.LeaveTypeId = new SelectList(db.LeaveTypes, "Id", "Name", leave.LeaveTypeId);
      //ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", leave.UserLeavePolicyId);
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
