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
    public class UserLeavePoliciesController : Controller
    {
        private LeaveONEntities db = new LeaveONEntities();

        // GET: UserLeavePolicies
        public async Task<ActionResult> Index()
        {
            var userLeavePolicies = db.UserLeavePolicies.Include(u => u.AspNetUser);
            return View(await userLeavePolicies.ToListAsync());
        }

        // GET: UserLeavePolicies/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicy userLeavePolicy = await db.UserLeavePolicies.FindAsync(id);
            if (userLeavePolicy == null)
            {
                return HttpNotFound();
            }
            return View(userLeavePolicy);
        }

        // GET: UserLeavePolicies/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: UserLeavePolicies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,WeeklyOffDays,AnnualOffDays,FiscalYearStart,FiscalYearEnd,FiscalYearPeriod")] UserLeavePolicy userLeavePolicy)
        {
            if (ModelState.IsValid)
            {
                db.UserLeavePolicies.Add(userLeavePolicy);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
            return View(userLeavePolicy);
        }

        // GET: UserLeavePolicies/Edit/5
        public async Task<ActionResult> Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicy userLeavePolicy = await db.UserLeavePolicies.FindAsync(id);
            if (userLeavePolicy == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
            return View(userLeavePolicy);
        }

        // POST: UserLeavePolicies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,WeeklyOffDays,AnnualOffDays,FiscalYearStart,FiscalYearEnd,FiscalYearPeriod")] UserLeavePolicy userLeavePolicy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLeavePolicy).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
            return View(userLeavePolicy);
        }

        // GET: UserLeavePolicies/Delete/5
        public async Task<ActionResult> Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicy userLeavePolicy = await db.UserLeavePolicies.FindAsync(id);
            if (userLeavePolicy == null)
            {
                return HttpNotFound();
            }
            return View(userLeavePolicy);
        }

        // POST: UserLeavePolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id)
        {
            UserLeavePolicy userLeavePolicy = await db.UserLeavePolicies.FindAsync(id);
            db.UserLeavePolicies.Remove(userLeavePolicy);
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
