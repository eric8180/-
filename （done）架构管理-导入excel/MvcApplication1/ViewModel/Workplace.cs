using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCDemo.ViewModel
{
    public class Workplace
    {
        [Key]
        [Display(Name = "序号")]
        public int seq { get; set; }

        [Required]
        [Display(Name = "归属地编号")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "请输入6位归属地编号")]
        public string branch_no { get; set; }

        [Required]
        [Display(Name = "归属地")]
        public string name { get; set; }

        [Required]
        [Display(Name = "支公司")]
        public string zyh_name { get; set; }

        [Required]
        [Display(Name = "职场")]
        public string zcname { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "请输入12位职场号")]
        [Display(Name = "职场编号")]
        public string zc_no { get; set; }

        [Required]
        [Display(Name = "分属系统")]
        public string sys { get; set; }

        //     [Required]
        [Display(Name = "类型")]
        public string type { get; set; }
    }
}