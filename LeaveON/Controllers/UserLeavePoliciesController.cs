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
using System.IO;
using LeaveON.Models;

namespace LeaveON.Controllers
{
  public class UserLeavePoliciesController : Controller
  {
    private LeaveONEntities db = new LeaveONEntities();

    // GET: UserLeavePolicies
    public async Task<ActionResult> Index()
    {
      //var userLeavePolicies = db.UserLeavePolicies.Include(u => u.AspNetUser);
      var userLeavePolicies = db.UserLeavePolicies;
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
      ViewBag.Employees = new SelectList(db.AspNetUsers, "Id", "UserName");
      ViewBag.LeaveTypes = new SelectList(db.LeaveTypes, "Id", "Name");
      ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");

      UserLeavePolicyViewModel userLeavePolicyViewModel = new UserLeavePolicyViewModel();
      return View(userLeavePolicyViewModel);
      //return View();
    }

    // POST: UserLeavePolicies/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Prefix = "UserLeavePolicy", Include = "Id,UserId,WeeklyOffDays,FiscalYearStart,FiscalYearEnd,FiscalYearPeriod")] UserLeavePolicy userLeavePolicy,
      [Bind(Prefix = "UserLeavePolicyDetail", Include = "LeaveTypeId,Allowed")] List<UserLeavePolicyDetail> userLeavePolicyDetail, string[] AnnualOffDays, string[] Departments, string[] Employees, string PolicyFor)
    {
      decimal maxId = db.UserLeavePolicies.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
      userLeavePolicy.Id = maxId;
      userLeavePolicy.CountryId = 1;//from which user is Login. but admin who can view all coutries there we have to user a list of country so that he choose a country
      userLeavePolicy.AnnualOffDays = string.Join(",", AnnualOffDays);
      userLeavePolicy.DepartmentPolicy = (PolicyFor == "Dep") ? true : false;
      foreach (UserLeavePolicyDetail item in userLeavePolicyDetail.ToList<UserLeavePolicyDetail>())
      {
        if (item.Allowed == null)
        {
          userLeavePolicyDetail.Remove(item);
        }
      }
      userLeavePolicy.UserLeavePolicyDetails = userLeavePolicyDetail;



      if (ModelState.IsValid)
      {
        db.UserLeavePolicies.Add(userLeavePolicy);

        int[] DepartmentsList = Array.ConvertAll(Departments, int.Parse);
        Department dep;
        foreach (int itm in DepartmentsList)
        {
          dep = db.Departments.FirstOrDefault(x => x.Id == itm);
          foreach (AspNetUser user in dep.AspNetUsers)
          {
            user.UserLeavePolicyId = userLeavePolicy.Id;
          }
        }

        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
      return View(userLeavePolicy);
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> Create(UserLeavePolicyViewModel userLeavePolicyViewModel)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    db.UserLeavePolicies.Add(userLeavePolicyViewModel.userLeavePolicy);
    //    await db.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //  }

    //  ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicyViewModel.userLeavePolicy.UserId);
    //  return View(userLeavePolicyViewModel.userLeavePolicy);
    //}

    // GET: UserLeavePolicies/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ViewBag.Employees = new SelectList(db.AspNetUsers, "Id", "UserName");
      ViewBag.LeaveTypes = new SelectList(db.LeaveTypes, "Id", "Name");
      //always remember viewbag name should not be as model name. other wise probelm. if same multilist will not show selected values
      ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");

      UserLeavePolicyViewModel userLeavePolicyViewModel = new UserLeavePolicyViewModel();

      UserLeavePolicy userLeavePolicy = await db.UserLeavePolicies.FindAsync(id);

      userLeavePolicyViewModel.userLeavePolicy = userLeavePolicy;
      userLeavePolicyViewModel.userLeavePolicyDetail = userLeavePolicy.UserLeavePolicyDetails.AsQueryable<UserLeavePolicyDetail>();
      userLeavePolicyViewModel.departments = db.Departments.Where(x => x.CountryId == 1).AsQueryable<Department>();//TODO Convert 1 to current user country variable
                                                                                                              //userLeavePolicyViewModel.departments= depFilterd;
      IQueryable<AspNetUser> usersFilterd = db.AspNetUsers.Where(x => x.UserLeavePolicyId == id);


      foreach (AspNetUser usr in usersFilterd)
      {
        //depFilterd = db.Departments.Where(x => x.Id == usr.DepartmentId).Distinct<Department>().ToList<Department>();
      }
      //List<int> SelectedDeps = new List<int>(new int[] { 1,2 });
      List<string> SelectedDeps = new List<string>();
      List<string> SelectedEmps = new List<string>();
      SelectedDeps = usersFilterd.Select(p => p.DepartmentId.ToString()).Distinct<string>().ToList<string>();
      SelectedEmps = usersFilterd.Select(p => p.Id).Distinct<string>().ToList<string>();

      ViewBag.SelectedDepartments = SelectedDeps;
      ViewBag.SelectedEmployees = SelectedEmps;
      if (userLeavePolicy.DepartmentPolicy == true)
      {
        ViewBag.DepStatus = true;
        ViewBag.EmpStatus = false;
      }
      else
      {
        ViewBag.DepStatus = false;
        ViewBag.EmpStatus = true;
      }

      if (userLeavePolicy == null)
      {
        return HttpNotFound();
      }
      //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
      //return View(userLeavePolicy);

      return View(userLeavePolicyViewModel);

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
      //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Hometown", userLeavePolicy.UserId);
      return View(userLeavePolicy);
    }
    [HttpPost]
    public ActionResult UploadFiles()
    {
      if (Request.Files.Count > 0)
      {
        try
        {
          HttpFileCollectionBase postedFiles = Request.Files;
          HttpPostedFileBase postedFile = postedFiles[0];
          List<AnnualLeaveModel> annualLeaves = new List<AnnualLeaveModel>();
          string filePath = string.Empty;
          if (postedFile != null)
          {
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);

            //Read the contents of CSV file.
            string csvData = System.IO.File.ReadAllText(filePath);
            int cntr = 0;

            //Execute a loop over the rows.
            foreach (string row in csvData.Split('\n'))
            {
              cntr += 1;
              if (!string.IsNullOrEmpty(row) && cntr > 1)
              {
                annualLeaves.Add(new AnnualLeaveModel
                {
                  Id = Convert.ToInt32(row.Split(',')[0]),
                  LeaveDate = row.Split(',')[1],
                  Description = row.Split(',')[2]
                });
              }
            }
          }


          //for (int i = 0; i < files.Count; i++)
          //{
          //  string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
          //  string filename = Path.GetFileName(Request.Files[i].FileName);

          //  HttpPostedFileBase file = files[i];
          //  string fname;
          //  if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
          //  {
          //    string[] testfiles = file.FileName.Split(new char[] { '\\' });
          //    fname = testfiles[testfiles.Length - 1];
          //  }
          //  else
          //  {
          //    fname = file.FileName;
          //  }

          //  fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
          //  file.SaveAs(fname);
          //}

          //return Json("File Uploaded Successfully!");
          ViewBag.AnnualLeaves = annualLeaves.OrderBy(i => i.Id).ToList();
          return PartialView("_DisplayAnnualLeaves");
        }
        catch (Exception ex)
        {
          return Json("Error occurred. Error details: " + ex.Message);
        }
      }
      else
      {
        return Json("No files selected.");
      }
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
