using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class OwnController : Controller
    {
        private ApplicationUserManager _userManager;
        public string GetUserId(IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        public bool EnviarCorreo(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("notificaciones@empresasadoc.com");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                //contenedor alrernativo para el visor html
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html);

                //se crea recurso vinculado con la imagen a incustar estos seran fijos(los mismos) en todos los correos
                LinkedResource img = new LinkedResource(@"C:\FormatosCorreo\images\linkedin@2x.png", MediaTypeNames.Image.Jpeg);
                img.ContentId = "linkedinpng";
                LinkedResource img2 = new LinkedResource(@"C:\FormatosCorreo\images\facebook.png", MediaTypeNames.Image.Jpeg);
                img2.ContentId = "facebookpng";

                //añadiendo los recursos vinculados.
                htmlView.LinkedResources.Add(img);
                htmlView.LinkedResources.Add(img2);

                //se añade el contenedor alternativo al correo
                mail.AlternateViews.Add(htmlView);

                //Se configura el cliente smtp
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "192.168.16.30";
                smtp.Port = 25;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("notificaciones@empresasadoc.com", "");
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool EnviarCorreo(List<string> to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("notificaciones@empresasadoc.com");
            foreach(var item in to)
            {
                mail.To.Add(item);
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                //contenedor alrernativo para el visor html
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Html);

                //se crea recurso vinculado con la imagen a incustar estos seran fijos(los mismos) en todos los correos
                LinkedResource img = new LinkedResource(@"C:\FormatosCorreo\images\linkedin@2x.png", MediaTypeNames.Image.Jpeg);
                img.ContentId = "linkedinpng";
                LinkedResource img2 = new LinkedResource(@"C:\FormatosCorreo\images\facebook.png", MediaTypeNames.Image.Jpeg);
                img2.ContentId = "facebookpng";

                //añadiendo los recursos vinculados.
                htmlView.LinkedResources.Add(img);
                htmlView.LinkedResources.Add(img2);

                //se añade el contenedor alternativo al correo
                mail.AlternateViews.Add(htmlView);

                //Se configura el cliente smtp
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "192.168.16.30";
                smtp.Port = 25;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("notificaciones@empresasadoc.com", "");
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }
}