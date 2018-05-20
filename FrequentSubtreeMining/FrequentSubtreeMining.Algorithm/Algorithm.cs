using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.Tools;
using FrequentSubtreeMining.Algorithm.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm
{
    public abstract class Algorithm
    {
        /// <summary>
        /// Dfs-индексация деревьев
        /// </summary>
        /// <param name="treeList">Список деревьев</param>
        private static void BuildDfsIndex(List<TextTreeEncoding> treeList)
        {
            foreach (TextTreeEncoding tree in treeList)
            {
                DfsIndexBuilder.BuildDfsIndex(tree);
            }
        }

        /// <summary>
        /// Объект списка расширенных поддеревьев
        /// </summary>
        protected internal readonly ExtendedSubtrees ExtendedSubtrees = new ExtendedSubtrees();

        /// <summary>
        /// Объект списка частых поддеревьев
        /// </summary>
        protected internal readonly FrequentSubtrees FrequentSubtrees = new FrequentSubtrees();

        /// <summary>
        /// Словарь поддеревьев из 1 узла
        /// </summary>
        protected internal readonly Dictionary<string, Tree> OneNodeTrees = new Dictionary<string, Tree>();

        /// <summary>
        /// Словарь поддеревьев из 2 узлов
        /// </summary>
        protected internal readonly Dictionary<string, Tree> TwoNodeTrees = new Dictionary<string, Tree>();

        /// <summary>
        /// Параметры поиска
        /// </summary>
        protected internal readonly SearchParameters SearchParams;

        /// <summary>
        /// Максимальная глубина
        /// </summary>
        internal int MaxDepth { get; private set; }

        /// <summary>
        /// Поиск частых поддеревьев
        /// </summary>
        /// <param name="treeSet">Список деревьев</param>
        /// <returns>Объект результатов поиска частых поддеревьев</returns>
        internal SearchResult Mine(List<TextTreeEncoding> treeSet)
        {
            if (treeSet == null)
            {
                throw new Exception("Список деревьев пуст");
            }
            Canonicalize(treeSet);
            BuildDfsIndex(treeSet);
            Stopwatch timeCounter = Stopwatch.StartNew();
            MaxDepth = Generate1NodeAnd2NodesTrees(treeSet);
            var depth = MaxDepth - 1;
            while (depth >= 0)
            {
                Combine(depth);
                Connect(--depth);
            }
            timeCounter.Stop();
            return OutputResults(timeCounter);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="searchParams">Параметры поиска</param>
        protected Algorithm(SearchParameters searchParams)
        {
            Debug.Assert(searchParams != null);
            SearchParams = (SearchParameters)searchParams.Clone();
        }

        protected abstract int Generate1NodeAnd2NodesTrees(List<TextTreeEncoding> treeSet);

        /// <summary>
        /// Комбинирование поддеревьев на глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        protected abstract void Combine(int depth);

        /// <summary>
        /// Соединение поддеревьев на глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        protected abstract void Connect(int depth);

        /// <summary>
        /// Приведение деревьев к каноническому виду
        /// </summary>
        /// <param name="treeSet">Список деревьев</param>
        internal void Canonicalize(IEnumerable<TextTreeEncoding> treeSet)
        {
            foreach (TextTreeEncoding tree in treeSet)
            {
                Canonicalize(tree);
            }
        }

        /// <summary>
        /// Приведение дерева к каноническому виду
        /// </summary>
        /// <param name="tree">Дерево</param>
        internal static void Canonicalize(TextTreeEncoding tree)
        {
            Debug.Assert(tree != null);
            if (tree.Root != null)
                Sort(tree.Root);
        }

        /// <summary>
        /// Сортировка узлов для приведения к каноническому виду
        /// </summary>
        /// <param name="node"></param>
        private static void Sort(TreeNode node)
        {
            if (node.Children == null) return;
            foreach (var child in node.Children) Sort(child);

            node.Children.Sort(TreeNode.Compare);
        }

        /// <summary>
        /// Возврат результатов поиска частых подграфов
        /// </summary>
        /// <param name="timeCounter">Затраченное на поиск время</param>
        /// <returns>Объект результатов поиска частых подграфов</returns>
        private SearchResult OutputResults(Stopwatch timeCounter)
        {
            Tree[] frequents = FrequentSubtrees.Frequents.Values.ToArray();
            Array.Sort(frequents);

            SearchResult result = new SearchResult
            {
                TotalTimeElapsed = timeCounter.ElapsedMilliseconds,
                SearchParams = this.SearchParams,
                FrequentSubtrees = frequents.ToList()
            };
            return result;
        }

    }
}
