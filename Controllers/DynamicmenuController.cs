using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBasics.Models;
using WebApiBasics.ORM.Entity;

namespace WebApiBasics.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicmenuController : ControllerBase
    {

        private readonly DenemeContext db;

        public DynamicmenuController(DenemeContext _db)
        {
            db = _db;
        }

        [Route("getmenus")]
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {

            var menus = await db.DynamicMenu
                .Select(i => new DtoMenu()
                {
                    Id = i.Id,
                    MenuName = i.MenuName
                })
                .ToListAsync();

            if (menus == null)
            {
                return NotFound();
            }

            return Ok(menus);
        }


    }
}