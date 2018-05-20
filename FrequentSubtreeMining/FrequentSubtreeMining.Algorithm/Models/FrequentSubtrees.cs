using System.Collections.Generic;
using System.Linq;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class FrequentSubtrees
    {
        /// <summary>
        /// Максимальная глубина
        /// </summary>
        internal int MaxDepth { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        internal FrequentSubtrees()
        {
            Frequent1NodeTrees = new Dictionary<string, Tree>();
            Frequent2NodesTrees = new Dictionary<string, Tree>();
            Frequents = new Dictionary<string, Tree>();
            DepthBasedFrequentTrees = new Dictionary<int, Dictionary<string, Tree>>();
        }

        /// <summary>
        /// Словарь деревьев, состоящих из 1 узла
        /// </summary>
        internal Dictionary<string, Tree> Frequent1NodeTrees { get; private set; }

        /// <summary>
        /// Словарь деревьев, состоящих из 2 узлов
        /// </summary>
        internal Dictionary<string, Tree> Frequent2NodesTrees { get; private set; }

        /// <summary>
        /// Словарь частых деревьев
        /// </summary>
        internal Dictionary<string, Tree> Frequents { get; private set; }

        /// <summary>
        /// Словарь деревьев (значение), корень которых встречается на определенной глубине(ключ)
        /// </summary>
        internal Dictionary<int, Dictionary<string, Tree>> DepthBasedFrequentTrees { get; private set; }

        /// <summary>
        /// Добавление частого поддерева
        /// </summary>
        /// <param name="freqSubtree">Частое поддерево</param>
        internal void AddFrequentSubtree(Tree freqSubtree)
        {
            if (!Frequents.ContainsKey(freqSubtree.DfsString))
            {
                Frequents.Add(freqSubtree.DfsString, freqSubtree);
            }
            if (freqSubtree.Is1NodeTree && !Frequent1NodeTrees.ContainsKey(freqSubtree.DfsString))
            {
                Frequent1NodeTrees.Add(freqSubtree.DfsString, freqSubtree);
                return;
            }
            if (freqSubtree.Is2NodeTree && !Frequent2NodesTrees.ContainsKey(freqSubtree.DfsString))
            {
                Frequent2NodesTrees.Add(freqSubtree.DfsString, freqSubtree);
            }
            for (int d = 0; d < MaxDepth; d++)
            {
                if (freqSubtree.ContainsDepth(d) && !DepthBasedFrequentTrees[d].ContainsKey(freqSubtree.DfsString))
                {
                    DepthBasedFrequentTrees[d].Add(freqSubtree.DfsString, freqSubtree);
                }
            }
        }

        /// <summary>
        /// Установка максимальной глубины и добавление записи в словарь деревьев по глубинам
        /// </summary>
        /// <param name="maxDepth">Максимальная глубина</param>
        internal void SetDepth(int maxDepth)
        {
            MaxDepth = maxDepth;
            for (int i = 0; i < MaxDepth; i++)
            {
                DepthBasedFrequentTrees.Add(i, new Dictionary<string, Tree>());
            }
        }

        /// <summary>
        /// Получение списка деревьев, корень которых встречается на заданной глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <returns>Список деревьев, корень которых встречается на заданной глубине</returns>
        internal List<Tree> GetFrequentsAtDepth(int depth)
        {
            List<Tree> freqTrees = new List<Tree>();
            if (DepthBasedFrequentTrees.ContainsKey(depth))
            {
                freqTrees.AddRange(from freqSubtree in DepthBasedFrequentTrees[depth] select freqSubtree.Value);
            }
            return freqTrees;
        }

        /// <summary>
        /// Получение списка деревьев, которые можно присоединить к деревьям из 2 узлов для формирования новых кандидатов 
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <returns>Список деревьев для присоединения</returns>
        internal List<Tree> GetToBeConnectableAtDepth(int depth)
        {
            List<Tree> treesToBeConnectable = new List<Tree>();
            if (DepthBasedFrequentTrees.ContainsKey(depth))
            {
                treesToBeConnectable.AddRange(DepthBasedFrequentTrees[depth].Values.Where(t => t.AbleToBeConnected));
            }
            return treesToBeConnectable;
        }

        /// <summary>
        /// Получение списка деревьев, которые можно соединить на данной глубине (имеют 2 узла, корень находится на глубине depth)
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <returns>Список поддеревьев, которые можно использовать для соединения с другими поддеревьями</returns>
        internal List<Tree> GetConnectableAtDepth(int depth)
        {
            return Frequent2NodesTrees.Values.Where(f2 => f2.AbleToConnect && f2.ContainsDepth(depth)).ToList();
        }

        /// <summary>
        /// Получение поддерева на заданной глубине
        /// </summary>
        /// <param name="subtreeDfsString">Кодировка дерева</param>
        /// <param name="depth">Глубина</param>
        /// <returns>Объект поддерева</returns>
        internal Tree GetSubtreeAtDepth(string subtreeDfsString, int depth)
        {
            if (DepthBasedFrequentTrees.ContainsKey(depth) && DepthBasedFrequentTrees[depth].ContainsKey(subtreeDfsString))
            {
                return DepthBasedFrequentTrees[depth][subtreeDfsString];
            }
            return null;
        }

        /// <summary>
        /// Получение списка кандидатов, которых можно комбинировать на заданной глубине
        /// </summary>
        /// <param name="depth">Глубина</param>
        /// <returns>Список кандидатов, которых можно комбинировать на заданной глубине</returns>
        internal List<Tree> GetCombinableAtDepth(int depth)
        {
            List<Tree> combinableTrees = new List<Tree>();
            if (DepthBasedFrequentTrees.ContainsKey(depth))
            {
                combinableTrees.AddRange(DepthBasedFrequentTrees[depth].Values.Where(tree => tree.AbleToCombine && tree.SingleChild));
            }
            return combinableTrees;
        }

        /// <summary>
        /// Удаление кандидатов, которые далее не могут быть расширены
        /// </summary>
        /// <param name="depth">Глубина</param>
        internal void RemoveCannotBeExtended(int depth)
        {
            List<string> keysToRemove = (from tree in DepthBasedFrequentTrees[depth].Values
                                where !tree.AbleToConnect && !tree.AbleToBeConnected && !tree.AbleToCombine
                                select tree.DfsString).ToList();
            RemoveSubtrees(keysToRemove);
        }

        /// <summary>
        /// Удаление поддеревьев
        /// </summary>
        /// <param name="keysToRemove">Список id деревьев для удаления</param>
        private void RemoveSubtrees(List<string> keysToRemove)
        {
            foreach (string key in keysToRemove)
            {
                for (int i = 0; i < MaxDepth; i++)
                {
                    if (DepthBasedFrequentTrees[i].ContainsKey(key))
                    {
                        DepthBasedFrequentTrees[i].Remove(key);
                    }
                }
            }
        }
    }
}
