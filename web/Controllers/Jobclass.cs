using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net;
using System.Net.Mail;
using System.Text;
using web.Models;
using System.Data.Entity;
using System.Net.Mime;

namespace web.Controllers
{
    public class Jobclass : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Execute(IJobExecutionContext context)
        {
            var viajes = db.Viajes.Where(v => v.FechaFin <= DateTime.Now && v.Eliminado != true).Include(v => v.LiquidacionesViaje).Include(v=>v.Usuario);
            foreach (var item in viajes)
            {
                if (item.LiquidacionesViaje == null)
                {
                    var dias = (DateTime.Now - item.FechaFin).Days;
                    if (dias < 3)
                    {
                        string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\notificacionLiquidar.html");
                        readText = readText.Replace("$$Proceso##", item.Viaje).Replace("$$fechaFin##", item.FechaFin.ToShortDateString());
                        EnviarCorreo(item.Usuario.Email, "Liquidar Proceso", readText);
                    }
                    else {
                        string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\notificacionLiquidar3.html");
                        readText = readText.Replace("$$Proceso##", item.Viaje).Replace("$$fechaFin##", item.FechaFin.ToShortDateString());
                        EnviarCorreo(item.Usuario.Email, "Liquidar Proceso", readText);
                    }
                }
                else if (item.LiquidacionesViaje.Where(l => l.Eliminado != true && l.IdEstado != Estado.Finalizado).Count() > 0)
                {
                    var dias = (DateTime.Now - item.FechaFin).Days;
                    if (dias < 3)
                    {
                        string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\notificacionLiquidar.html");
                        readText = readText.Replace("$$Proceso##", item.Viaje).Replace("$$fechaFin##", item.FechaFin.ToShortDateString());
                        EnviarCorreo(item.Usuario.Email, "Liquidar Proceso", readText);
                    }
                    else
                    {
                        string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\notificacionLiquidar3.html");
                        readText = readText.Replace("$$Proceso##", item.Viaje).Replace("$$fechaFin##", item.FechaFin.ToShortDateString());
                        EnviarCorreo(item.Usuario.Email, "Liquidar Proceso", readText);
                    }
                }
            }
        }


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
    }
}