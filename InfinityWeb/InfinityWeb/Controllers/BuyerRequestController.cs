using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class BuyerRequestController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public BuyerRequestController(RepositoryContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
            //else if (Id != Guid.Empty)
            //{
            //    clientBranchUserViewModals = await GetById(Id);
            //}
            return View(buyerRequestViewModals);
        }
        [HttpGet]
        public async Task<List<BuyerRequestViewModal>> Get()
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
                                  GroupTypeName=gt.GroupTypeName
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<BuyerRequestViewModal> GetById(Guid Id)
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
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "BuyerRequest");
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
                    return RedirectToAction("Index", "BuyerRequest");
                }
                else
                {
                    data.ItemsList = await GetItems();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            BuyerRequestViewModal buyerRequestViewModal = new BuyerRequestViewModal();
            buyerRequestViewModal.ItemsList = await GetItems();
            return View("Create", buyerRequestViewModal);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ClientBranchUserViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userExists = await _userManager.FindByNameAsync(model.Email);
                    if (userExists != null)
                    {
                        TempData["errorMessage"] = "Email already exists!";
                    }
                    else
                    {
                        User user = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = model.Email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Name = model.Name,
                            IsActive = true,
                            BranchId = model.BranchId
                        };
                        //model.RoleName = UserRoles.Admin;
                        var result = await _userManager.CreateAsync(user, model.Password);
                        model.RoleName = UserRoles.Admin;
                        if (!result.Succeeded)
                        {
                            TempData["errorMessage"] = "User creation failed! Please check user details and try again.";
                        }
                        if (!await _roleManager.RoleExistsAsync(UserRoles.BuyerUser.ToString()))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(UserRoles.BuyerUser.ToString()));
                        }
                        await _userManager.AddToRoleAsync(user, UserRoles.BuyerUser.ToString());
                        ModelState.Clear();
                        TempData["successMessage"] = "New User Created";
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
            }
            return RedirectToAction("Index", "BuyerRequest");
        }
        [HttpPost]
        public IActionResult Update(BuyerBranchUserViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Users.Where(z => z.Id == model.Id).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return View();
                    }
                    else
                    {

                        data.Name = model.Name;
                        data.Email = model.Email;
                        data.PhoneNumber = model.PhoneNumber;
                        data.IsActive = model.IsActive;
                        data.BranchId = model.BranchId;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Index", "BuyerRequest");
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
                return RedirectToAction("Edit", "BuyerRequest");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(BuyerBranchUserViewModal buyerBranchUserViewModal)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(buyerBranchUserViewModal.Id);
                if (user == null)
                {
                    TempData["errorMessage"] = "Record not found!";
                }
                else
                {
                    var resetPassResult = await _userManager.DeleteAsync(user);
                    if (resetPassResult.Succeeded)
                    {
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Deleted!";
                    }
                    else
                    {
                        TempData["errorMessage"] = DisplayError(resetPassResult);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in deleting the record!";
            }
            return RedirectToAction("Index", "BuyerRequest");
        }
        private string DisplayError(IdentityResult result)
        {
            List<IdentityError> errorList = result.Errors.ToList();
            var errors = string.Join(", ", errorList.Select(e => e.Description));
            return errors;
        }
        private async Task<List<SelectListItem>> GetItems()
        {
            var data = await _context.Items.ToListAsync();

            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].ItemName;
                selectListItem.Value = data[i].ItemId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}

