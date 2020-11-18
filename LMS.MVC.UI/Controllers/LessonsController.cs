using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.MVC.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.MVC.UI.Controllers
{
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Course).Where(l => l.IsActive == true);
            return View(lessons.ToList());
        }
        public ActionResult InactiveIndex()
        {
            var lessons = db.Lessons.Include(l => l.Course).Where(l => l.IsActive == false);
            return View(lessons.ToList());
        }
        public ActionResult EmpLessons(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string currentUserID = User.Identity.GetUserId();
            var lessonView = db.LessonViews.Where(e => e.EmpId == currentUserID);
            var lessons = db.Lessons.Include(v => v.LessonViews).Where(c => c.CourseId == id).Where(l => l.IsActive == true);
            return View(lessons.ToList());
        }
        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase lessonVideoURl, HttpPostedFileBase lessonPDFFile)
        {
            if (ModelState.IsValid)
            {
                string pdfLink = "noPDF.pdf";
                if (lessonPDFFile != null)
                {
                    pdfLink = lessonPDFFile.FileName;
                    string ext = pdfLink.Substring(pdfLink.LastIndexOf(".")).ToLower();
                    if (ext == ".pdf")
                    {
                        pdfLink = Guid.NewGuid() + ext;
                        lessonPDFFile.SaveAs(Server.MapPath("~/Content/LessonLinks/" + pdfLink));

                    }
                }
                else
                {
                    pdfLink = "noPDF.pdf";
                }
                //if (lessonVideoUrl != null)
                //{
                //    var v = lessonVideoUrl.IndexOf("v=");
                //    var amp = lessonVideoUrl.IndexOf("&", v);
                //    string vid;
                //    if (amp == -1)
                //    {
                //        vid = lessonVideoUrl.Substring(v + 2);
                //    }
                //    else
                //    {
                //        vid = lessonVideoUrl.Substring(v + 2, amp - (v + 2));
                //    }
                //    ViewBag.VideoID = vid;

                //}
                lesson.PdfFilename = pdfLink;
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase lessonVideoURl, HttpPostedFileBase lessonPDFFile)
        {
            if (ModelState.IsValid)
            {
                if (lessonPDFFile != null)
                {
                    string newPdfName = lessonPDFFile.FileName;
                    string ext = newPdfName.Substring(newPdfName.LastIndexOf(".")).ToLower();
                    if (ext == ".pdf")
                    {
                        newPdfName = Guid.NewGuid() + ext;
                        lessonPDFFile.SaveAs(Server.MapPath("~/Content/LessonLinks/" + newPdfName));
                        if (lesson.PdfFilename != null && lesson.PdfFilename != "noPDF.pdf")
                        {
                            System.IO.File.Delete(Server.MapPath("~/Content/LessonLinks/" + Session["currentImage"].ToString()));
                        }
                        lesson.PdfFilename = newPdfName;
                    }
                }
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            if (lesson.IsActive == true)
            {
                lesson.IsActive = false;
            }
            else
            {
                lesson.IsActive = true;
            }
            //db.Lessons.Remove(lesson);
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
