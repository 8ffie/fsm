using FrequentSubtreeMining.Algorithm.XML;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm;

namespace FrequentSubtreeMining.WebUI.Controllers
{
    public class MiningController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search()
        {
            try
            {
                var form = Request.Form;
                if (form != null)
                {
                    int minSize, maxSize; double support;
                    if (!int.TryParse(form.Get("minSize"), out minSize) || !int.TryParse(form.Get("maxSize"), out maxSize) || !double.TryParse(form.Get("support").Replace('.', ','), out support))
                    {
                        return Json(new { text = "Некорректный формат параметров поиска", code = -1 });
                    }
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentType != "text/xml")
                        {
                            return Json(new { text = "Некорректный формат файла: файл должен быть формата XML", code = -1 });
                        }
                        string path = Path.Combine(Server.MapPath("~/Content/files"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        List<XMLNode> NodeList = XMLReader.ReadXMLDocument(path);
                        if (NodeList != null && NodeList.Count > 0)
                        {
                            SearchResult result = SubtreeMiner.Mine(NodeList, support, minSize, maxSize, 10);
                            return Json(new { text = result.ToStrings(), code = 0 });
                        }
                        else
                        {
                            return Json(new { text = "Некорректный формат файла: файл не содержит деревья для поиска", code = -1 });
                        }
                    }
                    else
                    {
                        return Json(new { text = "Файл не найден либо имеет некорректный формат", code = -1 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { text = "Ошибка:" + ex.Message.ToString(), code = -1 });
            }
            return Json(new { text = "Ошибка запроса: параметры поиска не могут быть получены", code = -1 });
        }

        //[HttpPost]
        //public JsonResult Upload()
        //{
        //    var upload = Request.Files[0];
        //    if (upload != null)
        //    {
        //        string path = Path.Combine(Server.MapPath("~/Content/files"), Path.GetFileName(upload.FileName));
        //        upload.SaveAs(path);
        //        string[] text = GetDocumentText(path);
        //        return Json(new { fileName = upload.FileName, textContent = text });
        //    }
        //    return Json(null);
        //}

        //private string[] GetDocumentText(string filename)
        //{
        //    List<string> stringList = new List<string>();
        //    using (StreamReader fs = new StreamReader(filename))
        //    {
        //        while (!fs.EndOfStream)
        //        {
        //            stringList.Add(fs.ReadLine());
        //        }
        //    }
        //    return stringList.ToArray();
        //}
    }
}