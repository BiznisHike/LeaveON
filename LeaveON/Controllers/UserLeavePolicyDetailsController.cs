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
    public class UserLeavePolicyDetailsController : Controller
    {
        private LeaveONEntities db = new LeaveONEntities();

        // GET: UserLeavePolicyDetails
        public async Task<ActionResult> Index()
        {
            var userLeavePolicyDetails = db.UserLeavePolicyDetails.Include(u => u.UserLeavePolicy);
            return View(await userLeavePolicyDetails.ToListAsync());
        }

        // GET: UserLeavePolicyDetails/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicyDetail userLeavePolicyDetail = await db.UserLeavePolicyDetails.FindAsync(id);
            if (userLeavePolicyDetail == null)
            {
                return HttpNotFound();
            }
            return View(userLeavePolicyDetail);
        }

        // GET: UserLeavePolicyDetails/Create
        public ActionResult Create()
        {
            ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId");
            return View();
        }

        // POST: UserLeavePolicyDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserLeavePolicyId,LeaveTypeId,Allowed,Taken")] UserLeavePolicyDetail userLeavePolicyDetail)
        {
            if (ModelState.IsValid)
            {
                db.UserLeavePolicyDetails.Add(userLeavePolicyDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", userLeavePolicyDetail.UserLeavePolicyId);
            return View(userLeavePolicyDetail);
        }

        // GET: UserLeavePolicyDetails/Edit/5
        public async Task<ActionResult> Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicyDetail userLeavePolicyDetail = await db.UserLeavePolicyDetails.FindAsync(id);
            if (userLeavePolicyDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", userLeavePolicyDetail.UserLeavePolicyId);
            return View(userLeavePolicyDetail);
        }

        // POST: UserLeavePolicyDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserLeavePolicyId,LeaveTypeId,Allowed,Taken")] UserLeavePolicyDetail userLeavePolicyDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLeavePolicyDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "UserId", userLeavePolicyDetail.UserLeavePolicyId);
            return View(userLeavePolicyDetail);
        }

        // GET: UserLeavePolicyDetails/Delete/5
        public async Task<ActionResult> Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLeavePolicyDetail userLeavePolicyDetail = await db.UserLeavePolicyDetails.FindAsync(id);
            if (userLeavePolicyDetail == null)
            {
                return HttpNotFound();
            }
            return View(userLeavePolicyDetail);
        }

        // POST: UserLeavePolicyDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id)
        {
            UserLeavePolicyDetail userLeavePolicyDetail = await db.UserLeavePolicyDetails.FindAsync(id);
            db.UserLeavePolicyDetails.Remove(userLeavePolicyDetail);
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
