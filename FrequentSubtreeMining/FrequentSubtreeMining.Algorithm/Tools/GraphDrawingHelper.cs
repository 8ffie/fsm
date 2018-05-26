using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    public class GraphNode
    {
        public string label;
        public int depth;
        public int x;
        public int y;
        public int r;
        public GraphNode parent;
        public List<GraphNode> children;


        public static int SortByTag(GraphNode name1, GraphNode name2)
        {
            return name1.label.CompareTo(name2.label);
        }
    }

    public class GraphNodeN
    {
        public string label;
        public int depth;
        public int x;
        public int y;
        public int r;
    }


    public class GraphDrawingHelper
    {
        public static int GetMaxChildNumber(List<GraphNode> nodes)
        {
            int maxChildCount = 0;
            foreach (var n1 in nodes)
            {
                if (n1.children.Count > maxChildCount)
                {
                    maxChildCount = n1.children.Count;
                }
            }
            return maxChildCount;
        }

        public static void GetLines(GraphNode node, ref List<line> oldList)
        {
            if (node.children.Count != 0)
            {
                foreach (var child in node.children)
                {
                    line newLine = new line()
                    {
                        x1 = node.x,
                        y1 = node.y,
                        x2 = child.x,
                        y2 = child.y
                    };
                    oldList.Add(newLine);
                    GetLines(child, ref oldList);
                }
            }
        }

        public class line
        {
            public int x1;
            public int x2;
            public int y1;
            public int y2;
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

        public static void GetListWithCoordinates(List<GraphNode> nodes, int maxDepth, int maxChildCount, int radius)
        {
            Dictionary<int, List<int>> depthCoordinates = GetDepthCoordinates(nodes, maxDepth, maxChildCount, radius);
            GraphNode root = nodes.Find(x => x.depth == 1);
            List<int> currentRoute = new List<int>();
            for (int i = 0; i < maxDepth; i++)
            {
                currentRoute.Add(0);
            }
            GetCoordinate(root, depthCoordinates, ref currentRoute);
        }

        private static void GetCoordinate(GraphNode node, Dictionary<int, List<int>> depthCoordinates, ref List<int> currentRoute)
        {
            if (node.children.Count == 0)
            {
                node.x = depthCoordinates[node.depth][currentRoute[node.depth - 1]];
                currentRoute[node.depth - 1]++;
            }
            else
            {
                //node.children.Sort(GraphNode.SortByTag);
                foreach (GraphNode child in node.children)
                {
                    GetCoordinate(child, depthCoordinates, ref currentRoute);
                }
                node.x = depthCoordinates[node.depth][currentRoute[node.depth - 1]];
                currentRoute[node.depth - 1]++;
            }
        }

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

        public static List<GraphNode> GetNodes(string treeEncoding)
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
                    int childrenCount = (nodeParent == null) ? 0 : nodeParent.children.Count;
                    GraphNode node = new GraphNode()
                    {
                        depth = depth,
                        label = token,
                        r = radius,
                        x = -1,
                        y = 3 * radius * depth,
                        parent = nodeParent,
                        children = new List<GraphNode>()
                    };
                    if (currentNodeStack.Count > 0)
                    {
                        currentNodeStack.Peek().children.Add(node);
                    }
                    currentNodeStack.Push(node);
                }
            }
            return results;
        }

    }
}
