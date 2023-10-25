using InfinityWeb.Helpers;
using InfinityWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InfinityWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RepositoryContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUriService uriService;
        private readonly Pagination _pagination;
        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, RepositoryContext context, IConfiguration configuration, IUriService uriService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _pagination = new Pagination();
            this.uriService = uriService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationFilter filter)
        {
            try
            {
                var data = await (from a in _context.Users
                join ur in _context.UserRoles on a.Id equals ur.UserId
                                  join r in _context.Roles on ur.RoleId equals r.Id
                                  select new
                                  {
                                      a.Name,
                                      a.Email,
                                      a.PhoneNumber,
                                      a.IsActive,
                                      a.Id,
                                      ur.RoleId,
                                      RoleName = r.Name
                                  }).Where(r => r.RoleName == UserRoles.User.ToString()).ToListAsync();
                var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetClients")]
        public async Task<IActionResult> GetClients([FromQuery] PaginationFilter filter)
        {
            try
            {
                var data = await (from a in _context.Users
                                  join ur in _context.UserRoles on a.Id equals ur.UserId
                                  join r in _context.Roles on ur.RoleId equals r.Id
                                  select new
                                  {
                                      a.Name,
                                      a.Email,
                                      a.PhoneNumber,
                                      a.IsActive,
                                      a.Id,
                                      ur.RoleId,
                                      RoleName = r.Name
                                  }).Where(r => r.RoleName == UserRoles.Client.ToString()).ToListAsync();
                var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetCollectors")]
        public async Task<IActionResult> GetCollectors([FromQuery] PaginationFilter filter)
        {
            try
            {
                var data = await (from a in _context.Users
                                  join ur in _context.UserRoles on a.Id equals ur.UserId
                                  join r in _context.Roles on ur.RoleId equals r.Id
                                  select new
                                  {
                                      a.Name,
                                      a.Email,
                                      a.PhoneNumber,
                                      a.IsActive,
                                      a.Id,
                                      ur.RoleId,
                                      RoleName = r.Name
                                  }).Where(r => r.RoleName == UserRoles.Collector.ToString()).ToListAsync();
                var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
                return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = HttpStatusCode.InternalServerError, Data = ex.Message });
            }
        }
    }
}
