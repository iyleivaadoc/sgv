using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using web.Models;
using static web.Controllers.ManageController;
using ClosedXML;
using ClosedXML.Excel;

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.saludo = Resourses.Strings.saludo;
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Tu contraseña ha sido cambiada."
                : message == ManageMessageId.SetPasswordSuccess ? "Tu contraseña ha sido configurada."
                : message == ManageMessageId.Error ? "Ocurrió un error."
                : "";

            ViewBag.usuario = await UserManager.FindByIdAsync(this.GetUserId(User));
            return View();
        }


     
        public ActionResult EnviarCorreo()
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("notificaciones@empresasadoc.com");
            mail.To.Add("irvinadoc@gmail.com");
            mail.Subject = "correo de prueba";
            //mail.Body = "<div class=\"well\" style=\"margin-top:30px; text-align:center;\"> <h1>Sistema de Gestión de Viáticos</h1><label><strong>Bienvenido Monica Sibrian </strong></label> <br><label><strong>Código de empleado: </strong>123456 </label><br> <label><strong>Correo electrónico: </strong>monica.sibrian@empresasadoc.com</label><br> <label><strong>Dirección a la que reporta: </strong> Finanzas</label><br><label><strong>País: El Salvador </strong>  <strong>Tel: 7733 3377</strong> </label><br></div>";
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AprobarViatico.html");
            //mail.Body = readText.Replace("$$nombre##"," Irvin Leiva");
            AlternateView htmlView =     AlternateView.CreateAlternateViewFromString(readText.Replace("$$nombre##", " Irvin Leiva"), Encoding.UTF8,MediaTypeNames.Text.Html);
            LinkedResource img = new LinkedResource(@"C:\FormatosCorreo\images\linkedin@2x.png", MediaTypeNames.Image.Jpeg);
            img.ContentId = "linkedinpng";

            LinkedResource img2 = new LinkedResource(@"C:\FormatosCorreo\images\facebook.png", MediaTypeNames.Image.Jpeg);
            img2.ContentId = "facebookpng";

            htmlView.LinkedResources.Add(img);
            htmlView.LinkedResources.Add(img2);
            mail.AlternateViews.Add(htmlView);


            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "192.168.16.30";
            smtp.Port = 25;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("notificaciones@empresasadoc.com", "");
            smtp.Send(mail);
            return RedirectToAction("Index");

        }

        public ActionResult GenerateExcel()
        {
            // Generate the workbook...
            //var workbook = ClosedXmlDemoGenerator.GenerateWorkBook();
            XLWorkbook wb = new XLWorkbook();
            var worksheet = wb.Worksheets.Add("FB60");
            worksheet.Cell(1, 1).Value = "Cuenta de Mayor";
            worksheet.Cell(1, 4).Value = "Importe Moneda";
            worksheet.Cell(1, 10).Value = "Texto";
            worksheet.Cell(1, 16).Value = "Centro Costo";
            // ... and return it to the client
            return new ExcelResult(wb, "DemoFB60");
        }

        public string GetUserId(IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }
    }
}