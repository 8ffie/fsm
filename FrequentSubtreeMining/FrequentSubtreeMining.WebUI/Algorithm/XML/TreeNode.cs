using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.Tools;
using System.Collections.Generic;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.XML
{
    public class TreeNode
    {
        public bool Part { get; set; }

        /// <summary>
        /// Тэг-метка текущего узла
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Dfs-индекс текущего узла
        /// </summary>
        public int DfsIndex { get; set; }

        /// <summary>
        /// Глубина текущего узла
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Проверка, является ли текущий узел корнем
        /// </summary>
        public bool IsRoot
        {
            get { return Parent == null; }
        }

        /// <summary>
        /// Проверка, является ли текущий узел листом
        /// </summary>
        public bool IsLeaf
        {
            get { return (Children == null || Children.Count <= 0); }
        }

        /// <summary>
        /// Родительский узел текущего узла
        /// </summary>
        public TreeNode Parent { get; set; }

        /// <summary>
        /// Список детей текущего узла
        /// </summary>
        public List<TreeNode> Children { get; set; }

        /// <summary>
        /// Объект кодировки дерева с корнем в текущем узле
        /// </summary>
        public TextTreeEncoding Tree { get; set; }

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

        /// <summary>
        /// Проверка наличия поддерева по кодировкам
        /// </summary>
        /// <param name="treeEncoding">Кодировка дерева</param>
        /// <param name="subtreeEncoding">Кодировка поддерева</param>
        /// <param name="markedTree">Кодировка дерева с пометками узлов поддерева</param>
        /// <returns>true, если дерево содержит заданное поддерева</returns>
        public static bool ContainsSubtree(string treeEncoding, string subtreeEncoding, out TreeNode markedTree)
        {
            TextTreeEncoding treeEncodingObject = EncodingBuilder.ConvertToTextTreeEncodingWithoutTreeId(treeEncoding);
            TextTreeEncoding subtreeEncodingObject = EncodingBuilder.ConvertToTextTreeEncodingWithoutTreeId(subtreeEncoding);
            bool isSubtree = subtreeEncodingObject.Root.IsContainedIn(treeEncodingObject.Root);
            markedTree = isSubtree ? treeEncodingObject.Root : null;
            return isSubtree;
        }

        /// <summary>
        /// Проверка, является ли текущее дерево поддеревом source
        /// </summary>
        /// <param name="source">Дерево-источник</param>
        /// <returns>true, если текущее дерево является поддеревом</returns>
        private bool IsSubtreeOf(TreeNode source)
        {
            if (Children == null || Children.Count == 0)
            {
                return true;
            }
            else if (source.Children == null || source.Children.Count == 0)
            {
                return false;
            }
            else
            {
                Dictionary<TreeNode, int> coincidenceDict = new Dictionary<TreeNode, int>();
                foreach (TreeNode ch in Children)
                {
                    string s = ch.ToDfsString();
                    var coincidence = coincidenceDict.Keys.Where(x => x.ToDfsString() == s).FirstOrDefault();
                    if (coincidence == null)
                    {
                        coincidenceDict.Add(ch, 1);
                    }
                    else
                    {
                        coincidenceDict[coincidence]++;
                    }
                }
                //проверить Children на повторения. если есть повторяющиеся поддеревья, сделать словарь, сколько каких нужно найти
                foreach (TreeNode treeNode in coincidenceDict.Keys) //если есть повторения, нужно проверять, одинаковые ли они. если они одинаковые, ставить счетчик, сколько нужно найти
                {
                    List<TreeNode> sameChildrenList = source.Children.Where(x => x.Tag == treeNode.Tag && !x.Part).ToList();
                    if (sameChildrenList.Count >= coincidenceDict[treeNode])
                    {
                        int counter = 0;
                        foreach (TreeNode sChild in sameChildrenList)
                        {
                            if (treeNode.IsSubtreeOf(sChild))
                            {
                                sChild.Part = true;
                                counter++;
                            }
                        }
                        if (counter < coincidenceDict[treeNode]) return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Проверка, содержится ли текущее поддерево в заданном дереве
        /// </summary>
        /// <param name="source">Дерево-источник</param>
        /// <returns>true, если текущее поддерево содержится в дереве</returns>
        private bool IsContainedIn(TreeNode source)
        {
            List<TreeNode> sameRootTrees = new List<TreeNode>();
            RootSearchTraversal(Tag, source, sameRootTrees);
            if (sameRootTrees.Count > 0)
            {
                bool isFound = false;
                foreach (TreeNode sourceRoot in sameRootTrees)
                {
                    if (IsSubtreeOf(sourceRoot))
                    {
                        isFound = true;
                        sourceRoot.Part = true;
                    }
                }
                return isFound;
            }
            return false;
        }

        /// <summary>
        /// Обход дерева с поиском всех корней с соответствующей меткой
        /// </summary>
        /// <param name="rootTag">Метка корня</param>
        /// <param name="source">Дерево</param>
        /// <param name="previousList">Список узлов с меткой корня</param>
        private void RootSearchTraversal(string rootTag, TreeNode source, List<TreeNode> previousList)
        {
            if (source.Tag == rootTag)
            {
                previousList.Add(source);
            }
            if (source.Children != null && source.Children.Count != 0)
            {
                foreach (TreeNode child in source.Children)
                {
                    RootSearchTraversal(rootTag, child, previousList);
                }
            }
        }
    }
}
