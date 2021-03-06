﻿using System;
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
    public class CoursesController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Courses
        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View(db.Courses.Where(c => c.IsActive == true));
        }
        [Authorize(Roles = "HR")]
        public ActionResult InactiveIndex()
        {
            return View(db.Courses.Where(c => c.IsActive == false));
        }
        [Authorize(Roles = "Employee, Manager")]
        public ActionResult EmpView()
        {
            string currentUserID = User.Identity.GetUserId();
            var courseCompletion = db.CourseCompletions.Where(e => e.EmpId == currentUserID);
            var courses = db.Courses.Include(v => v.CourseCompletions).Where(c => c.IsActive == true);

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
            ViewBag.CourseComplete = courseCompletion;
            return View(db.Courses.Where(c => c.IsActive == true));

        }
        // GET: Courses/Details/5
        [Authorize(Roles = "HR")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR")]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR")]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "HR")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            if (course.IsActive == true)
            {
                course.IsActive = false;
            }
            else
            {
                course.IsActive = true;
            }
            //db.Courses.Remove(course);
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
