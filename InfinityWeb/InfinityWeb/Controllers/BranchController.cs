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
        public async Task<List<ClientBranchViewModal>> Get()
        {
            var data = await (from b in _context.Branches
                              join c in _context.Clients on b.ReferenceId equals c.ClientId
                              select new ClientBranchViewModal
                              {
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  PrimaryContact = b.PrimaryContact,
                                  Phone = b.Phone,
                                  Email = b.Email,
                                  Address = b.Address,
                                  ClientId = c.ClientId,
                                  ClientName = c.PrimaryContact,
                                  CollectionNotes=b.CollectionNotes,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate)
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid Branch Id";
                return RedirectToAction("Index", "Branch");
            }
            else
            {
                var data = await (from b in _context.Branches
                                  join c in _context.Clients on b.ReferenceId equals c.ClientId
                                  select new ClientBranchViewModal
                                  {
                                      BranchId = b.BranchId,
                                      BranchName = b.BranchName,
                                      PrimaryContact = b.PrimaryContact,
                                      Phone = b.Phone,
                                      Email = b.Email,
                                      Address = b.Address,
                                      ClientId = c.ClientId,
                                      ClientName = c.PrimaryContact,
                                      CollectionNotes=b.CollectionNotes,
                                      UpdatedDate = dataConversions.ConvertUTCtoLocal(b.UpdatedDate)
                                  }).Where(p => p.BranchId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Branch");
                }
                else
                {
                    data.ClientsList = await GetClients();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ClientBranchViewModal branchViewModal = new ClientBranchViewModal();
            branchViewModal.ClientsList = await GetClients();
            return View("Create", branchViewModal);
        }
        [HttpPost]
        public IActionResult Create(ClientBranchViewModal model)
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
                    branch.ReferenceId = model.ClientId;                    
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
        public IActionResult Update(ClientBranchViewModal model)
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
                        data.ReferenceId = model.ClientId;
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
        public async Task<IActionResult> Delete(ClientBranchViewModal branchViewModal)
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
        private async Task<List<SelectListItem>> GetClients()
        {
            var data = await _context.Clients.ToListAsync();
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].PrimaryContact;
                selectListItem.Value = data[i].ClientId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}
