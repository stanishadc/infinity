using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class BuyerBranchController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public BuyerBranchController(RepositoryContext context)
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
            List<BuyerBranchViewModal> BuyerBranchViewModals  = new List<BuyerBranchViewModal>();
            if (Id == Guid.Empty || Id == null)
            {
                BuyerBranchViewModals = await Get();
            }
            else if (Id != Guid.Empty)
            {
                BuyerBranchViewModals = await GetById(Id);
            }
            return View(BuyerBranchViewModals);
        }
        [HttpGet]
        public async Task<List<BuyerBranchViewModal>> Get()
        {
            var data = await (from b in _context.Branches
                              join c in _context.Buyers on b.ReferenceId equals c.BuyerId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new BuyerBranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  BuyerId = c.BuyerId,
                                  BuyerName = c.PrimaryContact,
                                  CollectionNotes = b.CollectionNotes,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate),
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<List<BuyerBranchViewModal>> GetById(Guid? Id)
        {
            var data = await (from b in _context.Branches
                              join c in _context.Buyers on b.ReferenceId equals c.BuyerId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new BuyerBranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  BuyerId = c.BuyerId,
                                  BuyerName = c.PrimaryContact,
                                  CollectionNotes = b.CollectionNotes,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate),
                                  GroupId = g.GroupId,
                                  GroupName = g.GroupName,
                              }).Where(c => c.BuyerId == Id).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid BuyerBranch Id";
                return RedirectToAction("Index", "BuyerBranch");
            }
            else
            {
                var data = await (from b in _context.Branches
                                  join c in _context.Buyers on b.ReferenceId equals c.BuyerId
                                  select new BuyerBranchViewModal
                                  {
                                      BranchId = b.BranchId,
                                      BranchName = b.BranchName,
                                      PrimaryContact = b.PrimaryContact,
                                      Phone = b.Phone,
                                      Email = b.Email,
                                      Address = b.Address,
                                      BuyerId = c.BuyerId,
                                      BuyerName = c.PrimaryContact,
                                      CollectionNotes = b.CollectionNotes,
                                      UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate)
                                  }).Where(p => p.BranchId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "BuyerBranch");
                }
                else
                {
                    data.BuyersList = await GetBuyers();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            BuyerBranchViewModal BuyerBranchViewModal = new BuyerBranchViewModal();
            BuyerBranchViewModal.BuyersList = await GetBuyers();
            return View("Create", BuyerBranchViewModal);
        }
        [HttpPost]
        public IActionResult Create(BuyerBranchViewModal model)
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
                    branch.ReferenceId = model.BuyerId;
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
                    return RedirectToAction("Create", "BuyerBranch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "BuyerBranch");
        }
        [HttpPost]
        public IActionResult Update(BuyerBranchViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Branches.Where(z => z.BranchId == model.BranchId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "BuyerBranch");
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
                        data.ReferenceId = model.BuyerId;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "BuyerBranch");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "BuyerBranch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "BuyerBranch");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(BuyerBranchViewModal BuyerBranchViewModal)
        {
            try
            {
                var branch = await _context.Branches.Where(g => g.BranchId == BuyerBranchViewModal.BranchId).AsNoTracking().FirstOrDefaultAsync();
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
            return RedirectToAction("Index", "BuyerBranch");
        }
        private async Task<List<SelectListItem>> GetBuyers()
        {
            var data = await _context.Buyers.ToListAsync();
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].PrimaryContact;
                selectListItem.Value = data[i].BuyerId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}

