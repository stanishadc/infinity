using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InfinityWeb.Controllers
{
    public class BranchController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public BranchController(RepositoryContext context)
        {
            _context = context;
            dataConversions = new DataConversions();
        }
        public async Task<IActionResult> Index()
        {
            var data = await Get();
            return View(data);
        }
        [HttpGet]
        public async Task<List<BranchViewModal>> Get()
        {
            var data = await (from b in _context.Branches
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              select new BranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(g.UpdatedDate)
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid GroupId";
                return RedirectToAction("Index", "Group");
            }
            else
            {
                var data = await (from b in _context.Branches
                                  join g in _context.Groups on b.GroupId equals g.GroupId
                                  select new BranchViewModal
                                  {
                                      BranchId = b.BranchId,
                                      BranchName = b.BranchName,
                                      CollectionNotes = b.CollectionNotes,
                                      PrimaryContact = b.PrimaryContact,
                                      Email = b.Email,
                                      Phone = b.Phone,
                                      Address = b.Address,
                                      UpdatedDate = b.UpdatedDate,
                                      GroupId = g.GroupId,
                                      GroupName = g.GroupName
                                  }).Where(p => p.BranchId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Group");
                }
                else
                {
                    data.GroupsList = await GetGroups();
                }
                return View(data);
            }
        }
        //[HttpGet]
        //public async Task<List<BranchViewModal>> GetByRole(string RoleName)
        //{
        //    var data = await (from b in _context.Branches
        //                      join u in _context.Users on b.UserId.ToString() equals u.Id
        //                      join ur in _context.UserRoles on u.Id equals ur.RoleId
        //                      join r in _context.Roles on ur.RoleId equals r.Id
        //                      select new
        //                      {
        //                          b.BranchId,
        //                          b.BranchName,
        //                          b.PrimaryContact,
        //                          b.Phone,
        //                          b.Email,
        //                          b.Address,
        //                          b.UserId,
        //                          u.Name,
        //                          u.IsActive,
        //                          RoleName = r.Name,
        //                          ur.RoleId
        //                      }).Where(r => r.RoleName == RoleName).ToListAsync();
        //    return data;
        //}
        //[HttpGet]
        //[Route("GetUsers")]
        //public async Task<IActionResult> GetUsers(Guid BranchId, [FromQuery] PaginationFilter filter)
        //{
        //    var data = await (from b in _context.Branches
        //                      join g in _context.Groups on b.GroupId equals g.GroupId
        //                      join u in _context.Users on b.UserId.ToString() equals u.Id
        //                      join ur in _context.UserRoles on u.Id equals ur.RoleId
        //                      join r in _context.Roles on ur.RoleId equals r.Id
        //                      select new
        //                      {
        //                          b.BranchId,
        //                          b.BranchName,
        //                          b.PrimaryContact,
        //                          b.Phone,
        //                          b.Email,
        //                          b.Address,
        //                          b.UserId,
        //                          g.GroupId,
        //                          g.GroupName,
        //                          u.Name,
        //                          u.IsActive,
        //                          RoleName = r.Name,
        //                          ur.RoleId
        //                      }).Where(r => r.BranchId == BranchId).ToListAsync();
        //    var pagedResponse = _pagination.GetPagination(data, filter, Request.Path.Value, uriService);
        //    return Ok(new { StatusCode = HttpStatusCode.OK, Data = pagedResponse });
        //}
        //[HttpGet]
        //[Route("GetByBranchId")]
        //public async Task<IActionResult> GetByBranchId(Guid BranchId)
        //{
        //    var data = await (from b in _context.Branches
        //                      join g in _context.Groups on b.GroupId equals g.GroupId
        //                      join u in _context.Users on b.UserId.ToString() equals u.Id
        //                      join ur in _context.UserRoles on u.Id equals ur.RoleId
        //                      join r in _context.Roles on ur.RoleId equals r.Id
        //                      select new
        //                      {
        //                          b.BranchId,
        //                          b.BranchName,
        //                          b.PrimaryContact,
        //                          b.Phone,
        //                          b.Email,
        //                          b.Address,
        //                          b.UserId,
        //                          g.GroupId,
        //                          g.GroupName,
        //                          u.Name,
        //                          u.IsActive,
        //                          RoleName = r.Name,
        //                          ur.RoleId
        //                      }).Where(r => r.BranchId == BranchId).ToListAsync();
        //    return View(data);
        //}
        //[HttpGet]
        //[Route("GetByGroupId")]
        //public async Task<IActionResult> GetByGroupId(Guid GroupId)
        //{
        //    var data = await (from b in _context.Branches
        //                      join g in _context.Groups on b.GroupId equals g.GroupId
        //                      join u in _context.Users on b.UserId.ToString() equals u.Id
        //                      join ur in _context.UserRoles on u.Id equals ur.RoleId
        //                      join r in _context.Roles on ur.RoleId equals r.Id
        //                      select new
        //                      {
        //                          b.BranchId,
        //                          b.BranchName,
        //                          b.PrimaryContact,
        //                          b.Phone,
        //                          b.Email,
        //                          b.Address,
        //                          b.UserId,
        //                          g.GroupId,
        //                          g.GroupName,
        //                          u.Name,
        //                          u.IsActive,
        //                          RoleName = r.Name,
        //                          ur.RoleId
        //                      }).Where(r => r.GroupId == GroupId).ToListAsync();
        //    return View(data);
        //}
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            BranchViewModal branchViewModal = new BranchViewModal();
            branchViewModal.GroupsList = await GetGroups();
            return View("Create", branchViewModal);
        }
        [HttpPost]
        public IActionResult Create(BranchViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Branch branch = new Branch();
                    branch.BranchId = Guid.NewGuid();
                    branch.BranchName = model.BranchName;
                    branch.PrimaryContact = model.PrimaryContact;
                    branch.Email = model.Email;
                    branch.Phone = model.Phone;
                    branch.CollectionNotes = model.CollectionNotes;
                    branch.Address = model.Address;
                    branch.UpdatedDate = model.UpdatedDate;
                    branch.GroupId = model.GroupId;                    
                    branch.CreatedDate = DateTime.UtcNow;
                    branch.UpdatedDate = DateTime.UtcNow;
                    _context.Add(branch);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "New Record Inserted";
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Create", "Branch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Branch");
        }
        [HttpPost]
        public IActionResult Update(Branch model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Branches.Where(z => z.BranchId == model.BranchId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Branch");
                    }
                    else
                    {

                        data.BranchName = model.BranchName;
                        data.PrimaryContact = model.PrimaryContact;
                        data.Email = model.Email;
                        data.Phone = model.Phone;
                        data.CollectionNotes = model.CollectionNotes;
                        data.Address = model.Address;
                        data.UpdatedDate = DateTime.UtcNow;
                        data.GroupId = model.GroupId;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "Branch");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "Branch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "Branch");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(BranchViewModal branchViewModal)
        {
            try
            {
                var branch = await _context.Branches.Where(g => g.BranchId == branchViewModal.BranchId).AsNoTracking().FirstOrDefaultAsync();
                if (branch == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    _context.Branches.Remove(branch);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "Branch");
        }
        private async Task<List<SelectListItem>> GetGroups()
        {
            var data = await _context.Groups.ToListAsync();
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].GroupName;
                selectListItem.Value = data[i].GroupId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}
