using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FrequentSubtreeMining.Algorithm.XML
{
    public class XMLReader
    {
        /// <summary>
        /// Получение списка узлов из файла XML-документа 
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Список XML-узлов</returns>
        public static List<XMLNode> ReadXMLDocument(string fileName)
        {
            List<XMLNode> nodeList = new List<XMLNode>();
            try
            {
                Dictionary<int, string> encodingDictionary = new Dictionary<int, string>();
                XmlTextReader reader = new XmlTextReader(fileName);
                Stack<int> currentNodeIdStack = new Stack<int>();
                int currentNodeId = -1;
                int code = -1;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // Узел является элементом
                            currentNodeId++;
                            XMLNode node = new XMLNode(currentNodeId, reader.Name, reader.Depth, reader.LineNumber);
                            nodeList.Add(node);
                            if (!encodingDictionary.Values.Contains(node.Tag))
                            {
                                code++;
                                encodingDictionary.Add(code, node.Tag);
                                nodeList[currentNodeId].Code = code;
                            }
                            else
                            {
                                nodeList[currentNodeId].Code = encodingDictionary.FirstOrDefault(x => x.Value == node.Tag).Key;
                            }
                            currentNodeIdStack.Push(currentNodeId);
                            if (reader.IsEmptyElement)
                            {
                                nodeList[currentNodeId].LineNumberEnd = reader.LineNumber;
                                currentNodeIdStack.Pop();
                            }
                            break;
                        case XmlNodeType.Text: // Вывести текст в каждом элементе
                            nodeList[currentNodeId].Text = reader.Value;
                            break;
                        case XmlNodeType.EndElement: // Вывести конец элемента
                            int lastNodeId = currentNodeIdStack.Peek();
                            nodeList[lastNodeId].LineNumberEnd = reader.LineNumber;
                            currentNodeIdStack.Pop();
                            break;
                    }
                }
                reader.Dispose();
            }
            catch
            {
                throw new System.Exception("Файл имеет неверный формат");
            }
            return nodeList;
        }
    }
}
