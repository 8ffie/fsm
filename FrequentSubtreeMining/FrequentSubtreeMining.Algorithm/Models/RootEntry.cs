using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.Models
{
    internal class RootEntry
    {
        /// <summary>
        /// Id дерева
        /// </summary>
        private string TreeId { get; set; }

        /// <summary>
        /// Глубина
        /// </summary>
        private int Depth { get; set; }

        /// <summary>
        /// Индекс корня
        /// </summary>
        internal int RootIndex { get; private set; }

        /// <summary>
        /// Список вхождений поддеревьев на крайнем правом пути
        /// </summary>
        internal List<SubtreeEntry> RightMostList { get; private set; }

        /// <summary>
        /// Первое вхождение поддерева на крайнем правом пути
        /// </summary>
        internal SubtreeEntry FirstEntry
        {
            get
            {
                if (RightMostList != null && RightMostList.Count > 0) return RightMostList[0];

                return null;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        /// <param name="depth">Глубина</param>
        /// <param name="rootIndex">Индекс корня</param>
        internal RootEntry(string treeId, int depth, int rootIndex)
        {
            TreeId = treeId;
            Depth = depth;
            RootIndex = rootIndex;
        }

        /// <summary>
        /// Добавить вхождение поддерева для текущего корня
        /// </summary>
        /// <param name="entry">Вхождение поддерева</param>
        /// <returns>1, если на крайнем правом пути есть 1 поддерево, иначе 0</returns>
        internal int AddEntry(SubtreeEntry entry)
        { 
            Debug.Assert(entry.TreeId == TreeId && entry.RootIndex == RootIndex);

            if (RightMostList == null)
            {
                RightMostList = new List<SubtreeEntry>();
            }
            RightMostList.Add(entry);
            return RightMostList.Count == 1 ? 1 : 0;
        }

        /// <summary>
        /// Проверка, содержится ли вхождение поддерева на крайнем правом пути (имеет крайний правый лист дерева)
        /// </summary>
        /// <param name="entry">Вхождение поддерева</param>
        /// <returns>true, если вхождение поддерева есть на крайнем правом пути</returns>
        internal bool ContainsEntry(SubtreeEntry entry)
        {
            if (RightMostList == null || RightMostList.Count == 0)
            {
                return false;
            }
            return RightMostList.Any(t => t.RightMostIndex == entry.RightMostIndex);
        }
    }
}
