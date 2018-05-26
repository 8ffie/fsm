using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.XML;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    public static class Converter
    {
        /// <summary>
        /// Представление дерева в виде dfs-кодировки 
        /// </summary>
        /// <param name="dfsRepresentation">Список dfs-кодов узлов</param>
        /// <returns>Dfs-кодировка дерева</returns>
        internal static string ToDfsString(this IList<string> dfsRepresentation)
        {
            Debug.Assert(dfsRepresentation != null);
            StringBuilder sb = new StringBuilder();
            foreach (string ns in dfsRepresentation) sb.Append(string.Format("{0}{1}", ns, TextTreeEncoding.Separator));
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Представление дерева в виде dfs-кодировки с индексом
        /// </summary>
        /// <param name="tree">Кодировка дерева</param>
        /// <returns>Dfs-кодировка дерева с индексом</returns>
        internal static string ToDfsStringWithIndex(this TextTreeEncoding tree)
        {
            Debug.Assert(tree != null);

            return tree.Root.ToDfsStringWithIndex();
        }

        /// <summary>
        /// Представление дерева в виде dfs-кодировки с индексом
        /// </summary>
        /// <param name="tree">Кодировка дерева</param>
        /// <returns>Dfs-кодировка дерева с индексом</returns>
        public static string ToDfsString(this TextTreeEncoding tree)
        {
            Debug.Assert(tree != null);

            return tree.Root.ToDfsString();
        }

        /// <summary>
        /// Представление дерева в виде dfs-кодировки
        /// </summary>
        /// <param name="itn">Корень дерева</param>
        /// <returns>dfs-кодировка дерева</returns>
        internal static string ToDfsString(this TreeNode itn)
        {
            string str = itn.Tag + TextTreeEncoding.Separator.ToString(CultureInfo.InvariantCulture);
            if (itn.Children != null && itn.Children.Count > 0)
            {
                str = itn.Children.Aggregate(str, (current, c) => current + c.ToDfsString());
            }
            return str + TextTreeEncoding.UpSign.ToString() + TextTreeEncoding.Separator.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToMarkedDfsString(this TreeNode itn)
        {
            string str = (itn.Part) 
                ? "(" + itn.Tag + ")" + TextTreeEncoding.Separator.ToString(CultureInfo.InvariantCulture)
                : itn.Tag + TextTreeEncoding.Separator.ToString(CultureInfo.InvariantCulture);
            if (itn.Children != null && itn.Children.Count > 0)
            {
                str = itn.Children.Aggregate(str, (current, c) => current + c.ToMarkedDfsString());
            }
            return str + TextTreeEncoding.UpSign.ToString() + TextTreeEncoding.Separator.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Представление дерева в виде dfs-кодировки с индексом
        /// </summary>
        /// <param name="node">Корень дерева</param>
        /// <returns>dfs-кодировка дерева с индексами</returns>
        private static string ToDfsStringWithIndex(this TreeNode node)
        {
            string str = string.Format("{0}[{1}]", node.Tag, node.DfsIndex);
            if (node.Children != null && node.Children.Count > 0)
            {
                str = node.Children.Aggregate(str, (current, child) => current + child.ToDfsStringWithIndex());
            }
            return str + TextTreeEncoding.UpSign.ToString();
        }

        /// <summary>
        /// Представление дерева в виде списка dfs-кодов узлов
        /// </summary>
        /// <param name="itn">Корень дерева</param>
        /// <returns>Список dfs-кодов узлов</returns>
        internal static List<string> ToDfsStringList(this TreeNode itn)
        {
            List<string> list = new List<string> { itn.Tag };
            if (itn.Children != null && itn.Children.Count > 0)
            {
                foreach (TreeNode child in itn.Children)
                {
                    list.AddRange(ToDfsStringList(child));
                }
            }
            list.Add(TextTreeEncoding.UpSign.ToString());
            return list;
        }
    }
}
