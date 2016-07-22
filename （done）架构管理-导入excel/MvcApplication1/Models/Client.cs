using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDemo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("list_2016_client")]
    public class Client
    {
        [Key]
        [Display(Name = "身份证号")]
        [RegularExpression(@"^(\d{15}|\d{18}|\d{17}[0-9Xx])$",ErrorMessage ="请输入正确的身份证号码")]
        public string identify_number { get; set; }

        [Required]
        //     [RegularExpression(@"^[\u4e00-\u9fa5]{2,5}$", ErrorMessage = "请输入正确的姓名")]
        [RegularExpression(@"^\S+$", ErrorMessage = "名字不能为空")]
        [Display(Name = "客户姓名")]
        public string client_name { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{11,11}$", ErrorMessage = "请输入11位手机号")]
        [Display(Name = "手机")]
        public string phone_number { get; set; }

        [Required]
        [Display(Name = "参会时间")]
        public DateTime join_time { get; set; }          

        [Required]
        [Display(Name = "客户经理")]
        [RegularExpression(@"^\d{14}$", ErrorMessage ="请输入14位数字工号")]    //后续考核该验证是否正确
        public string client_manager { get; set; }
    }
}