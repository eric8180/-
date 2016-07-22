using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDemo.Models
{
    [Table("list_zc")]
    public class ZClist
    {
        [Key]
        [Display(Name = "序号")]
        public int zcserno { get; set; }

        [Required]
        [Display(Name = "职场编号")]
        public string zcno { get; set; }

        [Required]
        [Display(Name = "职场名称")]
        public string zcname { get; set; }

        [Required]
        [Display(Name = "支公司编号")]
        public string bid { get; set; }

        [Required]
        [Display(Name = "一种标志")]
        public string areflag { get; set; }

        [Required]
        [Display(Name = "系统来源")]
        public string sys { get; set; }

        [Display(Name = "职场地址")]
        public string zcaddr { get; set; }
    }
}