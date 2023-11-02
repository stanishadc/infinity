using Azure;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InfinityWeb.Controllers
{
    public class GroupTypeController : Controller
    {
        private readonly RepositoryContext _context;
        public GroupTypeController(RepositoryContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var data = GetGroupTypes();
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            GroupType groupType=new GroupType();
            return PartialView("_CreateModalPartial", groupType);
        }
        
        [HttpPost]
        public IActionResult Create(GroupType groupType)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    groupType.GroupTypeId = Guid.NewGuid();
                    groupType.LastUpdated = DateTime.UtcNow;
                    _context.Add(groupType);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "New Record Inserted";
                    return RedirectToAction("Index", "GroupType");
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return PartialView("_CreateModalPartial", groupType);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return PartialView("_CreateModalPartial", groupType);
            }
        }
        [HttpGet]
        public IActionResult Edit(Guid GroupTypeId)
        {
            var data = _context.GroupTypes.Where(x => x.GroupTypeId == GroupTypeId).FirstOrDefault();
            return PartialView("_EditModalPartial", data);
        }
        [HttpPost]
        public IActionResult Edit(GroupType model)
        {
            try
            {
                var data = _context.GroupTypes.Where(z => z.GroupTypeId == model.GroupTypeId).AsNoTracking().FirstOrDefault();
                if (data == null)
                {
                    TempData["errorMessage"] = "Record not exists!";
                }
                else
                {
                    data.GroupTypeName = model.GroupTypeName;
                    data.LastUpdated = DateTime.UtcNow;
                    _context.Entry(data).State = EntityState.Modified;
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "GroupType");
        }
        [HttpPost]
        public IActionResult Delete(GroupType groupType)
        {
            try
            {
                _context.GroupTypes.Remove(groupType);
                _context.SaveChanges();
                ModelState.Clear();
                TempData["successMessage"] = "Record Deleted!";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "GroupType");
        }
        private List<GroupType> GetGroupTypes()
        {
            return _context.GroupTypes.OrderBy(o => o.LastUpdated).ToList();
        }
    }
}
