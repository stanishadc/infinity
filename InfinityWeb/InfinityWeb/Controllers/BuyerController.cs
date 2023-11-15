using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class BuyerController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public BuyerController(RepositoryContext context)
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
        private async Task<List<BuyerViewModal>> Get()
        {
            var data = await (from b in _context.Buyers
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              select new BuyerViewModal
                              {
                                  BuyerId = b.BuyerId,
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
                TempData["errorMessage"] = "Invalid Buyer Id";
                return RedirectToAction("Index", "Buyer");
            }
            else
            {
                var data = await (from c in _context.Buyers
                                  join g in _context.Groups on c.GroupId equals g.GroupId
                                  select new BuyerViewModal
                                  {
                                      BuyerId = c.BuyerId,
                                      CollectionNotes = c.CollectionNotes,
                                      PrimaryContact = c.PrimaryContact,
                                      Email = c.Email,
                                      Phone = c.Phone,
                                      Address = c.Address,
                                      UpdatedDate = c.UpdatedDate,
                                      GroupId = g.GroupId,
                                      GroupName = g.GroupName
                                  }).Where(p => p.BuyerId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Buyer");
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
            BuyerViewModal BuyerViewModal = new BuyerViewModal();
            BuyerViewModal.GroupsList = await GetGroups();
            return View("Create", BuyerViewModal);
        }
        [HttpPost]
        public IActionResult Create(BuyerViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Buyer data = new Buyer();
                    data.BuyerId = Guid.NewGuid();
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
                    return RedirectToAction("Create", "Buyer");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Buyer");
        }
        [HttpPost]
        public IActionResult Update(Buyer model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Buyers.Where(z => z.BuyerId == model.BuyerId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Buyer");
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
                        return RedirectToAction("Index", "Buyer");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Update", "Buyer");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "Buyer");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(BuyerViewModal BuyerViewModal)
        {
            try
            {
                var data = await _context.Buyers.Where(g => g.BuyerId == BuyerViewModal.BuyerId).AsNoTracking().FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    _context.Buyers.Remove(data);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "Buyer");
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
                              }).Where(g => g.GroupTypeName == UserRoles.Buyer.ToString()).ToListAsync();
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


