using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.Models
{
    internal class DepthEntries
    {
        /// <summary>
        /// Глубина
        /// </summary>
        private int Depth { get; set; }

        /// <summary>
        /// Словарь вхождений деревьев
        /// </summary>
        internal Dictionary<string, TreeEntries> TreeEntriesDictionary { get; private set; }
     
        internal TreeEntries this[string treeId]
        {
            get
            {
                return TreeEntriesDictionary.ContainsKey(treeId) ? TreeEntriesDictionary[treeId] : null;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="depth">Глубина</param>
        internal DepthEntries(int depth)
        {
            Depth = depth;
            TreeEntriesDictionary = new Dictionary<string, TreeEntries>();
        }

        /// <summary>
        /// Добавить вхождение дерева в словарь
        /// </summary>
        /// <param name="entry">Вхождение дерева</param>
        /// <returns></returns>
        internal int AddEntry(SubtreeEntry entry)
        {
            Debug.Assert(entry.Depth == Depth, "Разная глубина деревьев");
            if (!TreeEntriesDictionary.ContainsKey(entry.TreeId))
            {
                TreeEntriesDictionary.Add(entry.TreeId, new TreeEntries(entry.TreeId));
            }
            return TreeEntriesDictionary[entry.TreeId].AddEntry(entry);
        }

        /// <summary>
        /// Проверка, если ли вхождение дерева в словаре
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        internal bool ContainsEntry(SubtreeEntry entry)
        {
            return TreeEntriesDictionary.ContainsKey(entry.TreeId) && TreeEntriesDictionary[entry.TreeId].ContainsEntry(entry);
        }

        public override string ToString()
        {
            return string.Format("Глубина: {0}; число деревьев: {1}.", Depth, TreeEntriesDictionary.Count);
        }

        /// <summary>
        /// Получение списка вхождений деревьев
        /// </summary>
        /// <returns>Список вхождений деревьев</returns>
        internal List<TreeEntries> GetTreeEntries()
        {
            return TreeEntriesDictionary.Select(t => t.Value).ToList();
        }

        /// <summary>
        /// Проверка наличия дерева в словаре
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        /// <returns>true, если в словаре есть запись для дерева</returns>
        internal bool ContainsTree(string treeId)
        {
            return TreeEntriesDictionary.ContainsKey(treeId);
        }

        /// <summary>
        /// Проверка содержания индекса корня дерева в словаре
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        /// <param name="rootIndex">Индекс корня</param>
        /// <returns>true, если в словаре вхождений деревьев </returns>
        internal bool ContainsRootIndex(string treeId, int rootIndex)
        {
            return ContainsTree(treeId) && TreeEntriesDictionary[treeId].ContainsRootIndex(rootIndex);
        }
    }
}
