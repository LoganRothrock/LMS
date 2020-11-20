using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.MVC.DATA.EF;

namespace LMS.MVC.UI.Controllers
{
    public class EmpDetailsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: EmpDetails
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Index()
        {
            return View(db.EmpDetails.ToList());
        }

        // GET: EmpDetails/Details/5
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDetail empDetail = db.EmpDetails.Find(id);
            if (empDetail == null)
            {
                return HttpNotFound();
            }
            return View(empDetail);
        }

        // GET: EmpDetails/Create
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Create([Bind(Include = "EmpId,FirstName,LastName")] EmpDetail empDetail)
        {
            if (ModelState.IsValid)
            {
                db.EmpDetails.Add(empDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empDetail);
        }

        // GET: EmpDetails/Edit/5
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDetail empDetail = db.EmpDetails.Find(id);
            if (empDetail == null)
            {
                return HttpNotFound();
            }
            return View(empDetail);
        }

        // POST: EmpDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Edit([Bind(Include = "EmpId,FirstName,LastName")] EmpDetail empDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empDetail);
        }

        // GET: EmpDetails/Delete/5
        [Authorize(Roles = "HR, Manager")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDetail empDetail = db.EmpDetails.Find(id);
            if (empDetail == null)
            {
                return HttpNotFound();
            }
            return View(empDetail);
        }

        // POST: EmpDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR, Manager")]
        public ActionResult DeleteConfirmed(string id)
        {
            EmpDetail empDetail = db.EmpDetails.Find(id);
            db.EmpDetails.Remove(empDetail);
            db.SaveChanges();
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
