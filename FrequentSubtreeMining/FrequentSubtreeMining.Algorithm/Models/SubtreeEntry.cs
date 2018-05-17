using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FrequentSubtreeMining.Algorithm.Models
{
   internal class SubtreeEntry
    {
        /// <summary>
        /// Создание нового вхождения поддерева
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        /// <param name="depth">Глубина</param>
        /// <param name="dfsCode">Список индексов узлов дерева</param>
        /// <returns></returns>
        internal static SubtreeEntry Create(string treeId, int depth, IList<int> dfsCode)
        {
            return new SubtreeEntry(treeId, depth, dfsCode);
        }

        /// <summary>
        /// Id дерева
        /// </summary>
        internal string TreeId { get; private set; }

        /// <summary>
        /// Глубина
        /// </summary>
        internal int Depth { get; private set; }

        /// <summary>
        /// Индекс корня
        /// </summary>
        internal int RootIndex
        {
            get { return DfsCode[0]; }
        }

        /// <summary>
        /// Индекс крайнего правого листа поддерева
        /// </summary>
        internal int RightMostIndex
        {
            get { return DfsCode[DfsCode.Count - 1]; }
        }

        /// <summary>
        /// Индекс второго узла в кодировке дерева
        /// </summary>
        internal int SecondIndex
        {
            get
            {
                Debug.Assert(DfsCode.Count > 1);
                return DfsCode[1];
            }
        }

        /// <summary>
        /// Список индексов узлов дерева
        /// </summary>
        internal ReadOnlyCollection<int> DfsCode { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="treeId">Id дерева</param>
        /// <param name="depth">Глубина</param>
        /// <param name="dfsCode">Список индексов узлов дерева</param>
        private SubtreeEntry(string treeId, int depth, IList<int> dfsCode)
        {
            Debug.Assert(!string.IsNullOrEmpty(treeId));
            Debug.Assert(depth >= 0);
            Debug.Assert(dfsCode != null && dfsCode.Count > 0);
            
            TreeId = treeId;
            Depth = depth;
            DfsCode = new ReadOnlyCollection<int>(dfsCode);
        }

        /// <summary>
        /// Проверка наличия вхождения поддерева в текущем дереве
        /// </summary>
        /// <param name="entry">Вхождение поддерева</param>
        /// <returns>true, если поддерево содержится в текущем дереве</returns>
        internal bool Contains(SubtreeEntry entry)
        {
            if (entry == null) return false;
            if (entry.TreeId != TreeId || entry.RootIndex != RootIndex) return false;
            if (DfsCode.Count < entry.DfsCode.Count) return false;
            if (RightMostIndex < entry.RightMostIndex) return false;

            for (int i = 1; i < entry.DfsCode.Count; i++)
            {
                if (!DfsCode.Contains(entry.DfsCode[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Соединение 2 вхождений поддеревьев
        /// </summary>
        /// <param name="entryToBeConnected">Вхождение поддерева, с которым соединяется текущее поддерево</param>
        /// <returns>Новое вхождение поддерева</returns>
        internal SubtreeEntry Connect(SubtreeEntry entryToBeConnected)
        {
            Debug.Assert(entryToBeConnected != null);
            Debug.Assert(TreeId == entryToBeConnected.TreeId);
            Debug.Assert(RightMostIndex == entryToBeConnected.RootIndex);

            List<int> dfsList = new List<int> { RootIndex };
            dfsList.AddRange(entryToBeConnected.DfsCode);
            return Create(TreeId, Depth, dfsList);
        }

        /// <summary>
        /// Комбинирование 2 вхождений поддеревьев
        /// </summary>
        /// <param name="entryToBeCombined">Вхождение поддерева, с которым комбинируется текущее поддерево</param>
        /// <returns>Новое вхождение поддерева</returns>
        internal SubtreeEntry Combine(SubtreeEntry entryToBeCombined)
        {
            Debug.Assert(entryToBeCombined != null);
            Debug.Assert(TreeId == entryToBeCombined.TreeId);
            Debug.Assert(RootIndex == entryToBeCombined.RootIndex);
            Debug.Assert(RightMostIndex < entryToBeCombined.SecondIndex);

            List<int> dfsList = new List<int>(DfsCode);
            for (int i = 1; i < entryToBeCombined.DfsCode.Count; i++)
            {
                dfsList.Add(entryToBeCombined.DfsCode[i]);
            }
            return Create(TreeId, Depth, dfsList);
        }

        /// <summary>
        /// Преобразование вхождения поддерева в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("ID:{0} [", TreeId));
            for (int i = 0; i < DfsCode.Count - 1; i++)
            {
                sb.Append(DfsCode[i] + ", ");
            }
            sb.Append(DfsCode[DfsCode.Count - 1] + "]");
            return sb.ToString();
        }
    }
}
