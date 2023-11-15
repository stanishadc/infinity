using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class CollectorBranchController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public CollectorBranchController(RepositoryContext context)
        {
            _context = context;
            dataConversions = new DataConversions();
        }
        public async Task<IActionResult> Index(Guid? Id)
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
            List<CollectorBranchViewModal> collectorBranchViewModals = new List<CollectorBranchViewModal>();
            if (Id == Guid.Empty || Id == null)
            {
                collectorBranchViewModals = await Get();
            }
            else if (Id != Guid.Empty)
            {
                collectorBranchViewModals = await GetById(Id);
            }
            return View(collectorBranchViewModals);
        }
        [HttpGet]
        public async Task<List<CollectorBranchViewModal>> Get()
        {
            var data = await (from b in _context.Branches
                              join c in _context.Collectors on b.ReferenceId equals c.CollectorId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new CollectorBranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  CollectorId = c.CollectorId,
                                  CollectorName = c.PrimaryContact,
                                  CollectionNotes = b.CollectionNotes,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate),
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<List<CollectorBranchViewModal>> GetById(Guid? Id)
        {
            var data = await (from b in _context.Branches
                              join c in _context.Collectors on b.ReferenceId equals c.CollectorId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new CollectorBranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  CollectorId = c.CollectorId,
                                  CollectorName = c.PrimaryContact,
                                  CollectionNotes = b.CollectionNotes,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate),
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                              }).Where(c => c.CollectorId == Id).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid CollectorBranch Id";
                return RedirectToAction("Index", "CollectorBranch");
            }
            else
            {
                var data = await (from b in _context.Branches
                                  join c in _context.Collectors on b.ReferenceId equals c.CollectorId
                                  select new CollectorBranchViewModal
                                  {
                                      BranchId = b.BranchId,
                                      BranchName = b.BranchName,
                                      PrimaryContact = b.PrimaryContact,
                                      Phone = b.Phone,
                                      Email = b.Email,
                                      Address = b.Address,
                                      CollectorId = c.CollectorId,
                                      CollectorName = c.PrimaryContact,
                                      CollectionNotes = b.CollectionNotes,
                                      UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate)
                                  }).Where(p => p.BranchId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "CollectorBranch");
                }
                else
                {
                    data.CollectorsList = await GetCollectors();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CollectorBranchViewModal collectorBranchViewModal = new CollectorBranchViewModal();
            collectorBranchViewModal.CollectorsList = await GetCollectors();
            return View("Create", collectorBranchViewModal);
        }
        [HttpPost]
        public IActionResult Create(CollectorBranchViewModal model)
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
                    branch.ReferenceId = model.CollectorId;
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
                    return RedirectToAction("Create", "CollectorBranch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "CollectorBranch");
        }
        [HttpPost]
        public IActionResult Update(CollectorBranchViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Branches.Where(z => z.BranchId == model.BranchId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "CollectorBranch");
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
                        data.ReferenceId = model.CollectorId;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "CollectorBranch");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "CollectorBranch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "CollectorBranch");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(CollectorBranchViewModal collectorBranchViewModal)
        {
            try
            {
                var branch = await _context.Branches.Where(g => g.BranchId == collectorBranchViewModal.BranchId).AsNoTracking().FirstOrDefaultAsync();
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
            return RedirectToAction("Index", "CollectorBranch");
        }
        private async Task<List<SelectListItem>> GetCollectors()
        {
            var data = await _context.Collectors.ToListAsync();
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].PrimaryContact;
                selectListItem.Value = data[i].CollectorId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}
