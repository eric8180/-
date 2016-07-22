using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDemo.Models
{
    [Table("branch_name")]
    public class Hn_Area
    {

        [Key]
        [Display(Name = "区号")]
        public string branch { get; set; }

        [Display(Name = "市州名称")]
        public string name { get; set; }

    }
}