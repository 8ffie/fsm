using FrequentSubtreeMining.Algorithm.Tools;
using FrequentSubtreeMining.Algorithm.XML;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class TextTreeEncoding
    {
        /// <summary>
        /// Разделитель узлов в записи кодировки
        /// </summary>
        public static readonly char Separator = ';';

        /// <summary>
        /// Знак возврата к корню
        /// </summary>
        public static readonly char UpSign = '^';

        /// <summary>
        /// Id дерева
        /// </summary>
        internal string TreeId { get; set; }

        /// <summary>
        /// Корень дерева
        /// </summary>
        public TreeNode Root { get; set; }

        /// <summary>
        /// Строковое представление дерева (кодировка) с номером дерева
        /// </summary>
        /// <returns>Строковое представление дерева</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", TreeId, this.ToDfsStringWithIndex());
        }

        /// <summary>
        /// Строковое представление дерева (кодировка) без номера дерева
        /// </summary>
        /// <returns>Строковое представление дерева</returns>
        public string ToStringWithoutTreeId()
        {
            return string.Format("{0}", this.ToDfsString());
        }
    }
}
