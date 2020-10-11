using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBasics.IdentityModels
{
    public class ApplicationUser : IdentityUser<string>
    {

        public string FullName { get; set; }

        
    }
}
