using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcWebApp.Models.MainDemo
{
    /// <summary>
    /// 此model廢棄不用
    /// </summary>
    public class MainQueryModel
    {
        [Display(Name = "輸入字段1")]
        [Required(ErrorMessage = "请输入{0}")]
         public string queryFiled01 { get; set; }
    }
}