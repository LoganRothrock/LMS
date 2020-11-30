using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.MVC.DATA.EF
{


    public class EmpDetailMetadata
    {
        [Required(ErrorMessage = "* First Name is required")]
        [StringLength(50, ErrorMessage = "* First name cannot be greater than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "* Last Name is required")]
        [StringLength(50, ErrorMessage = "* Last name cannot be greater than 50 characters")]
        public string LastName { get; set; }
    }
    [MetadataType(typeof(EmpDetailMetadata))]
    public partial class EmpDetail{

        [Display(Name = "Employee Name")]
        public string EmpName { get { return FirstName + " " + LastName; } }

        public string EmpRole { get; set; }
    }

    public class CourseMetadata
    {
        [Required(ErrorMessage = "* Course Name is required")]
        [StringLength(200, ErrorMessage = "* Course name cannot be greater than 200 characters")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(500,ErrorMessage = "* Course description cannot be greater than 500 characters")]
        public string CourseDescription { get; set; }

        [Required(ErrorMessage = "* Is Active is requried")]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
    [MetadataType(typeof(CourseMetadata))]
    public partial class Course
    {
        public bool CompletedCourse { get; set; }
    }

    public class CourseCompletionMetadata
    {
        [Display(Name = "Day completed")]
        public System.DateTime DateCompleted { get; set; }
    }

    public class LessonMetadata
    {
        [Required(ErrorMessage = "* Lesson name is required")]
        [StringLength(200,ErrorMessage = "* Lesson name cannot be greater than 200 characters")]
        [Display(Name = "Lesson Name")]
        public string LessonTitle { get; set; }

        [StringLength(300, ErrorMessage = "* Introduction cannot be greater than 300 characters")]
        public string Introduction { get; set; }

        [StringLength(250, ErrorMessage = "* Video URL cannot be greater than 300 characters")]
        public string VideoURL { get; set; }

        [StringLength(100, ErrorMessage = "* PDF file name cannot be greater than 100 characters")]
        public string PdfFilename { get; set; }

        [Required(ErrorMessage = "* Requried")]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson
    {
        public bool CompletedLesson { get; set; }
    }
    //Add already viewed custom prop for lessons

    public class LessonViewMetadata
    {
        [Display(Name = "Day Viewed")]
        public System.DateTime DateViewed { get; set; }
    }
}
