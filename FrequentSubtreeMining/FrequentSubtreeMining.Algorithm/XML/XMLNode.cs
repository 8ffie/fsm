using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.XML
{
    public class XMLNode
    {
        /// <summary>
        /// Id узла
        /// </summary>
        internal int? Id { get; private set; }

        /// <summary>
        /// Тэг данного узла
        /// </summary>
        internal string Tag { get; private set; }

        /// <summary>
        /// Код данного узла
        /// </summary>
        internal int Code { get; set; }

        /// <summary>
        /// Начало XML-узла (номер строки в документе)
        /// </summary>
        internal int? LineNumberStart { get; private set; }

        /// <summary>
        /// Конец XML-узла (номер строки в документе)
        /// </summary>
        internal int? LineNumberEnd { get; set; }

        /// <summary>
        /// Текст узла (при наличии)
        /// </summary>
        internal string Text { get; set; }

        /// <summary>
        /// Глубина данного узла
        /// </summary>
        internal int Depth { get; set; }

        /// <summary>
        /// Родительский узел данного узла
        /// </summary>
        internal XMLNode Parent { get; set; }

        /// <summary>
        /// Список потомков данного узла
        /// </summary>
        internal List<XMLNode> Children { get; set; }

        internal XMLNode(int id, string tag, int depth, int lineNumberStart)
        {
            Id = id;
            Tag = tag;
            Depth = depth;
            LineNumberStart = lineNumberStart;
        }
    }
}
