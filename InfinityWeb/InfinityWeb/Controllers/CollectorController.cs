using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class CollectorController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public CollectorController(RepositoryContext context)
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

        private async Task<List<CollectorViewModal>> Get()
        {
            var data = await (from b in _context.Collectors
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              select new CollectorViewModal
                              {
                                  CollectorId = b.CollectorId,
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
                TempData["errorMessage"] = "Invalid Client Id";
                return RedirectToAction("Index", "Collector");
            }
            else
            {
                var data = await (from c in _context.Collectors
                                  join g in _context.Groups on c.GroupId equals g.GroupId
                                  select new CollectorViewModal
                                  {
                                      CollectorId = c.CollectorId,
                                      CollectionNotes = c.CollectionNotes,
                                      PrimaryContact = c.PrimaryContact,
                                      Email = c.Email,
                                      Phone = c.Phone,
                                      Address = c.Address,
                                      UpdatedDate = c.UpdatedDate,
                                      GroupId = g.GroupId,
                                      GroupName = g.GroupName
                                  }).Where(p => p.CollectorId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Collector");
                }
                else
                {
                    data.GroupsList = await GetGroups();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CollectorViewModal collectorViewModal = new CollectorViewModal();
            collectorViewModal.GroupsList = await GetGroups();
            return View("Create", collectorViewModal);
        }
        [HttpPost]
        public IActionResult Create(ClientViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Collector data = new Collector();
                    data.CollectorId = Guid.NewGuid();
                    data.PrimaryContact = model.PrimaryContact;
                    data.Email = model.Email;
                    data.Phone = model.Phone;
                    data.CollectionNotes = model.CollectionNotes;
                    data.Address = model.Address;
                    data.UpdatedDate = model.UpdatedDate;
                    data.GroupId = model.GroupId;
                    data.CreatedDate = DateTime.UtcNow;
                    data.UpdatedDate = DateTime.UtcNow;
                    _context.Add(data);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "New Record Inserted";
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Create", "Collector");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Collector");
        }
        [HttpPost]
        public IActionResult Update(Collector model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Collectors.Where(z => z.CollectorId == model.CollectorId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Collector");
                    }
                    else
                    {
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
                        return RedirectToAction("Index", "Collector");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "Collector");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "Collector");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(CollectorViewModal collectorViewModal)
        {
            try
            {
                var data = await _context.Collectors.Where(g => g.CollectorId == collectorViewModal.CollectorId).AsNoTracking().FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    _context.Collectors.Remove(data);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "Collector");
        }
        private async Task<List<SelectListItem>> GetGroups()
        {
            var data = await (from g in _context.Groups
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new
                              {
                                  g.GroupId,
                                  g.GroupName,
                                  gt.GroupTypeId,
                                  gt.GroupTypeName,
                              }).Where(g => g.GroupTypeName == UserRoles.Collector.ToString()).ToListAsync();
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

