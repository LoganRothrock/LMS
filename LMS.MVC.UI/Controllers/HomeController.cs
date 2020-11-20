using LMS.MVC.UI.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace LMS.MVC.UI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                string body = $"{cvm.Name} has sent you the following message: <br /> {cvm.Message} <strong> from the email address:</strong> {cvm.Email}";

                MailMessage m = new MailMessage("no-reply@loganrothrock.com", "loganrothrock@gmail.com", cvm.Subject, body);
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
                return View("EmailConfirmation", cvm);
            }
            else
            {
                return View(cvm);
            }
            
        }
    }
}
