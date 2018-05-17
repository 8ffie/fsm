using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class ExtendedSubtree
    {
        /// <summary>
        /// Список расширенных поддеревьев
        /// </summary>
        private readonly List<string> subtreesExtended = new List<string>();
        internal List<string> SubtreesExtended
        {
            get { return subtreesExtended; }
        }

        /// <summary>
        /// Добавить поддерево в список расширенных поддеревьев
        /// </summary>
        /// <param name="subtree">Поддерево</param>
        internal void AddSubtree(Tree subtree)
        {
            if (!AlreadyExtended(subtree.DfsString))
            {
                SubtreesExtended.Add(subtree.DfsString);
            }            
        }

        /// <summary>
        /// Проверка, если ли в поддерево в списке расширенных поддеревьев
        /// </summary>
        /// <param name="dfsString">Кодировка дерева</param>
        /// <returns>true, если дерево уже есть в списке расширенных поддеревьев</returns>
        internal bool AlreadyExtended(string dfsString)
        {
            return SubtreesExtended.Contains(dfsString);
        }
    }
}
