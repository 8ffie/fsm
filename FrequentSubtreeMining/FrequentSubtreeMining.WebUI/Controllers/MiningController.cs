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
        List<XMLNode> NodeList = null;

        // GET: Mining
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(int minSize, int maxSize, double support)
        {
            //if (NodeList != null)
            //{
                try
                {
                    NodeList = XMLReader.ReadXMLDocument("test2.xml");
                    SearchResult result = SubtreeMiner.Mine(NodeList, support, minSize, maxSize);
                    ResultViewModel viewModel = new ResultViewModel()
                    {
                        Results = result.ToStrings()
                    };
                    return PartialView("~/Views/Mining/MiningResult.cshtml", viewModel);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            //}
            return View("~/Views/Mining/Index.cshtml");
        }

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