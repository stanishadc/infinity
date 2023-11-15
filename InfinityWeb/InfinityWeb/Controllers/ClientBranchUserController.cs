using InfinityWeb.Helpers;
using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class ClientBranchUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RepositoryContext _context;
        private readonly DataConversions dataConversions;
        public ClientBranchUserController(RepositoryContext context, UserManager<User> userManager,
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
            List<ClientBranchUserViewModal> clientBranchUserViewModals = new List<ClientBranchUserViewModal>();
            if (Id == Guid.Empty || Id == null)
            {
                clientBranchUserViewModals = await Get();
            }
            //else if (Id != Guid.Empty)
            //{
            //    clientBranchUserViewModals = await GetById(Id);
            //}
            return View(clientBranchUserViewModals);
        }
        [HttpGet]
        public async Task<List<ClientBranchUserViewModal>> Get()
        {
            var data = await (from bu in _context.Users
                              join b in _context.Branches on bu.BranchId equals b.BranchId
                              join c in _context.Clients on b.ReferenceId equals c.ClientId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new ClientBranchUserViewModal
                              {
                                  Id = bu.Id,
                                  Name = bu.Name,
                                  Email = bu.Email,
                                  PhoneNumber = bu.PhoneNumber,
                                  IsActive = bu.IsActive,
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,                                  
                                  ClientName = c.PrimaryContact,
                                  ClientId = c.ClientId,
                              }).ToListAsync();
            return data;
        }
        [HttpGet]
        public async Task<ClientBranchUserViewModal> GetById(string Id)
        {
            var data = await (from bu in _context.Users
                              join b in _context.Branches on bu.BranchId equals b.BranchId
                              join c in _context.Clients on b.ReferenceId equals c.ClientId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              select new ClientBranchUserViewModal
                              {
                                  Id = bu.Id,
                                  Name = bu.Name,
                                  Email = bu.Email,
                                  PhoneNumber = bu.PhoneNumber,
                                  IsActive = bu.IsActive,
                                  BranchId = b.BranchId,
                                  BranchName = b.BranchName,
                                  ClientName = c.PrimaryContact,
                                  ClientId = c.ClientId,
                              }).Where(c => c.Id == Id).FirstOrDefaultAsync();
            return data;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "ClientBranchUser");
            }
            else
            {
                var data = await (from bu in _context.Users
                                  join b in _context.Branches on bu.BranchId equals b.BranchId
                                  join c in _context.Clients on b.ReferenceId equals c.ClientId
                                  join g in _context.Groups on c.GroupId equals g.GroupId
                                  select new ClientBranchUserViewModal
                                  {
                                      Id = bu.Id,
                                      Name = bu.Name,
                                      Email = bu.Email,
                                      PhoneNumber = bu.PhoneNumber,
                                      IsActive = bu.IsActive,
                                      BranchId = b.BranchId,
                                      BranchName = b.BranchName,
                                      ClientName = c.PrimaryContact,
                                      ClientId = c.ClientId,
                                  }).Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "ClientBranchUser");
                }
                else
                {
                    data.BranchsList = await GetClientBranches();
                }
                return View(data);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ClientBranchUserViewModal clientBranchUserViewModal = new ClientBranchUserViewModal();
            clientBranchUserViewModal.BranchsList = await GetClientBranches();
            return View("Create", clientBranchUserViewModal);
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
                            BranchId=model.BranchId
                        };
                        //model.RoleName = UserRoles.Admin;
                        var result = await _userManager.CreateAsync(user, model.Password);
                        model.RoleName = UserRoles.Admin;
                        if (!result.Succeeded)
                        {
                            TempData["errorMessage"] = "User creation failed! Please check user details and try again.";
                        }
                        if (!await _roleManager.RoleExistsAsync(UserRoles.ClientUser.ToString()))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(UserRoles.ClientUser.ToString()));
                        }
                        await _userManager.AddToRoleAsync(user, UserRoles.ClientUser.ToString());
                        ModelState.Clear();
                        TempData["successMessage"] = "New User Created";
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Create", "ClientBranchUser");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "ClientBranchUser");
        }
        [HttpPost]
        public IActionResult Update(ClientBranchUserViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _context.Users.Where(z => z.Id == model.Id).AsNoTracking().FirstOrDefault();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "ClientBranchUser");
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
                        return RedirectToAction("Index", "ClientBranchUser");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Edit", "ClientBranchUser");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "ClientBranchUser");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ClientBranchUserViewModal clientBranchUserViewModal)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(clientBranchUserViewModal.Id);
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
            return RedirectToAction("Index", "ClientBranchUser");
        }
        private string DisplayError(IdentityResult result)
        {
            List<IdentityError> errorList = result.Errors.ToList();
            var errors = string.Join(", ", errorList.Select(e => e.Description));
            return errors;
        }
        private async Task<List<SelectListItem>> GetClientBranches()
        {
            var data = await (from b in _context.Branches
                              join c in _context.Clients on b.ReferenceId equals c.ClientId
                              join g in _context.Groups on c.GroupId equals g.GroupId
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new
                              {
                                  b.BranchId,
                                  b.BranchName,
                                  gt.GroupTypeName,
                              }).Where(g => g.GroupTypeName == UserRoles.Client.ToString()).ToListAsync();

            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i < data.Count; i++)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = data[i].BranchName;
                selectListItem.Value = data[i].BranchId.ToString();
                list.Add(selectListItem);
            }
            return list;
        }
    }
}