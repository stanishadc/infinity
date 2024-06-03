using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Request = InfinityWeb.Models.Request;

namespace InfinityWeb.Controllers
{
    public class CollectorRequestController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public CollectorRequestController(RepositoryContext context)
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
            List<BuyerRequestViewModal> buyerRequestViewModals = new List<BuyerRequestViewModal>();
            if (Id == Guid.Empty || Id == null)
            {
                buyerRequestViewModals = await Get();
            }
            return View(buyerRequestViewModals);
        }
        [HttpGet]
        public async Task<List<BuyerRequestViewModal>> Get()
        {
            var data = await (from r in _context.Requests
                              join i in _context.Items on r.ItemId equals i.ItemId
                              join c in _context.Collectors on r.ItemId equals c.CollectorId
                              join bu in _context.Users on r.UserId equals bu.Id
                              join br in _context.Branches on bu.BranchId equals br.BranchId
                              join b in _context.Buyers on br.ReferenceId equals b.BuyerId
                              join g in _context.Groups on b.GroupId equals g.GroupId
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new BuyerRequestViewModal
                              {
                                  RequestId = r.RequestId,
                                  ItemId = r.ItemId,
                                  ItemName = i.ItemName,
                                  Quantity = r.Quantity,
                                  RequestedDate = r.RequestedDate,
                                  RequestStatus = r.RequestStatus,
                                  Notes = r.Notes,
                                  Name = bu.Name,
                                  Email = bu.Email,
                                  Phone = bu.PhoneNumber,
                                  BranchName = br.BranchName,
                                  BranchEmail = br.Email,
                                  BranchPhone = br.Phone,
                                  BranchPrimaryContact = br.PrimaryContact,
                                  BuyerEmail = b.Email,
                                  BuyerPhone = b.Phone,
                                  BuyerPrimaryContact = b.PrimaryContact,
                                  GroupName = g.GroupName,
                                  GroupTypeName = gt.GroupTypeName,
                                  CollectorId=c.CollectorId,
                                  CollectorName=c.PrimaryContact,
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "CollectorRequest");
            }
            else
            {
                var data = await (from r in _context.Requests
                                  join i in _context.Items on r.ItemId equals i.ItemId
                                  join c in _context.Collectors on r.CollectorId equals c.CollectorId
                                  join bu in _context.Users on r.UserId equals bu.Id
                                  join br in _context.Branches on bu.BranchId equals br.BranchId
                                  join b in _context.Buyers on br.ReferenceId equals b.BuyerId
                                  join g in _context.Groups on b.GroupId equals g.GroupId
                                  join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                                  select new BuyerRequestViewModal
                                  {
                                      RequestId = r.RequestId,
                                      ItemId = r.ItemId,
                                      ItemName = i.ItemName,
                                      Quantity = r.Quantity,
                                      RequestedDate = r.RequestedDate,
                                      RequestStatus = r.RequestStatus,
                                      Notes = r.Notes,
                                      Name = bu.Name,
                                      Email = bu.Email,
                                      Phone = bu.PhoneNumber,
                                      BranchName = br.BranchName,
                                      BranchEmail = br.Email,
                                      BranchPhone = br.Phone,
                                      BranchPrimaryContact = br.PrimaryContact,
                                      BuyerEmail = b.Email,
                                      BuyerPhone = b.Phone,
                                      BuyerPrimaryContact = b.PrimaryContact,
                                      GroupName = g.GroupName,
                                      GroupTypeName = gt.GroupTypeName,
                                      CollectorId = r.CollectorId,
                                      CollectorName = c.PrimaryContact,
                                  }).Where(c => c.RequestId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "CollectorRequest");
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Assign(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "CollectorRequest");
            }
            else
            {
                var data = await (from r in _context.Requests
                                  join i in _context.Items on r.ItemId equals i.ItemId
                                  join bu in _context.Users on r.UserId equals bu.Id
                                  join br in _context.Branches on bu.BranchId equals br.BranchId
                                  join b in _context.Buyers on br.ReferenceId equals b.BuyerId
                                  join g in _context.Groups on b.GroupId equals g.GroupId
                                  join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                                  select new BuyerRequestViewModal
                                  {
                                      RequestId = r.RequestId,
                                      ItemId = r.ItemId,
                                      ItemName = i.ItemName,
                                      Quantity = r.Quantity,
                                      RequestedDate = r.RequestedDate,
                                      RequestStatus = r.RequestStatus,
                                      Notes = r.Notes,
                                      Name = bu.Name,
                                      Email = bu.Email,
                                      Phone = bu.PhoneNumber,
                                      BranchName = br.BranchName,
                                      BranchEmail = br.Email,
                                      BranchPhone = br.Phone,
                                      BranchPrimaryContact = br.PrimaryContact,
                                      BuyerEmail = b.Email,
                                      BuyerPhone = b.Phone,
                                      BuyerPrimaryContact = b.PrimaryContact,
                                      GroupName = g.GroupName,
                                      GroupTypeName = gt.GroupTypeName
                                  }).Where(c => c.RequestId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "CollectorRequest");
                }
                else
                {
                    data.CollectorsList = await GetCollectorDrivers();
                }
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult Update(BuyerRequestViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var request = _context.Requests.Where(z => z.RequestId == model.RequestId).AsNoTracking().FirstOrDefault();
                    if (request == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return View();
                    }
                    else
                    {
                        request.CollectorId = model.CollectorId;
                        request.UpdatedDate = DateTime.UtcNow;
                        request.RequestStatus = RequestStatus.CollectorAssigned;
                        _context.Entry(request).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "CollectorRequest");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        private async Task<List<SelectListItem>> GetCollectorDrivers()
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
