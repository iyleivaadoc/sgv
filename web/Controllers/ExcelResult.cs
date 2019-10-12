using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class ExcelResult: ActionResult
    {
        private readonly XLWorkbook _workbook;
        private readonly string _fileName;

        public ExcelResult(XLWorkbook workbook, string fileName)
        {
            _workbook = workbook;
            _fileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument."
                                 + "spreadsheetml.sheet";
            response.AddHeader("content-disposition",
                               "attachment;filename=\"" + _fileName + ".xlsx\"");

            using (var memoryStream = new MemoryStream())
            {
                _workbook.SaveAs(memoryStream);
                memoryStream.WriteTo(response.OutputStream);
            }
            response.End();
        }
    }
}