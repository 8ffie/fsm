using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.XML
{
    internal class XMLTreeEncoding
    {
        /// <summary>
        /// Id дерева
        /// </summary>
        internal int TreeId { get; private set; }

        /// <summary>
        /// Список узлов дерева
        /// </summary>
        internal List<XMLNode> Nodes { get; set; }

        /// <summary>
        /// Кодировка дерева
        /// </summary>
        internal string Encoding { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">Id дерева</param>
        internal XMLTreeEncoding(int id)
        {
            TreeId = id;
            Nodes = new List<XMLNode>();
            Encoding = string.Empty;
        }
    }
}
