using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.XML;
using System.Diagnostics;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    internal class DfsIndexBuilder
    {
        /// <summary>
        /// Dfs-индексация дерева
        /// </summary>
        /// <param name="treeEncoding">Объект кодировки дерева</param>
        internal static void BuildDfsIndex(TextTreeEncoding treeEncoding)
        {
            Debug.Assert(treeEncoding != null);
            SetDfsIndex(treeEncoding.Root, 0);
        }

        /// <summary>
        /// Установка dfs-индексов узлов дерева
        /// </summary>
        /// <param name="treeNode">Узел дерева</param>
        /// <param name="index">Индекс</param>
        /// <returns>Индекс</returns>
        private static int SetDfsIndex(TreeNode treeNode, int index)
        {
            treeNode.DfsIndex = index;
            int nextIndex = treeNode.DfsIndex + 1;
            if (treeNode.IsLeaf)
            {
                return nextIndex;
            }
            int rightMostIndex = nextIndex;
            foreach (TreeNode t in treeNode.Children)
            {
                rightMostIndex = SetDfsIndex(t, nextIndex);
                nextIndex = rightMostIndex;
            }
            return rightMostIndex;
        }
    }
}
