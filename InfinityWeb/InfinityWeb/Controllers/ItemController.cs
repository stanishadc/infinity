using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class ItemController : Controller
    {
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public ItemController(RepositoryContext context)
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
        public IActionResult Create()
        {
            Item item = new Item();
            return View("Create", item);
        }
        [HttpGet]
        public async Task<List<Item>> Get()
        {
            var data = await (from i in _context.Items
                              select new Item
                              {
                                  ItemId = i.ItemId,
                                  ItemName = i.ItemName,
                                  CO2 = i.CO2,
                                  OZone = i.OZone,
                                  ItemDescription = i.ItemDescription,
                                  UpdatedDate = dataConversions.ConvertUTCtoLocal(i.UpdatedDate)
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "Item");
            }
            else
            {
                var data = await (from i in _context.Items
                                  select new Item
                                  {
                                      ItemId = i.ItemId,
                                      ItemName = i.ItemName,
                                      CO2=i.CO2,
                                      OZone=i.OZone,
                                      ItemDescription = i.ItemDescription,
                                      UpdatedDate = dataConversions.ConvertUTCtoLocal(i.UpdatedDate)
                                  }).Where(p => p.ItemId == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Item");
                }
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult Create(Item model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ItemId = Guid.NewGuid();
                    model.UpdatedDate = DateTime.UtcNow;
                    _context.Add(model);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "New Record Inserted";
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
            }
            return RedirectToAction("Index", "Item");
        }
        [HttpPost]
        public IActionResult Update(Item model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Items.Where(z => z.ItemId == model.ItemId).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Item");
                    }
                    else
                    {

                        data.ItemName = model.ItemName;
                        data.ItemDescription = model.ItemDescription;
                        data.CO2 = model.CO2;
                        data.OZone = model.OZone;
                        data.UpdatedDate = DateTime.UtcNow;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "Item");
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
                return RedirectToAction("Edit", "Item");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Item item)
        {
            try
            {
                var data = await _context.Items.Where(g => g.ItemId == item.ItemId).AsNoTracking().FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    _context.Items.Remove(data);
                    _context.SaveChanges();
                    ModelState.Clear();
                    TempData["successMessage"] = "Record Deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "Item");
        }
    }
}

