using FrequentSubtreeMining.Algorithm.XML;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm;
using System.Linq;
using FrequentSubtreeMining.Algorithm.Tools;
using static FrequentSubtreeMining.Algorithm.Tools.GraphDrawingHelper;

namespace FrequentSubtreeMining.WebUI.Controllers
{ 
    public class MiningController : Controller
    {
        /// <summary>
        /// GET-запрос отображения главной страницы приложения
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// Получение списка деревьев, в которых содержится заданное поддерево
        /// </summary>
        /// <param name="subtreeEncoding">Кодировка поддерева</param>
        /// <param name="allInitialTrees">Список кодировок всех деревьев</param>
        /// <returns>Список кодировок деревьев, в которых содержится заданное поддерево</returns>
        private List<string> GetInitialTreeEncodingsWithSubtree(string subtreeEncoding, List<string> allInitialTrees)
        {
            List<string> newInitialEncodings = new List<string>();
            foreach (var tree in allInitialTrees)
            {
                TreeNode markedTree = null;
                if (TreeNode.ContainsSubtree(tree, subtreeEncoding, out markedTree))
                {
                    string markedTreeEncoding = markedTree.ToMarkedDfsString();
                    newInitialEncodings.Add(markedTreeEncoding);
                }
            }
            return newInitialEncodings;
        }
       

        /// <summary>
        /// POST-запрос получения данных для выбранного из списка дерева
        /// </summary>
        [HttpPost]
        public JsonResult GetTree()
        {
            var form = Request.Form;
            var freqList = form.Get("treeEncoding").Split(',');
            if (freqList.Count() > 0)
            {
                string freq = freqList[0];
                List<string> newInitialEncodings = GetInitialTreeEncodingsWithSubtree(freq, freqList.Skip(1).ToList());             
                Dictionary<int, List<string>> treeDict = GetDepthNodesDictionary(freq);
                int maxDepth = treeDict.Keys.Max();
                List<GraphNode> nodes = GetGraphNodes(freq);
                int maxChildCount = nodes.Max(x => x.Children.Count);
                double maxWidth = Math.Pow(maxChildCount, maxDepth - 1);
                var root = nodes.Find(x => x.Depth == 1);
                root.X = 3 * root.R + (3 * root.R * ((int)maxWidth - 1) / 2);
                int screenWidth = root.X * 2;
                int screenHeight = maxDepth * 3 * root.R + 2 * root.R;
                List<GraphNodeData> res = new List<GraphNodeData>();
                GetListWithCoordinates(nodes, maxDepth, maxChildCount, root.R);
                List<Line> lineList = new List<Line>();
                GetLines(root, ref lineList);
                foreach (GraphNode n in nodes)
                {
                    res.Add(new GraphNodeData()
                    {
                        Label = n.Label,
                        X = n.X,
                        Y = n.Y,
                        R = n.R,
                        Depth = n.Depth
                    });
                }
                if (freqList.Count() > 1)
                {
                    return Json(new { nodes = res, lines = lineList, height = screenHeight, width = screenWidth, treeList = newInitialEncodings, support = (double)newInitialEncodings.Count / (freqList.Count() - 1) });
                }
                return Json(new { nodes = res, lines = lineList, height = screenHeight, width = screenWidth, treeList = newInitialEncodings });
            }
            return Json("Ошибка");
        }
      
        /// <summary>
        /// POST-запрос поиска частых поддеревьев по заданным параметрам поиска
        /// </summary>
        [HttpPost]
        public JsonResult Search()
        {
            try
            {
                var form = Request.Form;
                if (form != null)
                {
                    int minSize, maxSize/*, maxTime*/; double support;
                    if (!int.TryParse(form.Get("minSize"), out minSize) || !int.TryParse(form.Get("maxSize"), out maxSize) 
                        || !double.TryParse(form.Get("support").Replace('.', ','), out support)/* || (!int.TryParse(form.Get("maxTime"), out maxTime))*/)
                    {
                        return Json(new { text = "Некорректный формат параметров поиска", code = -1 });
                    }
                    double tempSup;
                    if (support > 1 && double.TryParse(form.Get("support"), out tempSup))
                    {
                        support = tempSup;
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
                            SearchResult result = SubtreeMiner.Mine(NodeList, support, minSize, maxSize/*, maxTime*/);
                            var docEncodings = new List<string>();
                            foreach (var tree in SearchParameters.initialTrees)
                            {
                                docEncodings.Add(tree.ToStringWithoutTreeId());
                            }
                            var resEncodings = new List<string>();
                            foreach (var tree in result.FrequentSubtrees)
                            {
                                resEncodings.Add(tree.DfsString);
                            }

                            return Json(new { text = result.ToStrings(), code = 0, doc = docEncodings, res = resEncodings });
                        }
                        else
                        {
                            return Json(new { text = "Некорректный формат документа: документ не содержит деревья для поиска", code = -1 });
                        }
                    }
                    else
                    {
                        return Json(new { text = "Документ не найден либо имеет некорректный формат", code = -1 });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { text = "Ошибка:" + ex.Message.ToString(), code = -1 });
            }
            return Json(new { text = "Ошибка запроса: параметры поиска не могут быть получены", code = -1 });
        }
    }
}