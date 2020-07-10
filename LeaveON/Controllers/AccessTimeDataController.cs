using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TimeManagement.Models;
using Repository.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace LeaveON.Controllers
{
  public class AccessTimeDataController : Controller
  {
    private BioStarEntities dbBioStar = new BioStarEntities();
    LeaveONEntities dbLeaveOn = new LeaveONEntities();
    // GET: AccessTimeData
    public async Task<ActionResult> PersonalData(string ReqMonthYear)
    {
      //DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
      //                                 System.Globalization.CultureInfo.InvariantCulture);
      DateTime reqDate = DateTime.Now;
      if (!string.IsNullOrEmpty(ReqMonthYear))
      {
        reqDate = DateTime.ParseExact(ReqMonthYear, "MM-yyyy",
                                 System.Globalization.CultureInfo.CurrentCulture);
      }
      reqDate = reqDate.AddYears(-1);
      string userId = User.Identity.GetUserId();
      int bioStarEmpNum = dbLeaveOn.AspNetUsers.FirstOrDefault(x => x.Id == userId).BioStarEmpNum.Value;

      IQueryable<UD_TB_AccessTime_Data> topRows = dbBioStar.UD_TB_AccessTime_Data.Where(x => x.EmployeeNumber == bioStarEmpNum && ((x.Date_IN.Value.Month == reqDate.Month && x.Date_IN.Value.Year == reqDate.Year) ||
                                                                                 x.Date_OUT.Value.Month == reqDate.Month && x.Date_OUT.Value.Year == reqDate.Year)).AsQueryable<UD_TB_AccessTime_Data>();
      //return View(await db.UD_TB_AccessTime_Data.ToListAsync());
      if (string.IsNullOrEmpty(ReqMonthYear))
      {
        return View(await topRows.ToListAsync());
      }
      else
      {
        return PartialView("_PersonalData", await topRows.OrderBy(i => i.Date_IN).ToListAsync());
      }

    }

    public async Task<ActionResult> DepartmentData(string ReqMonthYear, string DepartmentId)
    {



      //DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
      //                                 System.Globalization.CultureInfo.InvariantCulture);
      DateTime reqDate;

      int intDepartmentId;
      if (!string.IsNullOrEmpty(ReqMonthYear))
      {

        intDepartmentId = int.Parse(DepartmentId);
        //user.DepartmentId;//User.Identity.GetUserId();//
        reqDate = DateTime.ParseExact(ReqMonthYear, "MM-yyyy",
                                 System.Globalization.CultureInfo.CurrentCulture);

      }
      else
      {
        //in case of empty parameters or First Time

        reqDate = DateTime.Now;
        string userId = User.Identity.GetUserId();
        intDepartmentId = dbLeaveOn.AspNetUsers.FirstOrDefault(x => x.Id == userId).DepartmentId;
        List<string> SelectedDeps = new List<string>();
        SelectedDeps.Add(intDepartmentId.ToString());
        ViewBag.SelectedDepartments = SelectedDeps;
        ViewBag.Departments = new SelectList(dbLeaveOn.Departments, "Id", "Name");

      }
      IQueryable<UD_TB_AccessTime_Data> depData = null;
      if (!string.IsNullOrEmpty(ReqMonthYear))
      {
        var identity = (ClaimsIdentity)User.Identity;
        IEnumerable<Claim> claims = identity.Claims;
        Claim claim = claims.Where(x => x.Value == DepartmentId).FirstOrDefault();

        if (claim is null) return null;


        reqDate = reqDate.AddYears(-1);
        //string userId = User.Identity.GetUserId();

        //int bioStarEmpNum = dbLeaveOn.AspNetUsers.FirstOrDefault(x => x.Id == userId).BioStarEmpNum.Value;
      
        //IQueryable<UD_TB_AccessTime_Data> allUsersData = null;
        List<AspNetUser> users = dbLeaveOn.AspNetUsers.Where(x => x.DepartmentId == intDepartmentId).ToList<AspNetUser>();

        List<int> userIds = users.Select(x => x.BioStarEmpNum.Value).ToList<int>();
        foreach (AspNetUser user in users)
        {

          depData = dbBioStar.UD_TB_AccessTime_Data.Where(x => userIds.Contains(x.EmployeeNumber.Value) && ((x.Date_IN.Value.Month == reqDate.Month && x.Date_IN.Value.Year == reqDate.Year) ||
                                                                                     x.Date_OUT.Value.Month == reqDate.Month && x.Date_OUT.Value.Year == reqDate.Year)).AsQueryable<UD_TB_AccessTime_Data>();

        }
      }
      //return View(await db.UD_TB_AccessTime_Data.ToListAsync());
      if (string.IsNullOrEmpty(ReqMonthYear))
      {
        //in case of null param or first time
        if (!(depData is null))
        {
          return View(await depData.OrderBy(i => i.Date_IN).ToListAsync());
        }
        else
        {
          return View();
        }

      }
      else
      {
        return PartialView("_DepartmentData", await depData.OrderBy(i => i.Date_IN).ToListAsync());
      }

    }

    // GET: AccessTimeData/Details/5
    public async Task<ActionResult> Details(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UD_TB_AccessTime_Data uD_TB_AccessTime_Data = await dbBioStar.UD_TB_AccessTime_Data.FindAsync(id);
      if (uD_TB_AccessTime_Data == null)
      {
        return HttpNotFound();
      }
      return View(uD_TB_AccessTime_Data);
    }

    // GET: AccessTimeData/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: AccessTimeData/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "EventLogID_IN,EventLogID_OUT,EmployeeName,EmployeeNumber,DepartmentID,DepartmentName,Date_IN,Day_IN,Time_IN,ReaderID_IN,DeviceName_IN,Date_OUT,Day_OUT,Time_OUT,ReaderID_OUT,DeviceName_OUT,Duration")] UD_TB_AccessTime_Data uD_TB_AccessTime_Data)
    {
      if (ModelState.IsValid)
      {
        dbBioStar.UD_TB_AccessTime_Data.Add(uD_TB_AccessTime_Data);
        await dbBioStar.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(uD_TB_AccessTime_Data);
    }

    // GET: AccessTimeData/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UD_TB_AccessTime_Data uD_TB_AccessTime_Data = await dbBioStar.UD_TB_AccessTime_Data.FindAsync(id);
      if (uD_TB_AccessTime_Data == null)
      {
        return HttpNotFound();
      }
      return View(uD_TB_AccessTime_Data);
    }

    // POST: AccessTimeData/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "EventLogID_IN,EventLogID_OUT,EmployeeName,EmployeeNumber,DepartmentID,DepartmentName,Date_IN,Day_IN,Time_IN,ReaderID_IN,DeviceName_IN,Date_OUT,Day_OUT,Time_OUT,ReaderID_OUT,DeviceName_OUT,Duration")] UD_TB_AccessTime_Data uD_TB_AccessTime_Data)
    {
      if (ModelState.IsValid)
      {
        dbBioStar.Entry(uD_TB_AccessTime_Data).State = EntityState.Modified;
        await dbBioStar.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(uD_TB_AccessTime_Data);
    }

    // GET: AccessTimeData/Delete/5
    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UD_TB_AccessTime_Data uD_TB_AccessTime_Data = await dbBioStar.UD_TB_AccessTime_Data.FindAsync(id);
      if (uD_TB_AccessTime_Data == null)
      {
        return HttpNotFound();
      }
      return View(uD_TB_AccessTime_Data);
    }

    // POST: AccessTimeData/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      UD_TB_AccessTime_Data uD_TB_AccessTime_Data = await dbBioStar.UD_TB_AccessTime_Data.FindAsync(id);
      dbBioStar.UD_TB_AccessTime_Data.Remove(uD_TB_AccessTime_Data);
      await dbBioStar.SaveChangesAsync();
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        dbBioStar.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
