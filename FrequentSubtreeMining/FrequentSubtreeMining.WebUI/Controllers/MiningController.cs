using FrequentSubtreeMining.Algorithm.XML;
using FrequentSubtreeMining.WebUI.ViewModels;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrequentSubtreeMining;
using System.Text;
using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm;

namespace FrequentSubtreeMining.WebUI.Controllers
{
    public class MiningController : Controller
    {
        // GET: Mining
        [HttpGet]
        public ViewResult Index()
        {
            int minSize = 2;
            int maxSize = 10;
            double support = 0.3;
            MiningViewModel model = new MiningViewModel()
            {
                MinimumNodeNumber = minSize,
                MaximumNodeNumber = maxSize,
                Support = support
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult Index(MiningViewModel model)
        {
            try
            {
                string path = Path.Combine(Server.MapPath("~/Content/files"), Path.GetFileName(model.FileName));
                List<XMLNode> NodeList = XMLReader.ReadXMLDocument(path);
                SearchResult result = SubtreeMiner.Mine(NodeList, model.Support, model.MinimumNodeNumber, model.MaximumNodeNumber);
                ResultViewModel viewModel = new ResultViewModel()
                {
                    Results = result.ToStrings()
                };
                return Json("OK");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
            }
            return Json(" not OK");
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var upload = Request.Files[0];
            if (upload != null)
            {
                // получаем имя файла
                string path = Path.Combine(Server.MapPath("~/Content/files"), Path.GetFileName(upload.FileName));
                // string fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Files в проекте
                upload.SaveAs(path);
                string[] text = GetDocumentText(path);
                return Json(new { fileName = upload.FileName, textContent = text });
            }
            return Json(null);
        }

        //[HttpPost]
        //public ActionResult Upload1(HttpPostedFileBase fileInput)
        //{
        //    if (fileInput != null)
        //    {
        //        // получаем имя файла
        //        string fileName = System.IO.Path.GetFileName(fileInput.FileName);
        //        // сохраняем файл в папку Files в проекте
        //        fileInput.SaveAs(Server.MapPath("~/Content/files/" + fileName));
        //    }
        //    return RedirectToAction("Index");
        //}


        //public ActionResult Search(int minSize, int maxSize, double support)
        //{
        //    //if (NodeList != null)
        //    //{
        //        try
        //        {
        //            NodeList = XMLReader.ReadXMLDocument("test2.xml");
        //            SearchResult result = SubtreeMiner.Mine(NodeList, support, minSize, maxSize);
        //            ResultViewModel viewModel = new ResultViewModel()
        //            {
        //                Results = result.ToStrings()
        //            };
        //            return PartialView("~/Views/Mining/MiningResult.cshtml", viewModel);
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //        }
        //    //}
        //    return View("~/Views/Mining/Index.cshtml");
        //}

        //[HttpPost]
        //public ActionResult file(HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //        try
        //        {
        //            string path = Path.GetFileName(file.FileName);
        //            NodeList = XMLReader.ReadXMLDocument(path);
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //        }
        //    else
        //    {
        //        ViewBag.Message = "You have not specified a file.";
        //    }
        //    return View("~/Views/Mining/Index.cshtml");
        //}

        [HttpPost]
        public ActionResult file(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/files"),
                    Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    DocumentViewModel model = new DocumentViewModel()
                    {
                        Text = GetDocumentText(path)
                    };
                    return View("~/Views/Mining/DocumentText.cshtml", model);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("~/Views/Mining/Index.cshtml");
        }

        private string[] GetDocumentText(string filename)
        {
            List<string> stringList = new List<string>();
            using (StreamReader fs = new StreamReader(filename))
            {
                while (!fs.EndOfStream)
                {
                    stringList.Add(fs.ReadLine());
                }
            }
            return stringList.ToArray();
        }
    }
}