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
    public class LessonViewsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: LessonViews
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var lessonViews = db.LessonViews.Include(l => l.EmpDetail).Include(l => l.Lesson);
            return View(lessonViews.ToList());
        }

        // GET: LessonViews/Details/5
        [Authorize(Roles = "Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            return View(lessonView);
        }

        // GET: LessonViews/Create
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName");
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle");
            return View();
        }

        // POST: LessonViews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create([Bind(Include = "LessonViewId,EmpId,LessonId,DateViewed")] LessonView lessonView)
        {
            if (ModelState.IsValid)
            {
                db.LessonViews.Add(lessonView);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", lessonView.EmpId);
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            return View(lessonView);
        }

        // GET: LessonViews/Edit/5
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", lessonView.EmpId);
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            return View(lessonView);
        }

        // POST: LessonViews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "LessonViewId,EmpId,LessonId,DateViewed")] LessonView lessonView)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lessonView).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", lessonView.EmpId);
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonId", "LessonTitle", lessonView.LessonId);
            return View(lessonView);
        }

        // GET: LessonViews/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            return View(lessonView);
        }

        // POST: LessonViews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            LessonView lessonView = db.LessonViews.Find(id);
            db.LessonViews.Remove(lessonView);
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
