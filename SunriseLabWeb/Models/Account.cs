using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SunriseLabWeb.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter your User Name.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter your Password.")]
        public string Password { get; set; }
        public bool isRemember { get; set; }
    }
}