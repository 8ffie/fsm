using FrequentSubtreeMining.Algorithm.Models;
using FrequentSubtreeMining.Algorithm.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FrequentSubtreeMining.Algorithm.Tools
{
    public class EncodingBuilder
    {
        /// <summary>
        /// Преобразование строчного представления дерева в объект кодировки
        /// </summary>
        /// <param name="treeInString">Строчное представление дерева</param>
        /// <returns>Объект кодировки дерева</returns>
        public static TextTreeEncoding ConvertToTextTreeEncoding(string treeInString)
        {
            string[] treeInStringArr = treeInString.Split(new[] { TextTreeEncoding.Separator, ' ' }, StringSplitOptions.RemoveEmptyEntries);
            TextTreeEncoding tree = new TextTreeEncoding();
            DoConvert(treeInStringArr, tree);
            return tree;
        }

        /// <summary>
        /// Преобразование списка кодов дерева в объект кодировки дерева
        /// </summary>
        /// <param name="treeInStringArr">Список кодов дерева</param>
        /// <param name="tree">Объект кодировки дерева</param>
        private static void DoConvert(IList<string> treeInStringArr, TextTreeEncoding tree)
        {
            Debug.Assert(treeInStringArr != null && treeInStringArr.Count - 1 >= 2, "Ошибка при конвертации: недостаточно символов в записи дерева");
            Debug.Assert(!treeInStringArr[0].Equals(TextTreeEncoding.UpSign.ToString()), string.Format("Ошибка при конвертации: недопустимый первый символ '{0}'", TextTreeEncoding.UpSign));

            tree.TreeId = treeInStringArr[0];
            int start = 1;
            TreeNode curNode = new TreeNode { Tag = treeInStringArr[start++], Tree = tree };
            tree.Root = curNode;

            for (int i = start; i < treeInStringArr.Count; i++)
            {
                if (treeInStringArr[i].Equals(TextTreeEncoding.UpSign.ToString()))
                {
                    Debug.Assert(!curNode.IsRoot, "Ошибка при конвертации: лишний знак возврата к родителю в записи дерева");

                    curNode = curNode.Parent;
                }
                else
                {
                    TreeNode node = new TreeNode { Tag = treeInStringArr[i], Tree = tree };
                    node.SetParent(curNode);
                    curNode = node;
                }
            }
        }

        /// <summary>
        /// Преобразование строчного представления дерева в объект кодировки
        /// </summary>
        /// <param name="treeInString">Строчное представление дерева</param>
        /// <returns>Объект кодировки дерева</returns>
        public static TextTreeEncoding ConvertToTextTreeEncodingWithoutTreeId(string treeInString)
        {
            string[] treeInStringArr = treeInString.Split(new[] { TextTreeEncoding.Separator, ' ' }, StringSplitOptions.RemoveEmptyEntries);
            TextTreeEncoding tree = new TextTreeEncoding();
            DoConvertWithoutTreeId(treeInStringArr, tree);
            return tree;
        }

        /// <summary>
        /// Преобразование списка кодов дерева в объект кодировки дерева
        /// </summary>
        /// <param name="treeInStringArr">Список кодов дерева</param>
        /// <param name="tree">Объект кодировки дерева</param>
        private static void DoConvertWithoutTreeId(IList<string> treeInStringArr, TextTreeEncoding tree)
        {
            Debug.Assert(treeInStringArr != null && treeInStringArr.Count >= 2, "Ошибка при конвертации: недостаточно символов в записи дерева");
            Debug.Assert(!treeInStringArr[0].Equals(TextTreeEncoding.UpSign.ToString()), string.Format("Ошибка при конвертации: недопустимый первый символ '{0}'", TextTreeEncoding.UpSign));

            int start = 0;
            TreeNode curNode = new TreeNode { Tag = treeInStringArr[start++], Tree = tree };
            tree.Root = curNode;

            for (int i = start; i < treeInStringArr.Count - 1; i++)
            {
                if (treeInStringArr[i].Equals(TextTreeEncoding.UpSign.ToString()))
                {
                    Debug.Assert(!curNode.IsRoot, "Ошибка при конвертации: лишний знак возврата к родителю в записи дерева");

                    curNode = curNode.Parent;
                }
                else
                {
                    TreeNode node = new TreeNode { Tag = treeInStringArr[i], Tree = tree };
                    node.SetParent(curNode);
                    curNode = node;
                }
            }
        }
    }
}
