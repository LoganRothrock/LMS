using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Course).Where(l => l.IsActive == true);
            return View(lessons.ToList());
        }
        [Authorize(Roles = "HR")]
        public ActionResult InactiveIndex()
        {
            var lessons = db.Lessons.Include(l => l.Course).Where(l => l.IsActive == false);
            return View(lessons.ToList());
        }
        [Authorize(Roles = "Employee")]
        public ActionResult EmpLessons(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string currentUserID = User.Identity.GetUserId();
            var lessonView = db.LessonViews.Where(e => e.EmpId == currentUserID);
            var lessons = db.Lessons.Where(c => c.CourseId == id).Where(l => l.IsActive == true);

            foreach (var lesson in lessons)
            {
                foreach (var lessonV in lessonView)
                {
                    if (lesson.LessonId == lessonV.LessonId)
                    {
                        lesson.CompletedLesson = true;
                    }
                }
            }
            var Course = db.Courses.Where(c => c.CourseId == id);
            ViewBag.CurrentCourse = Course.SingleOrDefault().CourseName;
            ViewBag.LessonView = lessonView;
            ViewBag.User = currentUserID;
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        [Authorize(Roles = "Employee, HR")]
        public ActionResult Details(int? id, bool? cl)
        {
            string userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            if (lesson.VideoURL != null)
            {
                var v = lesson.VideoURL.IndexOf("v=");
                var amp = lesson.VideoURL.IndexOf("&", v);
                string vid;
                if (amp == -1)
                {
                    vid = lesson.VideoURL.Substring(v + 2);
                }
                else
                {
                    vid = lesson.VideoURL.Substring(v + 2, amp - (v - 2));
                }
                ViewBag.VideoID = vid;
            }
            if (User.IsInRole("HR"))
            {
                return View(lesson);
            }
            if (cl != true)
            {
                lesson.CompletedLesson = true;
                LessonView lessonViewed = new LessonView();
                lessonViewed.EmpId = userId;
                lessonViewed.LessonId = lesson.LessonId;
                lessonViewed.DateViewed = DateTime.Now.Date;
                db.LessonViews.Add(lessonViewed);
                db.SaveChanges();
            }
            var courseCheck = db.Courses.Where(c => c.CourseId == lesson.CourseId).FirstOrDefault();
            var lessonsViewed = db.LessonViews.Where(l => l.EmpId == userId);
            if (lessonsViewed.Where(lv => lv.Lesson.CourseId == courseCheck.CourseId).Count() + db.Lessons.Where(l => l.CourseId == courseCheck.CourseId).Where(l => l.IsActive == false).Count() == db.Lessons.Where(l => l.CourseId == courseCheck.CourseId).Count())
            {
                var coursesCompleted = db.CourseCompletions.Where(cc => cc.EmpId == userId);
                if (coursesCompleted.Where(cc => cc.CourseId == courseCheck.CourseId).Count() == 0)
                { 
                //    courseCheck.CompletedCourse = true;
                //}
                //if (courseCheck.CompletedCourse != true)
                    CourseCompletion courseCompleted = new CourseCompletion();
                    courseCompleted.EmpId = userId;
                    courseCompleted.CourseId = courseCheck.CourseId;
                    courseCompleted.DateCompleted = DateTime.Now.Date;
                    db.CourseCompletions.Add(courseCompleted);
                    db.SaveChanges();
                    #region Email to manager
                    var emp = db.EmpDetails.Where(e => e.EmpId == courseCompleted.EmpId).FirstOrDefault();
                    var course = db.Courses.Where(c => c.CourseId == courseCompleted.CourseId).FirstOrDefault();
                    string body = $"{emp.EmpName} has completed the following course {course.CourseName}";

                    MailMessage m = new MailMessage("no-reply@loganrothrock.com", "loganrothrock@gmail.com", "Completed Course", body);
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;

                    SmtpClient client = new SmtpClient("mail.loganrothrock.com");
                    client.Credentials = new NetworkCredential("no-reply@loganrothrock.com", "WorstUrchin7539!");
                    try
                    {
                        client.Send(m);
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = e.StackTrace;
                    }
                    #endregion
                }
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        [Authorize(Roles ="HR")]
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
        [Authorize(Roles = "HR")]
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
                lesson.PdfFilename = pdfLink;
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
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
