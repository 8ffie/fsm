using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace FrequentSubtreeMining.WebUI.ViewModels
{
    public class MiningViewModel
    {
        [Required(ErrorMessage="Пожалуйста, введите минимальный размер подграфов (минимальное число узлов)")]
        public int MinimumNodeNumber { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите максимальный размер подграфов (максимальное число узлов)")]
        public int MaximumNodeNumber { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите минимальную поддержку (отношение числа деревьев, в которых встречается подграф, к количеству анализируемых деревьев")]
        public double Support { get; set; }

        public string FileName { get; set; }

        public string[] DocumentText { get; set; }
        public string ErrorMessage { get; set; }
    }
}