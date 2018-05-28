using FrequentSubtreeMining.Algorithm.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    internal static class SubtreeExtension
    {
        /// <summary>
        /// Комбинирование dfs-кодировок поддеревьев
        /// </summary>
        /// <param name="xSubtree">Dfs-кодировка одного дерева</param>
        /// <param name="ySubtree">Dfs-кодировка второго дерева</param>
        /// <returns>Dfs-кодировка нового дерева (полученного путем комбинирования)</returns>
        internal static string[] CombineDfsRepresentation(this Tree xSubtree, Tree ySubtree)
        {
            Debug.Assert(ySubtree.FirstSymbol == xSubtree.FirstSymbol, "Невозможно комбинировать данные dfs-кодировки.");

            var nodeSymbols1 = xSubtree.DfsRepresentation;
            var nodeSymbols2 = ySubtree.DfsRepresentation;
            List<string> temp = new List<string>();
            for (int i = 0; i < nodeSymbols1.Count - 1; i++)
            {
                temp.Add(nodeSymbols1[i]);
            }
            for (int i = 1; i < nodeSymbols2.Count; i++)
            {
                temp.Add(nodeSymbols2[i]);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Соединение dfs-кодировок поддеревьев
        /// </summary>
        /// <param name="doubleSubtree">Поддерево из 2 узлов</param>
        /// <param name="subtree">Поддерево с корнем в листе doubleSubtree</param>
        /// <returns>Dfs-кодировка нового дерева (полученного путем соединения)</returns>
        internal static string[] ConnectDfsRepresentation(this Tree doubleSubtree, Tree subtree)
        {
            Debug.Assert(doubleSubtree.Is2NodeTree);
            Debug.Assert(doubleSubtree.SecondSymbol == subtree.FirstSymbol, "Невозможно соединить данные dfs-кодировки.");

            var nodeSymbols2NodeTree = doubleSubtree.DfsRepresentation;
            var nodeSymbolsTree = subtree.DfsRepresentation;

            List<string> temp = new List<string> { nodeSymbols2NodeTree[0] };
            temp.AddRange(nodeSymbolsTree);
            temp.Add(nodeSymbols2NodeTree[nodeSymbols2NodeTree.Count - 1]);
            return temp.ToArray();
        }

        /// <summary>
        /// Проверка, можно ли получить на глубине новое поддерево путем комбинирования
        /// </summary>
        /// <param name="xSubtree">Первое поддерево</param>
        /// <param name="ySubtree">Второе поддерево</param>
        /// <param name="depth">Глубина</param>
        /// <returns>true, если можно получить новое поддерево путем комбинирования</returns>
        internal static bool HasNewCombineEntryAtDepth(this Tree xSubtree, Tree ySubtree, int depth)
        {
            Debug.Assert(xSubtree != null && ySubtree != null);

            if (xSubtree.FirstSymbol != ySubtree.FirstSymbol) return false;
            if (!xSubtree.ContainsDepth(depth) || !ySubtree.ContainsDepth(depth)) return false;

            foreach (TreeEntries tree in xSubtree[depth].GetTreeEntries())
            {
                if (!ySubtree.ContainsTreeAtDepth(depth, tree.TreeId)) continue;
                foreach (RootEntry rSet in tree.GetRootEntries())
                {
                    if (!ySubtree[depth][tree.TreeId].ContainsRootIndex(rSet.RootIndex)) continue;
                    foreach (SubtreeEntry entryY in ySubtree[depth][tree.TreeId][rSet.RootIndex].RightMostList)
                    {
                        if (rSet.FirstEntry.RightMostIndex < entryY.SecondIndex) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка, можно ли получить на глубине новое поддерево путем соединения
        /// </summary>
        /// <param name="doubleNodeTree">Поддерево из 2 узлов</param>
        /// <param name="tree">Поддерево</param>
        /// <param name="depth">Глубина</param>
        /// <returns>true, если можно получить новое поддерево путем соединения</returns>
        internal static bool HasNewConnectEntryAtDepth(this Tree doubleNodeTree, Tree tree, int depth)
        {
            Debug.Assert(doubleNodeTree != null && tree != null && doubleNodeTree.Is2NodeTree);

            if (doubleNodeTree.SecondSymbol != tree.FirstSymbol) return false;

            int depthConnect = depth;
            int depthToBeConnected = depthConnect + 1;

            if (!doubleNodeTree.ContainsDepth(depthConnect) || !tree.ContainsDepth(depthToBeConnected)) return false;

            foreach (TreeEntries tSet in doubleNodeTree[depthConnect].GetTreeEntries())
            {
                if (!tree.ContainsTreeAtDepth(depthToBeConnected, tSet.TreeId)) continue;
                foreach (RootEntry rSet in tSet.GetRootEntries())
                {
                    foreach (SubtreeEntry iEntry in rSet.RightMostList)
                    {
                        if (tree[depthToBeConnected][tSet.TreeId].ContainsRootIndex(iEntry.RightMostIndex))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}
