using System;
using System.Text;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class SearchParameters : ICloneable
    {
        /// <summary>
        /// Минимальная поддержка (доля частых поддеревьев)
        /// </summary>
        private double minimumSupport;

        /// <summary>
        /// Количество анализируемых деревьев
        /// </summary>
        internal static int treeNumber;

        /// <summary>
        /// Поддержка (число деревьев, в которых должны встречаться частые поддеревья)
        /// </summary>
        internal int Support
        {
            get
            {
                return (int)(minimumSupport * treeNumber);
            }
        }

        /// <summary>
        /// Минимальное число узлов
        /// </summary>
        internal int MinimumNodeNumber { get; private set; }

        /// <summary>
        /// Максимальное число узлов
        /// </summary>
        internal int MaximumNodeNumber { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="minimumSupport">Минимальная поддержка</param>
        /// <param name="minNodeNumber">Минимальное число узлов</param>
        /// <param name="maxNodeNumber">Максимальное число узлов</param>
        internal SearchParameters(double minimumSupport, int minNodeNumber, int maxNodeNumber)
        {
            this.minimumSupport = minimumSupport;
            MinimumNodeNumber = minNodeNumber;
            MaximumNodeNumber = maxNodeNumber;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="another">Параметры поиска</param>
        internal SearchParameters(SearchParameters another)
        {
            minimumSupport = another.minimumSupport;
            MinimumNodeNumber = another.MinimumNodeNumber;
            MaximumNodeNumber = another.MaximumNodeNumber;
        }

        /// <summary>
        /// Строковое представление параметров поиска
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Условия поиска:");
            sb.AppendLine(string.Format("Поддержка: {0}", minimumSupport));
            sb.AppendLine(string.Format("Минимальное число узлов: {0}", MinimumNodeNumber));
            sb.AppendLine(string.Format("Максимальное число узлов: {0}", MaximumNodeNumber));
            sb.AppendLine(string.Format("Общее число деревьев: {0}", treeNumber));
            return sb.ToString();
        }

        /// <summary>
        /// Клонирование параметров поиска
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SearchParameters(this);
        }
    }
}
