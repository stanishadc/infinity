using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class GroupController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public GroupController(RepositoryContext context)
        {
            _context = context;
            dataConversions = new DataConversions();
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Home" });
                return RedirectToRoute(routeValue);
            }
            if (HttpContext.Session.GetString("Token") == null)
            {
                var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Home" });
                return RedirectToRoute(routeValue);
            }
            var data = await Get();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            GroupViewModal groupViewModal = new GroupViewModal();
            groupViewModal.GroupTypesList = await GetGroupTypes();
            return View("Create", groupViewModal);
        }
        [HttpGet]
        public async Task<List<GroupViewModal>> Get()
        {
            var data = await (from g in _context.Groups
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new GroupViewModal
                              {
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                                  Address = g.Address,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(g.UpdatedDate),
                                  GroupTypeId = gt.GroupTypeId,
                                  GroupTypeName = gt.GroupTypeName
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> GetById(Guid GroupId)
        {
            var data = await (from g in _context.Groups
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new GroupViewModal
                              {
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                                  Address = g.Address,
                                  UpdatedDate = g.UpdatedDate,
                                  GroupTypeId = gt.GroupTypeId,
                                  GroupTypeName = gt.GroupTypeName
                              }).Where(p => p.GroupId == GroupId).ToListAsync();
            return View(data);
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
                var data = await (from g in _context.Groups
                                  join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                                  select new GroupViewModal
                                  {
                                      GroupId = g.GroupId,
                                      GroupName = g.GroupName,
                                      Address = g.Address,
                                      UpdatedDate = g.UpdatedDate,
                                      GroupTypeId = gt.GroupTypeId,
                                      GroupTypeName = gt.GroupTypeName
                                  }).Where(p => p.GroupId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Group");
                }
                else
                {
                    data.GroupTypesList = await GetGroupTypes();
                }
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult Create(GroupViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Group group=new Group();
                    group.GroupId = Guid.NewGuid();
                    group.Address = model.Address;
                    group.UpdatedDate = model.UpdatedDate;
                    group.GroupTypeId = model.GroupTypeId;
                    group.GroupName = model.GroupName;
                    group.CreatedDate = DateTime.UtcNow;
                    group.UpdatedDate = DateTime.UtcNow;
                    _context.Add(group);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "New Record Inserted";
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Create", "Group");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Group");
        }
        [HttpPost]
        public IActionResult Update(Group model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Groups.Where(z => z.GroupId == model.GroupId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Group");
                    }
                    else
                    {

                        data.GroupName = model.GroupName;
                        data.Address = model.Address;
                        data.UpdatedDate = DateTime.UtcNow;
                        data.GroupTypeId = model.GroupTypeId;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "Group");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "Group");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "Group");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(GroupViewModal groupViewModal)
        {
            try
            {
                var group = await _context.Groups.Where(g => g.GroupId == groupViewModal.GroupId).AsNoTracking().FirstOrDefaultAsync();
                if (group == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    _context.Groups.Remove(group);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "Group");
        }
        private async Task<List<SelectListItem>> GetGroupTypes()
        {
            var data = await _context.GroupTypes.ToListAsync();
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].GroupTypeName;
                selectListItem.Value = data[i].GroupTypeId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}
