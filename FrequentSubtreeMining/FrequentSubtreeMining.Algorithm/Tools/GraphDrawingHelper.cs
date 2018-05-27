using FrequentSubtreeMining.Algorithm.Models;
using System;
using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    #region Вспомогательные классы для отрисовки деревьев
    public class GraphNodeData
    {
        public string Label;
        public int Depth;
        public int X;
        public int Y;
        public int R;
    }

    public class GraphNode : GraphNodeData
    {
        public GraphNode Parent { get; set; }
        public List<GraphNode> Children { get; set; }

        public static int SortByTag(GraphNode name1, GraphNode name2)
        {
            return name1.Label.CompareTo(name2.Label);
        }
    }

    public class Line
    {
        public int X1;
        public int X2;
        public int Y1;
        public int Y2;
    }
    #endregion

    public class GraphDrawingHelper
    {
        /// <summary>
        /// Получить координаты концов отрезков ребер графа
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="oldList">Список ребер графа</param>
        public static void GetLines(GraphNode node, ref List<Line> oldList)
        {
            if (node.Children.Count != 0)
            {
                foreach (GraphNode child in node.Children)
                {
                    Line newLine = new Line()
                    {
                        X1 = node.X,
                        Y1 = node.Y,
                        X2 = child.X,
                        Y2 = child.Y
                    };
                    oldList.Add(newLine);
                    GetLines(child, ref oldList);
                }
            }
        }

        /// <summary>
        /// Построение сетки с координатами деревьев 
        /// </summary>
        /// <param name="nodes">Список узлов</param>
        /// <param name="maxDepth">Максимальная глубина</param>
        /// <param name="maxChildCount">Максимальное число потомков</param>
        /// <param name="radius">Радиус узла</param>
        /// <returns>Словарь с x-координатами узлов по глубинам (ключ - глубина, значение - список x-координат узлов)</returns>
        private static Dictionary<int, List<int>> GetDepthCoordinates(List<GraphNode> nodes, int maxDepth, int maxChildCount, int radius)
        {
            Dictionary<int, List<int>> depthCoordinates = new Dictionary<int, List<int>>();
            depthCoordinates.Add(maxDepth, new List<int>());
            for (int i = 1; i <= Math.Pow(maxChildCount, maxDepth - 1); i++)
            {
                depthCoordinates[maxDepth].Add(3 * radius * i);
            }
            for (int depth = maxDepth - 1; depth > 0; depth--)
            {
                depthCoordinates.Add(depth, new List<int>());
                for (int i = 1; i <= Math.Pow(maxChildCount, depth - 1); i++)
                {
                    int val = (depthCoordinates[depth + 1][i * maxChildCount - 1] - depthCoordinates[depth + 1][(i - 1) * maxChildCount]) / 2 + depthCoordinates[depth + 1][(i - 1) * maxChildCount];
                    depthCoordinates[depth].Add(val);
                }
            }
            return depthCoordinates;
        }

        /// <summary>
        /// Получить список координат узлов графа
        /// </summary>
        /// <param name="nodes">Узлы графа</param>
        /// <param name="maxDepth">Максимальная глубина</param>
        /// <param name="maxChildCount">Максимальное число потомков</param>
        /// <param name="radius">Радиус</param>
        public static void GetListWithCoordinates(List<GraphNode> nodes, int maxDepth, int maxChildCount, int radius)
        {
            Dictionary<int, List<int>> depthCoordinates = GetDepthCoordinates(nodes, maxDepth, maxChildCount, radius);
            GraphNode root = nodes.Find(x => x.Depth == 1);
            List<int> currentRoute = new List<int>();
            for (int i = 0; i < maxDepth; i++)
            {
                currentRoute.Add(0);
            }
            GetCoordinate(root, depthCoordinates, ref currentRoute);
        }

        /// <summary>
        /// Получение координат узла графа
        /// </summary>
        private static void GetCoordinate(GraphNode node, Dictionary<int, List<int>> depthCoordinates, ref List<int> currentRoute)
        {
            if (node.Children.Count == 0)
            {
                node.X = depthCoordinates[node.Depth][currentRoute[node.Depth - 1]];
                currentRoute[node.Depth - 1]++;
            }
            else
            {
                foreach (GraphNode child in node.Children)
                {
                    GetCoordinate(child, depthCoordinates, ref currentRoute);
                }
                node.X = depthCoordinates[node.Depth][currentRoute[node.Depth - 1]];
                currentRoute[node.Depth - 1]++;
            }
        }

        /// <summary>
        /// Получение словаря узлов дерева по глубинам
        /// </summary>
        /// <param name="encoding">Кодировка дерева</param>
        /// <returns>Словарь узлов дерева по глубинам</returns>
        public static Dictionary<int, List<string>> GetDepthNodesDictionary(string encoding)
        {
            Dictionary<int, List<string>> depthNodesDict = new Dictionary<int, List<string>>();
            int key = 0;
            string[] tokens = encoding.Trim().Split(new char[] { TextTreeEncoding.Separator });
            foreach (string token in tokens)
            {
                if (token == TextTreeEncoding.UpSign.ToString())
                {
                    key--;
                }
                else
                {
                    key++;
                    if (depthNodesDict.ContainsKey(key))
                    {
                        depthNodesDict[key].Add(token);
                    }
                    else
                    {
                        List<string> nodes = new List<string>();
                        nodes.Add(token);
                        depthNodesDict.Add(key, nodes);
                    }
                }
            }
            return depthNodesDict;
        }

        /// <summary>
        /// Получение данных узлов дерева для отрисовки
        /// </summary>
        /// <param name="treeEncoding">Кодировка дерева</param>
        /// <returns>Список узлов с данными для отрисовки</returns>
        public static List<GraphNode> GetGraphNodes(string treeEncoding)
        {
            List<GraphNode> results = new List<GraphNode>();
            Stack<GraphNode> currentNodeStack = new Stack<GraphNode>();
            int depth = 0;
            int radius = 20;
            string[] tokens = treeEncoding.Split(TextTreeEncoding.Separator);
            foreach (string token in tokens)
            {
                if (token == TextTreeEncoding.UpSign.ToString())
                {
                    depth--;
                    results.Add(currentNodeStack.Peek());
                    currentNodeStack.Pop();
                }
                else
                {
                    depth++;
                    GraphNode nodeParent = (currentNodeStack.Count > 0) ? currentNodeStack.Peek() : null;
                    int childrenCount = (nodeParent == null) ? 0 : nodeParent.Children.Count;
                    GraphNode node = new GraphNode()
                    {
                        Depth = depth,
                        Label = token,
                        R = radius,
                        X = -1,
                        Y = 3 * radius * depth,
                        Parent = nodeParent,
                        Children = new List<GraphNode>()
                    };
                    if (currentNodeStack.Count > 0)
                    {
                        currentNodeStack.Peek().Children.Add(node);
                    }
                    currentNodeStack.Push(node);
                }
            }
            return results;
        }
    }
}
