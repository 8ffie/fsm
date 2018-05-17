using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.Tools;
using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.XML
{
    internal class TreeNode
    {
        /// <summary>
        /// Тэг-метка текущего узла
        /// </summary>
        internal string Tag { get; set; }

        /// <summary>
        /// Dfs-индекс текущего узла
        /// </summary>
        internal int DfsIndex { get; set; }

        /// <summary>
        /// Глубина текущего узла
        /// </summary>
        internal int Depth { get; set; }

        /// <summary>
        /// Проверка, является ли текущий узел корнем
        /// </summary>
        internal bool IsRoot
        {
            get { return Parent == null; }
        }

        /// <summary>
        /// Проверка, является ли текущий узел листом
        /// </summary>
        internal bool IsLeaf
        {
            get { return (Children == null || Children.Count <= 0); }
        }

        /// <summary>
        /// Родительский узел текущего узла
        /// </summary>
        internal TreeNode Parent { get; set; }

        /// <summary>
        /// Список детей текущего узла
        /// </summary>
        internal List<TreeNode> Children { get; set; }

        /// <summary>
        /// Объект кодировки дерева с корнем в текущем узле
        /// </summary>
        internal TextTreeEncoding Tree { get; set; }

        /// <summary>
        /// Сравнение деревьев
        /// </summary>
        /// <param name="other">Узел, с которым сравнивается текущий узел</param>
        /// <returns>-1 (меньше), 0 (равны), 1 (больше)</returns>
        public int CompareTo(TreeNode other)
        {
            return Tag.CompareTo(other.Tag);
        }

        /// <summary>
        /// Преобразование объекта узла дерева в строку кодировки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToDfsString();
        }

        /// <summary>
        /// Установка родителя текущего узла
        /// </summary>
        /// <param name="parent">Родительский узел</param>
        internal void SetParent(TreeNode parent)
        {
            if (Parent != null && !Equals(Parent, parent))
            {
                Parent.Children.Remove(this);
            }
            Parent = parent;
            if (parent == null)
            {
                Depth = 0;
            }
            else
            {
                Depth = Parent.Depth + 1;
                if (Parent.Children == null) Parent.Children = new List<TreeNode>();
                Parent.Children.Add(this);
            }
        }

        /// <summary>
        /// Сравнение 2 деревьев
        /// </summary>
        /// <param name="nodeX">Корень первого дерева</param>
        /// <param name="nodeY">Корень второго дерева</param>
        /// <returns>-1 (меньше), 0 (равны), 1 (больше)</returns>
        internal static int Compare(TreeNode nodeX, TreeNode nodeY)
        {
            if (nodeX == null && nodeY == null) return 0; //равны

            if (nodeX == null) return 1; //больше
            if (nodeY == null) return -1; //меньше

            var strListX = nodeX.ToDfsStringList();
            var strListY = nodeY.ToDfsStringList();

            int min = strListX.Count < strListY.Count ? strListX.Count : strListY.Count;

            for (var i = 0; i < min; i++)
            {
                if (strListX[i] != TextTreeEncoding.UpSign.ToString() && strListY[i] == TextTreeEncoding.UpSign.ToString()) return -1;
                if (strListX[i] == TextTreeEncoding.UpSign.ToString() && strListY[i] != TextTreeEncoding.UpSign.ToString()) return 1;

                int result = string.CompareOrdinal(strListX[i], strListY[i]);
                if (result != 0) return result;
            }

            if (strListX.Count == strListY.Count) return 0;

            return (strListX.Count > strListY.Count) ? -1 : 1;
        }
    }
}
