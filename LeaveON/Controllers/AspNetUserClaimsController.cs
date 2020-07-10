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
  public class AspNetUserClaimsController : Controller
  {
    private LeaveONEntities db = new LeaveONEntities();

    // GET: AspNetUserClaims
    public async Task<ActionResult> Index()
    {
      ViewBag.Employees = new SelectList(db.AspNetUsers.OrderBy(x=>x.UserName), "Id", "UserName");
      //ViewBag.LeaveTypes = new SelectList(db.LeaveTypes, "Id", "Name");
      ViewBag.Departments = new SelectList(db.Departments.OrderBy(x=>x.Name), "Id", "Name");
      var aspNetUserClaims = db.AspNetUserClaims.Include(a => a.AspNetUser);
      return View(await aspNetUserClaims.ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Index([Bind(Include = "Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    {
      if (ModelState.IsValid)
      {
        aspNetUserClaim.ClaimType = db.Departments.FirstOrDefault(x => x.Id.ToString() == aspNetUserClaim.ClaimValue).Name;
        db.AspNetUserClaims.Add(aspNetUserClaim);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", aspNetUserClaim.UserId);
      return View(aspNetUserClaim);
    }
    //DeleteRight
    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<ActionResult> DeleteRight([Bind(Include = "Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    public async Task<ActionResult> DeleteClaim(int ClaimId)
    {
      AspNetUserClaim aspNetUserClaim = await db.AspNetUserClaims.FindAsync(ClaimId);
      db.AspNetUserClaims.Remove(aspNetUserClaim);
      await db.SaveChangesAsync();
      return RedirectToAction("Index");
    }
    // POST: AspNetUserClaims/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      AspNetUserClaim aspNetUserClaim = await db.AspNetUserClaims.FindAsync(id);
      db.AspNetUserClaims.Remove(aspNetUserClaim);
      await db.SaveChangesAsync();
      return RedirectToAction("Index");
    }
    // GET: AspNetUserClaims/Create
    public ActionResult Create()
    {
      ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown");
      return View();
    }

    // POST: AspNetUserClaims/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    {
      if (ModelState.IsValid)
      {
        db.AspNetUserClaims.Add(aspNetUserClaim);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", aspNetUserClaim.UserId);
      return View(aspNetUserClaim);
    }

    // GET: AspNetUserClaims/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AspNetUserClaim aspNetUserClaim = await db.AspNetUserClaims.FindAsync(id);
      if (aspNetUserClaim == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", aspNetUserClaim.UserId);
      return View(aspNetUserClaim);
    }

    // POST: AspNetUserClaims/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    {
      if (ModelState.IsValid)
      {
        db.Entry(aspNetUserClaim).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", aspNetUserClaim.UserId);
      return View(aspNetUserClaim);
    }

    // GET: AspNetUserClaims/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AspNetUserClaim aspNetUserClaim = await db.AspNetUserClaims.FindAsync(id);
      if (aspNetUserClaim == null)
      {
        return HttpNotFound();
      }
      return View(aspNetUserClaim);
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
