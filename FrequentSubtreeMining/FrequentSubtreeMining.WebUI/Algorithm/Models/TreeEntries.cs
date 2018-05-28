using System.Collections.Generic;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.Models
{
    internal class TreeEntries
    {
        /// <summary>
        /// Id дерева
        /// </summary>
        internal string TreeId { get; private set; }

        /// <summary>
        /// Словарь вхождений корней деревьев (ключ - индекс корня) 
        /// </summary>
        internal Dictionary<int, RootEntry> RootDictionary { get; private set; }

        internal RootEntry this[int rootIndex]
        {
            get
            {
                return RootDictionary.ContainsKey(rootIndex) ? RootDictionary[rootIndex] : null;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        internal TreeEntries(string treeId)
        {
            TreeId = treeId;
            RootDictionary = new Dictionary<int, RootEntry>();
        }

        /// <summary>
        /// Получение вхождений корней деревьев
        /// </summary>
        /// <returns></returns>
        internal List<RootEntry> GetRootEntries()
        {
            return RootDictionary.Select(r => r.Value).ToList();
        }

        /// <summary>
        /// Проверка наличия вхождения дерева 
        /// </summary>
        /// <param name="entry">Вхождение дерева</param>
        /// <returns>true, если есть вхождение дерева</returns>
        internal bool ContainsEntry(SubtreeEntry entry)
        {
            return RootDictionary.ContainsKey(entry.RootIndex) && RootDictionary[entry.RootIndex].ContainsEntry(entry);
        }

        /// <summary>
        /// Добавить вхождение поддерева 
        /// </summary>
        /// <param name="entry">Вхождение поддерева</param>
        /// <returns>1 - единственное дерево на крайнем правом пути, иначе 0</returns>
        internal int AddEntry(SubtreeEntry entry)
        {
            if (!RootDictionary.ContainsKey(entry.RootIndex))
            {
                RootDictionary.Add(entry.RootIndex, new RootEntry(TreeId, entry.Depth, entry.RootIndex));
            }
            return RootDictionary[entry.RootIndex].AddEntry(entry);
        }

        /// <summary>
        /// Проверка содержания в словаре корневых вхождений записи с заданным индексом корня
        /// </summary>
        /// <param name="rootIndex">Индекс корня</param>
        /// <returns>true, если в словаре есть запись</returns>
        internal bool ContainsRootIndex(int rootIndex)
        {
            return RootDictionary.ContainsKey(rootIndex);
        }

        /// <summary>
        /// Преобразование объекта вхождений деревьев в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Id дерева: {0}; число деревьев с корнем текущего дерева: {1}.", TreeId, RootDictionary.Count);
        }
    }
}
