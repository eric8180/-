namespace MVCDemo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("list_zgs_2016_ry")]
    public partial class RY
    {
        [Key]
        [Display(Name = "序号")]
        public int seq { get; set; }

        [Required]
        [Display(Name = "地区")]
        public string area { get; set; }

        [Required]
        [Display(Name = "支公司")]
        public string c_name { get; set; }

        [Required]
        [RegularExpression(@"^[\u4e00-\u9fa5]{2,4}$", ErrorMessage = "请输入正确的姓名")]
        [Display(Name = "姓名")]
        public string c_person { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{11,11}$", ErrorMessage = "请输入11位手机号")]
        [Display(Name = "电话")]
        public string phone { get; set; }

        [Required]
        [Display(Name = "系统来源")]
        public string sys { get; set; }

   //     [Required]
        [Display(Name = "类型")]
        public string type { get; set; }
    }
}