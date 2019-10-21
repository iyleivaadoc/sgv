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
            var liquidaciones = db.LiquidacionesViaje.Where(a => a.Viaje.IdUsuarioViaja == rpt.IdUsuario && a.Eliminado != true).Include(l => l.Viaje).Include(l => l.Viaje.Origen).Include(l => l.Viaje.Destino).Include(l => l.DetallesLiquidacion).OrderBy(l=>l.NoSolicitud);
            if (liquidaciones.Count() > 0)
            {
                XLWorkbook wb = new XLWorkbook();
                var worksheet = wb.Worksheets.Add("Liquidaciones");
                //worksheet.Range("A1:F1").Merge().Value="Histórico de liquidaciones por usuario";
                //worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.Gray;
                //worksheet.Range("A1:F1").Style.Font.Bold = true;
                //worksheet.Range("A1:F1").Style.Font.FontSize = 20;
                //worksheet.Range("A1:F1").Style.Font.FontName = "Cambria";
                worksheet.Cell(1, 1).Value = "No de Liquidación";
                worksheet.Cell(1, 2).Value = "Estatus";
                worksheet.Cell(1, 3).Value = "Proceso";
                worksheet.Cell(1, 4).Value = "Origen";
                worksheet.Cell(1, 5).Value = "Destino";
                worksheet.Cell(1, 6).Value = "Desde";
                worksheet.Cell(1, 7).Value = "Hasta";
                worksheet.Cell(1, 8).Value = "Clasificación viaje";
                worksheet.Cell(1, 9).Value = "Via viaje";
                worksheet.Cell(1, 10).Value = "Fecha Gasto";
                worksheet.Cell(1, 11).Value = "Monto";
                worksheet.Cell(1, 12).Value = "Cuenta";
                worksheet.Cell(1, 13).Value = "No Cuenta";
                worksheet.Cell(1, 14).Value = "Centro de Costos";
                worksheet.Cell(1, 15).Value = "Monto Anticipo";
                worksheet.Cell(1, 16).Value = "Fecha de creación";
                worksheet.Cell(1, 17).Value = "Ultima modificación";

                var liq = liquidaciones.FirstOrDefault();
                int index =2;
                foreach(var liquidacion in liquidaciones)
                {
                    foreach(var item in liquidacion.DetallesLiquidacion)
                    {
                        worksheet.Cell(index, 1).Value = "'"+ liquidacion.NoSolicitud;
                        worksheet.Cell(index, 2).Value = liquidacion.IdEstado;
                        worksheet.Cell(index, 3).Value = liquidacion.Viaje.Viaje;
                        worksheet.Cell(index, 4).Value = liquidacion.Viaje.Origen.Pais;
                        worksheet.Cell(index, 5).Value = liquidacion.Viaje.Destino.Pais;
                        worksheet.Cell(index, 6).Value = liquidacion.Viaje.FechaInicio;
                        worksheet.Cell(index, 7).Value = liquidacion.Viaje.FechaFin;
                        worksheet.Cell(index, 8).Value = liquidacion.Viaje.ClasificacionViaje;
                        worksheet.Cell(index, 9).Value = liquidacion.Viaje.ViaViaje;
                        worksheet.Cell(index, 10).Value = item.FechaGasto;
                        worksheet.Cell(index, 11).Value = (item.Monto*liquidacion.TasaCambio).ToString("#####0.00");
                        var cu = db.CuentasGasto.Where(c => c.IdCuentaGasto == item.CuentaGasto && c.IdClasificacion == liquidacion.Viaje.ClasificacionViaje && c.CeCo == item.CentroCosto).FirstOrDefault();
                        worksheet.Cell(index, 12).Value = cu==null?"":cu.cuenta;
                        worksheet.Cell(index, 13).Value = item.CuentaGasto;
                        worksheet.Cell(index, 14).Value = item.CentroCosto;
                        worksheet.Cell(index, 15).Value = liquidacion.TotalAnticipo;
                        worksheet.Cell(index, 16).Value = liquidacion.FechaCrea;
                        worksheet.Cell(index, 17).Value = liquidacion.FechaMod;
                        index++;
                    }

                    //worksheet.Cell(index, 1).Value = "No de Liquidación";
                    //worksheet.Cell(index, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    //worksheet.Cell(index, 2).Value = "'" + liquidacion.NoSolicitud;
                    //worksheet.Cell(index, 3).Value = "Proceso";
                    //worksheet.Cell(index, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                    //worksheet.Range(worksheet.Cell(index, 4), worksheet.Cell(index, 6)).Merge().Value = liquidacion.Viaje.Viaje;
                    //index++;
                    //worksheet.Cell(index, 1).Value = "Destino";
                    //worksheet.Cell(index, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                    //worksheet.Cell(index, 2).Value = liquidacion.Viaje.Destino.Pais;
                    //worksheet.Cell(index, 3).Value = "Desde";
                    //worksheet.Cell(index, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                    //worksheet.Cell(index, 4).Value = liquidacion.Viaje.FechaInicio;
                    //worksheet.Cell(index, 5).Value = "Hasta";
                    //worksheet.Cell(index, 5).Style.Fill.BackgroundColor = XLColor.Gray;
                    //worksheet.Cell(index, 6).Value = liquidacion.Viaje.FechaFin;
                    //index+=2;
                    //worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Merge().Value = "Detalles de gastos Liquidados";
                    //worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Style.Fill.BackgroundColor = XLColor.Gray;
                    //index++;

                    //worksheet.Cell(index, 1).Value = "Fecha de gasto";
                    //worksheet.Cell(index, 2).Value = "Monto";
                    //worksheet.Cell(index, 3).Value = "Cuenta de gasto";
                    //worksheet.Cell(index, 4).Value = "Centro de costos";
                    //worksheet.Range(worksheet.Cell(index, 1), worksheet.Cell(index, 6)).Style.Fill.BackgroundColor = XLColor.DarkGray;
                    //index++;

                    //foreach (var dt in liq.DetallesLiquidacion)
                    //{
                    //    worksheet.Cell(index, 1).Value = dt.CuentaGasto;
                    //    worksheet.Cell(index, 2).Value = (dt.Monto * liq.TasaCambio).ToString("#####0.00");
                    //    worksheet.Cell(index, 3).Value = dt.ComentariosSolicitante;
                    //    worksheet.Cell(index, 4).Value = liq.Viaje.Usuario.CentroCosto;
                    //    index++;
                    //}
                    
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