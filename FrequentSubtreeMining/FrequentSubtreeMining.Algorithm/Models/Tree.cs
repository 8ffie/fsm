using FrequentSubtreeMining.Algorithm.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class Tree : IComparable
    {
        /// <summary>
        /// Создание объекта нового дерева
        /// </summary>
        /// <param name="dfsRepresentation">Dfs-кодировка дерева</param>
        /// <param name="singleChild">Флаг, имеет ли корень дерева единственного ребенка</param>
        /// <param name="support">Поддержка</param>
        /// <returns>Объект нового дерева</returns>
        internal static Tree Create(string[] dfsRepresentation, bool singleChild, int support)
        {
            Debug.Assert(dfsRepresentation != null && dfsRepresentation.Length >= 2, "Неправильное dfs-представление");
           
            return new Tree(dfsRepresentation, singleChild, support);
        }

        /// <summary>
        /// Деревья, из которых было получено текущее
        /// </summary>
        internal Tree Father { get; set; }
        internal Tree Mother { get; set; }

        /// <summary>
        /// Минимальная поддержка
        /// </summary>
        internal int Support { get; private set; }
       
        /// <summary>
        /// Код корня дерева
        /// </summary>
        internal string FirstSymbol
        {
            get
            {
                Debug.Assert(DfsRepresentation != null && DfsRepresentation.Count() > 0);
                return DfsRepresentation[0];
            }
        }

        /// <summary>
        /// Код второго узла в кодировке
        /// </summary>
        internal string SecondSymbol
        {
            get
            {
                Debug.Assert(!Is1NodeTree);
                return DfsRepresentation[1];
            }
        }

        /// <summary>
        /// Проверка, является ли дерево частым
        /// </summary>
        internal bool IsFrequent
        {
            get
            {       
                return TreeSupport >= Support;
            }
        }

        /// <summary>
        /// Dfs-кодировка
        /// </summary>
        internal string DfsString { get; private set; }

        /// <summary>
        /// Список dfs-кодов узлов
        /// </summary>
        internal readonly ReadOnlyCollection<string> DfsRepresentation;

        //Флаги возможности использования для расширения 
        internal bool AbleToCombine { get; private set; }
        internal bool AbleToConnect { get; private set; }
        internal bool AbleToBeConnected { get; private set; }

        /// <summary>
        /// Размер дерева
        /// </summary>
        internal int Size
        {
            get { return DfsRepresentation.Count / 2; }
        }

        /// <summary>
        /// Проверка, состоит ли дерево из 2 узлов
        /// </summary>
        internal bool Is2NodeTree
        {
            get { return Size == 2; }
        }

        /// <summary>
        /// Проверка, состоит ли дерево из 1 узла
        /// </summary>
        internal bool Is1NodeTree
        {
            get { return Size == 1; }
        }

        /// <summary>
        /// Флаг, имеет ли корень дерева 1 ребенка
        /// </summary>
        internal readonly bool SingleChild;
        
        readonly Dictionary<int, DepthEntries> depthEntriesDictionary;
        /// <summary>
        /// Словарь вхождений деревьев (значение) по глубинам (ключ)
        /// </summary>
        internal Dictionary<int, DepthEntries> DepthEntriesDictionary
        {
            get { return depthEntriesDictionary; }
        }

        internal DepthEntries this[int depth]
        {
            get
            {
                return DepthEntriesDictionary.ContainsKey(depth) ? DepthEntriesDictionary[depth] : null;
            }
        }

        readonly HashSet<string> supportTreeSet;
        /// <summary>
        /// Множество идентификаторов деревьев, в которых встречается текущее дерево
        /// </summary>
        internal HashSet<string> SupportTreeSet
        {
            get { return supportTreeSet; }
        }

        /// <summary>
        /// Число деревьев, в которых встречается текущее дерево
        /// </summary>
        internal int TreeSupport
        {
            get { return SupportTreeSet.Count; }
        }

        /// <summary>
        /// Получение числа вхождений деревьев на глубинах, выше заданной
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <param name="includingRoot">Флаг, включать ли корень в рассмотрение</param>
        /// <returns>Число вхождений деревьев на глубинах, выше заданной</returns>
        internal int TreeSupportAbove(int depth, bool includingRoot)
        {
            HashSet<string> hashSet = new HashSet<string>();
            while (--depth >= 0)
            {
                if (depth == 0 && !includingRoot) break;
                if (!ContainsDepth(depth)) continue;
                foreach (TreeEntries tree in this[depth].GetTreeEntries())
                {
                    hashSet.Add(tree.TreeId);
                }
            }
            return hashSet.Count;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dfsRepresentation">Список dfs-кодов узлов</param>
        /// <param name="singleChild">Флаг, имеет ли корень дерева 1 ребенка</param>
        /// <param name="support">Минимальная поддержка</param>
        private Tree(IList<string> dfsRepresentation, bool singleChild, int support)
        {
            Debug.Assert(dfsRepresentation != null && dfsRepresentation.Count >= 2 && dfsRepresentation.Count % 2 == 0);

            DfsString = dfsRepresentation.ToDfsString();
            SingleChild = singleChild;
            DfsRepresentation = new ReadOnlyCollection<string>(dfsRepresentation);
            depthEntriesDictionary = new Dictionary<int, DepthEntries>();
            supportTreeSet = new HashSet<string>();
            AbleToConnect = Is2NodeTree;
            AbleToBeConnected = !Is1NodeTree;
            AbleToCombine = true;
            Support = support;
        }

        /// <summary>
        /// Добавить вхождение дерева в словарь вхождений по глубинам
        /// </summary>
        /// <param name="entry">Вхождение дерева</param>
        internal void AddEntry(SubtreeEntry entry)
        {
            if (!DepthEntriesDictionary.ContainsKey(entry.Depth))
            {
                DepthEntriesDictionary.Add(entry.Depth, new DepthEntries(entry.Depth));
            }
            if (!DepthEntriesDictionary[entry.Depth].ContainsEntry(entry))
            {
                if (!SupportTreeSet.Contains(entry.TreeId))
                {
                    SupportTreeSet.Add(entry.TreeId);
                }
                DepthEntriesDictionary[entry.Depth].AddEntry(entry);
            }
        }

        public override string ToString()
        {
            return string.Format("Дерево: {0}, поддержка: {1}; ", DfsString, ((double)TreeSupport / SearchParameters.treeNumber));
        }

        /// <summary>
        /// Преобразование кодировки в тэговую запись XML
        /// </summary>
        /// <returns></returns>
        internal string[] ToTagEntry()
        {
            List<string> tagEntry = new List<string>();
            Stack<string> tagStack = new Stack<string>();
            int offsetCounter = 0;
            foreach (string item in DfsRepresentation)
            {
                if (item != TextTreeEncoding.UpSign.ToString())
                {
                    string str = "";
                    for (int i = 0; i < offsetCounter; i++)
                    {
                        str += "\t";
                    }
                    tagEntry.Add(str + "<" + item + ">");
                    tagStack.Push(item);
                    offsetCounter++;
                }
                else
                {
                    offsetCounter--;
                    string str = "";
                    for (int i = 0; i < offsetCounter; i++)
                    {
                        str += "\t";
                    }
                    string elem = tagStack.Pop();
                    tagEntry.Add(str + "</" + elem + ">");
                }
            }
            return tagEntry.ToArray();
        }

        /// <summary>
        /// Проверка, есть ли записи деревьев в словаре вхождений по глубинам текущего дерева
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <returns>true, если запись есть</returns>
        internal bool ContainsDepth(int depth)
        {
            return DepthEntriesDictionary.ContainsKey(depth);
        }

        public int CompareTo(object obj)
        {
            var tree = obj as Tree;
            if (tree != null)
                return string.Compare(DfsString, tree.DfsString, StringComparison.Ordinal);
            return 0;
        }

        /// <summary>
        /// Проверка вхождения поддерева на определенной глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <param name="treeId">Id поддерева</param>
        /// <returns>true, если поддерево на заданной глубине существует</returns>
        internal bool ContainsTreeAtDepth(int depth, string treeId)
        {
            return (DepthEntriesDictionary.ContainsKey(depth) && DepthEntriesDictionary[depth].ContainsTree(treeId));
        }

        /// <summary>
        /// Проверка наличия индекса корня 
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <param name="treeId">Id дерева</param>
        /// <param name="rootIndex">Индекс корня</param>
        /// <returns>true, если запись </returns>
        internal bool ContainsRootIndex(int depth, string treeId, int rootIndex)
        {
            return DepthEntriesDictionary.ContainsKey(depth) && (DepthEntriesDictionary[depth].ContainsRootIndex(treeId, rootIndex));
        }

        /// <summary>
        /// Получение первого вхождения поддерева на крайнем правом пути
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <param name="treeId">Id дерева</param>
        /// <param name="rootIndex">Индекс корня</param>
        /// <returns>Первое вхождение поддерева на крайнем правом пути</returns>
        internal SubtreeEntry GetEntry(int depth, string treeId, int rootIndex)
        {
            return ContainsRootIndex(depth, treeId, rootIndex) ? this[depth][treeId][rootIndex].FirstEntry : null;
        }

        /// <summary>
        /// Получение первого вхождения поддерева после заданного индекса
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <param name="treeId">Id дерева</param>
        /// <param name="rootIndex">Индекс корня</param>
        /// <param name="specifiedIndex">Индекс поддерева</param>
        /// <returns>Первое вхождение поддерева после заданного индекса</returns>
        internal SubtreeEntry GetFirstEntryAfterSpecifiedIndex(int depth, string treeId, int rootIndex, int specifiedIndex)
        {
            if (!ContainsRootIndex(depth, treeId, rootIndex)) return null;
            RootEntry rEntry = this[depth][treeId][rootIndex];
            return rEntry.RightMostList.FirstOrDefault(t => t.SecondIndex > specifiedIndex);
        }

        /// <summary>
        /// Пометка кандидатов, не являющихся частыми 
        /// </summary>
        /// <param name="support">Поддержка</param>
        /// <param name="depth">Глубина</param>
        internal void PruneAfterConnection(int support, int depth)
        {
            if (AbleToCombine || AbleToConnect)
            {
                int t = TreeSupportAbove(depth + 1, true);
                if (t < support)
                {
                    AbleToCombine = false;
                    AbleToConnect = false;
                }
            }
            if (AbleToBeConnected)
            {
                int t = TreeSupportAbove(depth + 1, false);
                if (t < support) AbleToBeConnected = false;
            }

        }
    }
}
