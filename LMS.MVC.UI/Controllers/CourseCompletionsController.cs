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
    public class CourseCompletionsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: CourseCompletions
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            var courseCompletions = db.CourseCompletions.Include(c => c.Course).Include(c => c.EmpDetail);
            return View(courseCompletions.ToList());
        }
        [Authorize(Roles = "Manager")]
        public ActionResult EmpProgress()
        {
            //TODO Add a filter that only shows employee progress and not managers or HR
            var activeCourses = db.Courses.Where(c => c.IsActive == true).Count();
            var empProgress = db.EmpDetails.Include(c => c.CourseCompletions);
            ViewBag.ActiveCourses = activeCourses;
            return View(empProgress);
        }
        [Authorize(Roles = "Manager")]
        public ActionResult EmpCourseProgress(string id)
        {
            var courseCompletion = db.CourseCompletions.Where(e => e.EmpId == id);
            var courses = db.Courses.Include(v => v.CourseCompletions).Where(c => c.IsActive == true);
            var activeLessons = db.Lessons.Where(l => l.IsActive == true);
            var completedLessons = db.LessonViews.Where(l => l.EmpId == id);
            var employee = db.EmpDetails.Where(e => e.EmpId == id).FirstOrDefault();
            foreach (var course in courses)
            {
                foreach (var courseC in courseCompletion)
                {
                    if (course.CourseId == courseC.CourseId)
                    {
                        course.CompletedCourse = true;
                    }
                }
            }
            ViewBag.Employee = employee.EmpName;
            ViewBag.CompletedLessons = completedLessons;
            ViewBag.ActiveLessons = activeLessons;
            ViewBag.Emp = id;
            return View(courses);
        }
        [Authorize(Roles = "Manager")]
        public ActionResult EmpLessonProgress(string id, int? cid)
        {
            var lessonView = db.LessonViews.Where(e => e.EmpId == id);
            var lessons = db.Lessons.Where(l => l.IsActive == true).Where(l => l.CourseId == cid);
            var employee = db.EmpDetails.Where(e => e.EmpId == id).FirstOrDefault();
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
            ViewBag.Employee = employee.EmpName;
            ViewBag.LessonView = lessonView;
            return View(lessons);
        }
        [ChildActionOnly]
        [Authorize(Roles = "Manager")]
        public ActionResult RenderLessons(int? courseId, string empId)
        {
            var lessonsInCourse = db.Lessons.Where(l => l.CourseId == courseId).Where(l => l.IsActive == true);
            var completetedLessons = db.LessonViews.Where(lv => lv.EmpId == empId);
            int cl = 0;
            foreach (var lesson in lessonsInCourse)
            {
                foreach (var lessonV in completetedLessons)
                {
                    if (lesson.LessonId == lessonV.LessonId)
                    {
                        cl++;
                    }
                }
            }
            ViewBag.LessonsInCourse = lessonsInCourse.Count();
            ViewBag.CompletedLessons = cl;
            return PartialView("RenderLessons");
        }
        // GET: CourseCompletions/Details/5
        [Authorize(Roles = "Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Create
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName");
            return View();
        }

        // POST: CourseCompletions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Create([Bind(Include = "CourseCompletionId,EmpId,CourseId,DateCompleted")] CourseCompletion courseCompletion)
        {
            if (ModelState.IsValid)
            {
                db.CourseCompletions.Add(courseCompletion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", courseCompletion.EmpId);
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Edit/5
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", courseCompletion.EmpId);
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "CourseCompletionId,EmpId,CourseId,DateCompleted")] CourseCompletion courseCompletion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseCompletion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.EmpId = new SelectList(db.EmpDetails, "EmpId", "FirstName", courseCompletion.EmpId);
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Delete/5
        [Authorize(Roles = "Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            db.CourseCompletions.Remove(courseCompletion);
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
