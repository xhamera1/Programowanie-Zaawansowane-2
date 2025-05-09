
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcPracownicy.Pages
{
    public class FormModel : PageModel
    {
        public String? Message {get; private set;}

        public void OnGet()
        {
            if (Request.Query != null) {
                Message += Request.Query["mojepole"].ToString();
            }
        }
    }
}