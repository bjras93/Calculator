using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Model
{
    public class MainModel
    {
        [Required]
        public int? Input { get; set; }
        public List<int> Numbers { get; set; }
        public List<string> Signs { get; set; }
        public string Sign { get; set; }
        public int? Result { get; set; }
        public string Calculation { get; set; }
    }
}
