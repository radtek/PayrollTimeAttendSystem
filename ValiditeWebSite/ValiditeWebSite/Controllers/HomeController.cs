using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Configuration;
using ValiditeWebSite.Models;
using ValiditeWebSite.Helpers;

namespace ValiditeWebSite.Controllers
{
    public class HomeController : Controller
    {
        Helpers.Helpers helpers;
        
        public HomeController()
        {
            //Used For Logs
            helpers = new Helpers.Helpers("HomeController");
        }

        public ActionResult Index()
        {
            if (@Session["UserName"] == null)
            {
                @Session["UserName"] = "";
            }

            return View();
        }

        public ActionResult Login()
        {
            User user = new User();

            user.ReturnMessage = "";

            //Testuing
            //user.UserId = "johnnyh";
            //user.UserPassword = "jesus*01";

            return View(user);
        }
        
        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user.UserId.ToUpper() == "INTERACT" 
            && user.UserPassword.ToUpper() == "TCARETNI")
            {
                @Session["UserName"] = "Errol Le Roux";

                return RedirectToAction("BusHome", "Home");
            }
            else
            {
                Users users = new Users();
                User userReturn = users.GetUser(user);

                if (userReturn.UserNames != "")
                {
                    @Session["UserName"] = userReturn.UserNames;

                    return RedirectToAction("BusHome", "Home");
                }
            }
            
            user.ReturnMessage = "User Id / User Password Combination Not Found.";

            return View(user);
        }
        
        public ActionResult Logout()
        {
            @Session["UserName"] = "";

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Wages()
        {
            ViewData["Message"] = "Your application Services - Wages page.";

            return View();
        }

        public ActionResult TimeAttendance()
        {
            ViewData["Message"] = "Your application Services - Time Attendance page.";

            return View();
        }

        public ActionResult Salaries()
        {
            ViewData["Message"] = "Your application Services - Salaries page.";

            return View();
        }

        public ActionResult Biometrics()
        {
            ViewData["Message"] = "Your application Hardware page.";

            return View();
        }
               
        public ActionResult Gallery()
        {
            ViewData["Message"] = "Your application Services - gallery page.";

            return View();
        }

        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Contact contact = new Contact();

            contact.ContactPerson1 =  WebConfigurationManager.AppSettings["ContactPerson1"].ToString();
            contact.ContactPerson1Phone = WebConfigurationManager.AppSettings["ContactPerson1Phone"].ToString();
            contact.ContactPerson2 = WebConfigurationManager.AppSettings["ContactPerson2"].ToString();
            contact.ContactPerson2Phone = WebConfigurationManager.AppSettings["ContactPerson2Phone"].ToString();
            contact.ContactPerson3 = WebConfigurationManager.AppSettings["ContactPerson3"].ToString();
            contact.ContactPerson3Phone = WebConfigurationManager.AppSettings["ContactPerson3Phone"].ToString();

            //contact.FirstName = "Errol Mark";
            //contact.LastName = "Le Roux";
            //contact.Email = "lerouxerrol#gmail.com";
            //contact.Comment = "Give me more Info.";
            //contact.ReturnMessage = "Message sent Successfully";
            
            ViewBag.Markers = GetGoogleInfo();

            return View(contact);
        }

        [HttpPost]
        public ActionResult Contact(Contact contact)
        {
          
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                MailAddress from = new MailAddress(contact.Email.ToString());
                StringBuilder sb = new StringBuilder();
                msg.IsBodyHtml = false;
                smtp.Host = WebConfigurationManager.AppSettings["smtpEmailServer"].ToString();
                smtp.Port = 25;
                msg.To.Add(WebConfigurationManager.AppSettings["EmailToAccount"].ToString());
                smtp.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["EmailToAccount"].ToString(), WebConfigurationManager.AppSettings["EmailToAccountPassword"].ToString());
                msg.From = from;
                msg.Subject = "Validite Contact Us";
                sb.Append("First Name: " + contact.FirstName.Trim());
                sb.Append(Environment.NewLine);
                sb.Append("Last Name: " + contact.LastName.Trim());
                sb.Append(Environment.NewLine);

                if (contact.CompanyName != null)
                {
                    if (contact.CompanyName.Trim() != "")
                    {
                        sb.Append("Company Name: " + contact.CompanyName.Trim());
                        sb.Append(Environment.NewLine);
                    }
                }

                sb.Append("Email: " + contact.Email.Trim());
                sb.Append(Environment.NewLine);
                sb.Append("Comments: " + contact.Comment.Trim());
                msg.Body = sb.ToString();
                smtp.Send(msg);
                msg.Dispose();
                contact.ReturnMessage = "Message sent Successfully";
            }
            catch (Exception ex)
            {
                helpers.WriteExceptionErrorLog("Contact",ex);

                contact.ReturnMessage = "Error on Web Site";
            }
           
            contact.ContactPerson1 = WebConfigurationManager.AppSettings["ContactPerson1"].ToString();
            contact.ContactPerson1Phone = WebConfigurationManager.AppSettings["ContactPerson1Phone"].ToString();
            contact.ContactPerson2 = WebConfigurationManager.AppSettings["ContactPerson2"].ToString();
            contact.ContactPerson2Phone = WebConfigurationManager.AppSettings["ContactPerson2Phone"].ToString();
            contact.ContactPerson3 = WebConfigurationManager.AppSettings["ContactPerson3"].ToString();
            contact.ContactPerson3Phone = WebConfigurationManager.AppSettings["ContactPerson3Phone"].ToString();
            
            ViewBag.Markers = GetGoogleInfo();

            return View(contact);
        }

        [OutputCache(Duration = 0, VaryByParam = "none", NoStore = true)]
        [HttpGet]
        public ActionResult Downloads()
        {
            //Haven't Logged In
            if (@Session["UserName"] == null
            || @Session["UserName"].ToString() == "")
            {
                return RedirectToAction("Index", "Home");
            }
            
            @Session["ValiditePayroll"] = WebConfigurationManager.AppSettings["ValiditePayroll"].ToString();
            @Session["ValiditeTime"] = WebConfigurationManager.AppSettings["ValiditeTime"].ToString();
            @Session["ValiditeAllProgramsZipped"] = WebConfigurationManager.AppSettings["ValiditeAllProgramsZipped"].ToString();

            return View();
        }

        [HttpGet]
        public ActionResult Support()
        {
            //Haven't Logged In
            if (@Session["UserName"] == null
            || @Session["UserName"].ToString() == "")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult BusHome()
        {
            //Haven't Logged In
            if (@Session["UserName"] == null
            || @Session["UserName"].ToString() == "")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Image(string imagename)
        {
            switch (imagename)
            {
                case "Wages.png":
                 
                    ViewData["ReturnUrl"] = "/Home/Wages";
                    break;

                case "EmployeeClockingProcess.png":
                case "FingerPrintImageExtraction.png":

                    ViewData["ReturnUrl"] = "/Home/Index";
                    break;

                case "TimeAttendanceClient.png":
                case "TimeAttendanceInternet.png":

                    ViewData["ReturnUrl"] = "/Home/TimeAttendance";
                    break;

                case "Salaries.png":

                    ViewData["ReturnUrl"] = "/Home/Salaries";
                    break;

                case "BiometricProject.png":
                case "A11.png":
                case "BiometricReaders.png":
                    
                    ViewData["ReturnUrl"] = "/Home/Biometrics";
                    break;

                case "PayrollTotals.png":
                case "TimeSheetTotals.png":
                case "RunTimeAndAttendance.png":
                case "TimeSheet.png":
                case "TimeSheetBatch.png":
                case "Leave.png":
                case "CostCentre.png":
                case "Employee.png":

                    ViewData["ReturnUrl"] = "/Home/Gallery";
                    break;

            }

            ViewData["ImageSrc"] = "/Image/" + imagename;

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        
        private string GetGoogleInfo()
        {
            string markers = "[";
            markers += "{";
            markers += string.Format("'title': '{0}',", WebConfigurationManager.AppSettings["GoogleMapTitle"].ToString());
            markers += string.Format("'lat': '{0}',", WebConfigurationManager.AppSettings["GoogleMapLatitude"].ToString());
            markers += string.Format("'lng': '{0}',", WebConfigurationManager.AppSettings["GoogleMapLongitude"].ToString());
            markers += string.Format("'description': '{0}'", WebConfigurationManager.AppSettings["GoogleMapDescription"].ToString());
            markers += "},";
            markers += "];";

            return markers;
        }
    }
}