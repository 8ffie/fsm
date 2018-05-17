using FrequentSubtreeMining.Algorithm.Tools;
using FrequentSubtreeMining.Algorithm.XML;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class TextTreeEncoding
    {
        /// <summary>
        /// Разделитель узлов в записи кодировки
        /// </summary>
        internal static readonly char Separator = ',';

        /// <summary>
        /// Знак возврата к корню
        /// </summary>
        internal static readonly char UpSign = '^';

        /// <summary>
        /// Id дерева
        /// </summary>
        internal string TreeId { get; set; }

        /// <summary>
        /// Корень дерева
        /// </summary>
        internal TreeNode Root { get; set; }

        /// <summary>
        /// Строковое представление дерева (кодировка)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", TreeId, this.ToDfsStringWithIndex());
        }
    }
}
