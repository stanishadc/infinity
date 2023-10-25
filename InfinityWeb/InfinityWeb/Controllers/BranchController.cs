using InfinityWeb.Helpers;
using InfinityWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InfinityWeb.Controllers
{
    public class BranchController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly IUriService uriService;
        private readonly Pagination _pagination;
        public BranchController(RepositoryContext context, IUriService uriService)
        {
            _context = context;
            _pagination = new Pagination();
            this.uriService = uriService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Get(string RoleName, [FromQuery] PaginationFilter filter)
        {
            var data = await (from b in _context.Branches
                              join u in _context.Users on b.UserId.ToString() equals u.Id
                              join ur in _context.UserRoles on u.Id equals ur.RoleId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              select new
                              {
                                  b.BranchId,
                                  b.BranchName,
                                  b.PrimaryContact,
                                  b.Phone,
                                  b.Email,
                                  b.Address,
                                  b.UserId,
                                  u.Name,
                                  u.IsActive,
                                  RoleName = r.Name,
                                  ur.RoleId
                              }).Where(r => r.RoleName == RoleName).ToListAsync();
            var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
            return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
        }
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers(Guid BranchId, [FromQuery] PaginationFilter filter)
        {
            var data = await (from b in _context.Branches
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              join u in _context.Users on b.UserId.ToString() equals u.Id
                              join ur in _context.UserRoles on u.Id equals ur.RoleId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              select new
                              {
                                  b.BranchId,
                                  b.BranchName,
                                  b.PrimaryContact,
                                  b.Phone,
                                  b.Email,
                                  b.Address,
                                  b.UserId,
                                  g.GroupId,
                                  g.GroupName,
                                  u.Name,
                                  u.IsActive,
                                  RoleName = r.Name,
                                  ur.RoleId
                              }).Where(r => r.BranchId == BranchId).ToListAsync();
            var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
            return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
        }
        [HttpGet]
        [Route("GetByBranchId")]
        public async Task<IActionResult> GetByBranchId(Guid BranchId)
        {
            var data = await (from b in _context.Branches
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              join u in _context.Users on b.UserId.ToString() equals u.Id
                              join ur in _context.UserRoles on u.Id equals ur.RoleId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              select new
                              {
                                  b.BranchId,
                                  b.BranchName,
                                  b.PrimaryContact,
                                  b.Phone,
                                  b.Email,
                                  b.Address,
                                  b.UserId,
                                  g.GroupId,
                                  g.GroupName,
                                  u.Name,
                                  u.IsActive,
                                  RoleName = r.Name,
                                  ur.RoleId
                              }).Where(r => r.BranchId == BranchId).ToListAsync();
            return View(data);
        }
        [HttpGet]
        [Route("GetByGroupId")]
        public async Task<IActionResult> GetByGroupId(Guid GroupId)
        {
            var data = await (from b in _context.Branches
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              join u in _context.Users on b.UserId.ToString() equals u.Id
                              join ur in _context.UserRoles on u.Id equals ur.RoleId
                              join r in _context.Roles on ur.RoleId equals r.Id
                              select new
                              {
                                  b.BranchId,
                                  b.BranchName,
                                  b.PrimaryContact,
                                  b.Phone,
                                  b.Email,
                                  b.Address,
                                  b.UserId,
                                  g.GroupId,
                                  g.GroupName,
                                  u.Name,
                                  u.IsActive,
                                  RoleName = r.Name,
                                  ur.RoleId
                              }).Where(r => r.GroupId == GroupId).ToListAsync();
            return View(data);
        }
        [HttpPost]
        public IActionResult Insert(Branch model)
        {
            try
            {
                model.BranchId = Guid.NewGuid();
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
                _context.Add(model);
                _context.SaveChanges();
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Index", ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Update(Branch model)
        {
            try
            {
                var data = _context.Branches.Where(z => z.BranchId == model.BranchId).AsNoTracking().FirstOrDefault();
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    data.BranchName = model.BranchName;
                    data.Address = model.Address;
                    data.UserId = model.UserId;
                    data.Email= model.Email;
                    data.GroupId = model.GroupId;
                    data.Phone = model.Phone;
                    data.PrimaryContact = model.PrimaryContact;
                    data.UpdatedDate = DateTime.UtcNow;
                    _context.Entry(data).State = EntityState.Modified;
                    _context.SaveChanges();
                    return View("Index", "Record Updated Successfully");
                }
            }
            catch (Exception ex)
            {
                return View("Index", ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                var data = await _context.Branches.FindAsync(Id);
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    _context.Branches.Remove(data);
                    await _context.SaveChangesAsync();
                    return View("Index", "Record Deleted!");
                }
            }
            catch (Exception ex)
            {
                return View("Index", "Error in deleting the record!");
            }
        }
    }
}
