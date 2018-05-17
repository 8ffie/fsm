using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.Tools;
using FrequentSubtreeMining.Algorithm.XML;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm
{
    public class SubtreeMiner : Algorithm
    {
        /// <summary>
        /// Поиск частых поддеревьев в XML-деревьях
        /// </summary>
        /// <param name="nodeList">Список узлов деревьев</param>
        /// <param name="support">Поддержка</param>
        /// <param name="minNodeNumber">Минимальное число узлов</param>
        /// <param name="maxNodeNumber">Максимальное число узлов</param>
        /// <returns>Список частых поддеревьев</returns>
        public static SearchResult Mine(List<XMLNode> nodeList, double support, int minNodeNumber, int maxNodeNumber)
        {
            List<string> trees = InitializeTreeEncodings(nodeList, TextTreeEncoding.Separator, TextTreeEncoding.UpSign);
            List<TextTreeEncoding> encList = new List<TextTreeEncoding>();
            foreach (string treeEncoding in trees)
            {
                TextTreeEncoding newTree = EncodingBuilder.ConvertToTextTreeEncoding(treeEncoding);
                if (newTree != null)
                {
                    Algorithm.Canonicalize(newTree);
                    DfsIndexBuilder.BuildDfsIndex(newTree);
                    encList.Add(newTree);
                }
            }
            SearchParameters searchParams = new SearchParameters(support, minNodeNumber, maxNodeNumber);
            SearchParameters.treeNumber = encList.Count;
            SubtreeMiner treeMiner = new SubtreeMiner(searchParams);
            SearchResult miningResult = treeMiner.Mine(encList);
            miningResult.SearchParams = new SearchParameters(support, minNodeNumber, maxNodeNumber);
            miningResult.FrequentSubtrees = PruneSubtrees(miningResult.FrequentSubtrees, searchParams.MinimumNodeNumber, searchParams.MaximumNodeNumber);
            return miningResult;
        }

        /// <summary>
        /// Преобразование XML-узлов в кодировки
        /// </summary>
        /// <param name="nodes">Узлы</param>
        /// <param name="separator">Разделитель узлов в кодировке</param>
        /// <param name="upSign">Знак возврата к родителю</param>
        /// <returns>Список кодировок деревьев</returns>
        private static List<string> InitializeTreeEncodings(List<XMLNode> nodes, char separator, char upSign)
        {
            int k = -1;
            List<XMLTreeEncoding> trees = new List<XMLTreeEncoding>();
            foreach (XMLNode node in nodes)
            {
                if (node.Depth == 0)
                {
                    //Корневой элемент
                }
                else if (node.Depth == 1)
                {
                    k++;
                    XMLTreeEncoding tree = new XMLTreeEncoding(k);
                    tree.Nodes.Add(node);
                    tree.Encoding = k.ToString() + separator;
                    trees.Add(tree);
                }
                else
                {
                    trees[k].Nodes.Add(node);
                }
            }
            foreach (XMLTreeEncoding tree in trees)
            {
                int currentDepth = -1;
                XMLNode root = tree.Nodes[0];
                for (int j = 0; j < tree.Nodes.Count; j++)
                {
                    XMLNode node = tree.Nodes[j];
                    int nodeDepth = node.Depth;
                    while (currentDepth >= nodeDepth)
                    {
                        tree.Encoding += (upSign.ToString() + separator);
                        currentDepth--;
                        root = root.Parent;
                    }
                    if (node.Id != root.Id)
                    {
                        node.Parent = (j == 0) ? null : root;
                        if (root.Children == null) root.Children = new List<XMLNode>();
                        root.Children.Add(node);
                        root = node;
                    }
                    tree.Encoding += (node.Tag + separator);
                    currentDepth = nodeDepth;
                }
            }
            return trees.Select(x => x.Encoding).ToList();
        }

        /// <summary>
        /// Отброс частых поддеревьев, не удовлетворяющих требованиям размеров
        /// </summary>
        /// <param name="subtreeList">Список поддеревьев</param>
        /// <param name="minNodeNumber">Минимальный размер поддерева</param>
        /// <param name="maxNodeNumber">Максимальный размер поддерева</param>
        /// <returns>Список частых поддеревьев, удовлетворяющих всем требования (поддержка и размер)</returns>
        private static List<Tree> PruneSubtrees(List<Tree> subtreeList, int minNodeNumber, int maxNodeNumber)
        {
            List<Tree> subtreesToRemove = new List<Tree>();
            foreach(Tree subtree in subtreeList)
            {
                if (subtree.Size < minNodeNumber || subtree.Size > maxNodeNumber)
                {
                    subtreesToRemove.Add(subtree);
                }
            }
            subtreeList.RemoveAll(x => subtreesToRemove.Contains(x));
            return subtreeList;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="searchParams">Параметры поиска</param>
        internal SubtreeMiner(SearchParameters searchParams) : base(searchParams) { }

        /// <summary>
        /// Формирование поддеревьев из 1 и 2 узлов
        /// </summary>
        /// <param name="treeSet">Список кодировок деревьев</param>
        /// <returns>Максимальная глубина деревьев</returns>
        protected override int Generate1NodeAnd2NodesTrees(List<TextTreeEncoding> treeSet)
        {
            int maxDepth = -1;
            foreach (TextTreeEncoding tree in treeSet)
            {
                Get1NodeAnd2NodesTrees(tree.Root, ref maxDepth);
            }
            FrequentSubtrees.SetDepth(maxDepth);
            EvaluateFrequency();
            return maxDepth;
        }

        /// <summary>
        /// Оценка частоты каждого поддерева из 1 и 2 узлов
        /// </summary>
        private void EvaluateFrequency()
        {
            foreach (Tree t1 in OneNodeTrees.Values.Where(t => t.IsFrequent))
            {
                FrequentSubtrees.AddFrequentSubtree(t1);
            }
            foreach (Tree t2 in TwoNodeTrees.Values.Where(t => t.IsFrequent))
            {
                FrequentSubtrees.AddFrequentSubtree(t2);
            }
        }

        /// <summary>
        /// Получение поддеревьев из 1 и 2 узлов
        /// </summary>
        /// <param name="tn">Корень дерева</param>
        /// <param name="maxDepth">Максимальная глубина</param>
        private void Get1NodeAnd2NodesTrees(TreeNode tn, ref int maxDepth)
        {
            if (maxDepth <= tn.Depth) maxDepth = tn.Depth;
            string treeId = tn.Tree.TreeId;
            var dfsList1Node = new[] { tn.Tag, TextTreeEncoding.UpSign.ToString() };
            var subtreeKey1Node = dfsList1Node.ToDfsString();
            if (!OneNodeTrees.ContainsKey(subtreeKey1Node))
            {
                Tree oneNodeTree = Tree.Create(dfsList1Node, false, SearchParams.Support);
                ExtendedSubtrees.AddSubtree(oneNodeTree);
                OneNodeTrees.Add(oneNodeTree.DfsString, oneNodeTree);
            }
            OneNodeTrees[subtreeKey1Node].AddEntry(SubtreeEntry.Create(treeId, tn.Depth, new[] { tn.DfsIndex }));
            if (tn.Children == null) return;
            foreach (TreeNode child in tn.Children)
            {
                var dfsList2Nodes = new[] { tn.Tag, child.Tag, TextTreeEncoding.UpSign.ToString(), TextTreeEncoding.UpSign.ToString() };
                var subtreeKey2Nodes = dfsList2Nodes.ToDfsString();
                if (!TwoNodeTrees.ContainsKey(subtreeKey2Nodes))
                {
                    Tree twoNodesTree = Tree.Create(dfsList2Nodes, true, SearchParams.Support);
                    ExtendedSubtrees.AddSubtree(twoNodesTree);
                    TwoNodeTrees.Add(twoNodesTree.DfsString, twoNodesTree);
                }
                SubtreeEntry entry = SubtreeEntry.Create(treeId, tn.Depth, new[] { tn.DfsIndex, child.DfsIndex });
                TwoNodeTrees[subtreeKey2Nodes].AddEntry(entry);
                Get1NodeAnd2NodesTrees(child, ref maxDepth);
            }
        }

        /// <summary>
        /// Комбинирование поддеревьев на заданной глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        protected override void Combine(int depth)
        {
            List<Tree> combinableSubtrees = FrequentSubtrees.GetCombinableAtDepth(depth);
            StartTraversal(combinableSubtrees, depth);
        }

        /// <summary>
        /// Обход с целью комбинирования поддеревьев
        /// </summary>
        /// <param name="combinableSubtrees">Поддеревья, которые могут участвовать в комбинировании (корень имеет 1 ребенка и находится на глубине depth)</param>
        /// <param name="depth">Глубина</param>
        private void StartTraversal(List<Tree> combinableSubtrees, int depth)
        {
            Dictionary<string, List<Tree>> dic = new Dictionary<string, List<Tree>>();
            foreach (Tree t in combinableSubtrees)
            {
                if (!dic.ContainsKey(t.FirstSymbol))
                    dic.Add(t.FirstSymbol, new List<Tree>());
                dic[t.FirstSymbol].Add(t);
            }
            var groups = dic.Select(v => v.Value);
            foreach (List<Tree> group in groups.Where(x => x.Count > 0))
            {
                foreach (Tree t in group)
                {
                    for (int y = 0; y < group.Count; y++)
                    {
                        Traversal(t, y, group, depth);
                    }
                }
            }
        }

        /// <summary>
        /// Обход с целью комбинирования поддеревьев
        /// </summary>
        /// <param name="xTree">Дерево, с которым производится комбинация</param>
        /// <param name="yIndex">Индекс дерева в списке</param>
        /// <param name="group">Список деревьев, которые можно комбинировать с xTree</param>
        /// <param name="depth">Глубина</param>
        private void Traversal(Tree xTree, int yIndex, IList<Tree> group, int depth)
        {
            Tree treeX = xTree;
            Tree treeY = group[yIndex];
            string childDfsStr = treeX.CombineDfsRepresentation(treeY).ToDfsString();
            Tree child = null;
            if (ExtendedSubtrees.AlreadyExtended(childDfsStr))
            {
                child = FrequentSubtrees.GetSubtreeAtDepth(childDfsStr, depth);
            }
            else if (treeX.HasNewCombineEntryAtDepth(treeY, depth))
            {
                child = Combine2Subtrees(treeX, treeY, depth);
            }
            if (child == null) return;
            for (int i = 0; i < group.Count; i++) Traversal(child, i, group, depth);
        }

        /// <summary>
        /// Комбинирование 2 поддеревьев
        /// </summary>
        /// <param name="xTree">Первое поддерево</param>
        /// <param name="yTree">Второе поддерево</param>
        /// <param name="depth">Глубина</param>
        /// <returns>Новое поддерево, полученное путем комбинирования</returns>
        private Tree Combine2Subtrees(Tree xTree, Tree yTree, int depth)
        {
            string[] dfsList = xTree.CombineDfsRepresentation(yTree);
            Tree child = Tree.Create(dfsList, false, SearchParams.Support);
            ExtendedSubtrees.AddSubtree(child);
            int curDepth = depth + 1;
            while (--curDepth >= 0)
            {
                if (!xTree.ContainsDepth(curDepth) || !yTree.ContainsDepth(curDepth)) continue;
                foreach (TreeEntries tSet in xTree[curDepth].GetTreeEntries())
                {
                    if (!yTree.ContainsTreeAtDepth(curDepth, tSet.TreeId)) continue;
                    foreach (RootEntry root in tSet.GetRootEntries())
                    {
                        if (!yTree.ContainsRootIndex(curDepth, tSet.TreeId, root.RootIndex)) continue;

                        SubtreeEntry xEntry = xTree.GetEntry(curDepth, tSet.TreeId, root.RootIndex);
                        SubtreeEntry yEntry = yTree.GetFirstEntryAfterSpecifiedIndex(xEntry.Depth, xEntry.TreeId, xEntry.RootIndex, xEntry.RightMostIndex);

                        if (yEntry == null) continue;

                        child.AddEntry(xEntry.Combine(yEntry));
                    }
                }
            }
            if (!child.IsFrequent) return null;
            FrequentSubtrees.AddFrequentSubtree(child);
            child.Father = xTree;
            child.Mother = yTree;
            return child;
        }

        /// <summary>
        /// Соединение поддеревьев на заданной глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        protected override void Connect(int depth)
        {
            List<Tree> connectableTrees = FrequentSubtrees.GetConnectableAtDepth(depth);
            List<Tree> treesToBeConnected = FrequentSubtrees.GetToBeConnectableAtDepth(depth + 1);
            foreach (Tree f2 in connectableTrees)
            {
                List<Tree> toBeConnected = SelectSubtreesOfSameRoot(f2.SecondSymbol, treesToBeConnected, depth + 1);
                foreach (Tree t in toBeConnected)
                {
                    string childDfsStr = f2.ConnectDfsRepresentation(t).ToDfsString();
                    if (ExtendedSubtrees.AlreadyExtended(childDfsStr)) continue;
                    if (!f2.HasNewConnectEntryAtDepth(t, depth)) continue;
                    Connect2Subtrees(f2, t, depth);
                }
            }
            PruneAfterConnection(FrequentSubtrees, SearchParams.Support, depth);
        }

        /// <summary>
        /// Проверка и удаление нечастых поддеревьев
        /// </summary>
        /// <param name="freqList">Список поддеревьев</param>
        /// <param name="support">Поддержка</param>
        /// <param name="depth">Глубина</param>
        private void PruneAfterConnection(FrequentSubtree freqList, int support, int depth)
        {
            List<Tree> freqSubtreesAtDepth = freqList.GetFrequentsAtDepth(depth + 1);
            foreach (Tree tree in freqSubtreesAtDepth)
            {
                tree.PruneAfterConnection(support, depth);
            }
            freqList.RemoveCannotBeExtended(depth + 1);
        }

        /// <summary>
        /// Соединение 2 поддеревьев
        /// </summary>
        /// <param name="t2">Поддерево из 2 узлов</param>
        /// <param name="tree">Поддерево с корнем в листе первого</param>
        /// <param name="depth">Глубина</param>
        private void Connect2Subtrees(Tree t2, Tree tree, int depth)
        {
            Debug.Assert(t2.Size == 2);

            string[] preList = t2.ConnectDfsRepresentation(tree);
            Tree child = Tree.Create(preList, true, SearchParams.Support);
            ExtendedSubtrees.AddSubtree(child);
            int depthC = depth + 1; // Глубина соединения
            while (--depthC >= 0)
            {
                if (!t2.ContainsDepth(depthC)) continue;
                int depthTree = depthC + 1;
                if (!tree.ContainsDepth(depthTree)) continue;
                foreach (TreeEntries tSet in t2[depthC].GetTreeEntries())
                {
                    if (!tree.ContainsTreeAtDepth(depthTree, tSet.TreeId)) continue;
                    foreach (RootEntry root in tSet.GetRootEntries())
                    {
                        foreach (SubtreeEntry t2Entry in root.RightMostList)
                        {
                            if (!tree[depthTree][tSet.TreeId].RootDictionary.ContainsKey(t2Entry.SecondIndex)) continue;
                            var newEntry = t2Entry.Connect(tree[depthTree][tSet.TreeId][t2Entry.SecondIndex].FirstEntry);
                            child.AddEntry(newEntry);
                        }
                    }
                }
            }
            if (!child.IsFrequent) return;
            FrequentSubtrees.AddFrequentSubtree(child);
            child.Father = t2;
            child.Mother = tree;
        }

        /// <summary>
        /// Получение списка деревьев, корень которых находится на глубине depth и имеет метку symbol
        /// </summary>
        /// <param name="symbol">Метка</param>
        /// <param name="treesToBeConnected">Список всех деревьев, которые можно присоединить на заданной глубине</param>
        /// <param name="depth">Глубина</param>
        /// <returns>Список деревьев, корень которых находится на глубине depth и имеет метку symbol</returns>
        private List<Tree> SelectSubtreesOfSameRoot(string symbol, List<Tree> treesToBeConnected, int depth)
        {
            List<Tree> subtrees = new List<Tree>();
            if (treesToBeConnected == null || treesToBeConnected.Count <= 0) return subtrees;
            subtrees.AddRange(treesToBeConnected.Where(s => s.FirstSymbol == symbol && s.AbleToBeConnected && s.ContainsDepth(depth)));
            return subtrees;
        }
    }
}
