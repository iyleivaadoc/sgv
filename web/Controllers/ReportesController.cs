using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using web.Models;
using web.ViewModels;

namespace web.Controllers
{
    public class ReportesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult LiquidacionesUsuario()
        {
            ViewBag.IdUsuario = new SelectList(db.Users.Where(u => u.Nombres != "Administrador" && u.Apellidos != "Administrador"), "Id", "FullName");
            ReporteriaVM rpt = new ReporteriaVM();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidacionesPorUsuario(ReporteriaVM rpt)
        {
            var liquidaciones = db.LiquidacionesViaje.Where(a => a.Viaje.IdUsuarioViaja == rpt.IdUsuario && a.Eliminado != true).Include(l => l.Viaje).Include(l => l.Viaje.Destino).Include(l => l.DetallesLiquidacion).OrderBy(l=>l.NoSolicitud);
            if (liquidaciones.Count() > 0)
            {
                XLWorkbook wb = new XLWorkbook();
                var worksheet = wb.Worksheets.Add("Liquidaciones");
                worksheet.Range("A1:F1").Merge().Value="Histórico de liquidaciones por usuario";
                worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.Gray;
                worksheet.Range("A1:F1").Style.Font.Bold = true;
                worksheet.Range("A1:F1").Style.Font.FontSize = 20;
                worksheet.Range("A1:F1").Style.Font.FontName = "Cambria";
                var liq = liquidaciones.FirstOrDefault();
                int index =3;
                foreach(var liquidacion in liquidaciones)
                {
                    worksheet.Cell(index, 1).Value = "No de Liquidación";
                    worksheet.Cell(index, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Cell(index, 2).Value = "'" + liquidacion.NoSolicitud;
                    worksheet.Cell(index, 3).Value = "Proceso";
                    worksheet.Cell(index, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Range(worksheet.Cell(index, 4), worksheet.Cell(index, 6)).Merge().Value = liquidacion.Viaje.Viaje;
                    index++;
                    worksheet.Cell(index, 1).Value = "Destino";
                    worksheet.Cell(index, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Cell(index, 2).Value = liquidacion.Viaje.Destino.Pais;
                    worksheet.Cell(index, 3).Value = "Desde";
                    worksheet.Cell(index, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Cell(index, 4).Value = liquidacion.Viaje.FechaInicio;
                    worksheet.Cell(index, 5).Value = "Hasta";
                    worksheet.Cell(index, 5).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Cell(index, 6).Value = liquidacion.Viaje.FechaFin;
                    index+=2;
                    worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Merge().Value = "Detalles de gastos Liquidados";
                    worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Style.Fill.BackgroundColor = XLColor.Gray;
                    index++;

                    worksheet.Cell(index, 1).Value = "Fecha de gasto";
                    worksheet.Cell(index, 2).Value = "Monto";
                    worksheet.Cell(index, 3).Value = "Cuenta de gasto";
                    worksheet.Cell(index, 4).Value = "Centro de costos";
                    worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                    index++;

                    foreach (var dt in liq.DetallesLiquidacion)
                    {
                        worksheet.Cell(index, 1).Value = dt.CuentaGasto;
                        worksheet.Cell(index, 2).Value = (dt.Monto * liq.TasaCambio).ToString("#####0.00");
                        worksheet.Cell(index, 3).Value = dt.ComentariosSolicitante;
                        worksheet.Cell(index, 4).Value = liq.Viaje.Usuario.CentroCosto;
                        index++;
                    }
                    index++;
                }
                
                return new ExcelResult(wb, "Liquidaciones-" + liq.Viaje.Usuario.UserName+"-"+DateTime.Now.ToShortDateString());
            }
            else
            {
                Session["MyAlert"] = "<script type='text/javascript'>alertify.warning('El usuario seleccionado no tiene liquidaciones realizadas.');</script>";
                return RedirectToAction("LiquidacionesUsuario");
            }
        }
    }
}