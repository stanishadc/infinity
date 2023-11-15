using InfinityWeb.Models;
using InfinityWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InfinityWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RepositoryContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, RepositoryContext context, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LoginResponse loginResponse = new LoginResponse();
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        if (await _userManager.CheckPasswordAsync(user, model.Password))
                        {
                            var userRoles = await _userManager.GetRolesAsync(user);
                            var authClaims = new List<Claim>
                                    {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //modified                    
                                //new Claim("LoggedOn", DateTime.Now.ToString()),
                                    };
                            foreach (var userRole in userRoles)
                            {
                                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                            }
                            var token = GetToken(authClaims);
                            var roleName = userRoles.FirstOrDefault();
                            HttpContext.Session.SetString("Token", new JwtSecurityTokenHandler().WriteToken(token));
                            HttpContext.Session.SetString("Expiration", token.ValidTo.ToString());
                            HttpContext.Session.SetString("UserId", user.Id);
                            HttpContext.Session.SetString("Role", roleName);
                            HttpContext.Session.SetString("Name", user.Name);

                            var routeValue = new RouteValueDictionary(new { action = "Index", controller = "GroupType" });
                            ModelState.Clear();
                            return RedirectToRoute(routeValue);
                        }
                        else
                        {
                            TempData["errorMessage"] = "Please check the credentials!";
                        }
                    }
                    else
                    {
                        TempData["errorMessage"] = "User not exists!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                }
            }
            else
            {
                TempData["errorMessage"] = "Please check the mandatory fields";
            }
            return View("Index");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region user creation
                    var userExists = await _userManager.FindByNameAsync(model.Email);
                    if (userExists != null)
                    {
                        TempData["errorMessage"] = "Email already exists!";
                    }
                    else
                    {
                        User user = new()
                        {
                            Email = model.Email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Name = model.Name,
                            IsActive = true,
                            BranchId = Guid.Empty
                        };
                        model.RoleName = UserRoles.Admin;
                        var result = await _userManager.CreateAsync(user, model.Password);
                        model.RoleName = UserRoles.Admin;
                        if (!result.Succeeded)
                        {
                            TempData["errorMessage"] = "User creation failed! Please check user details and try again.";
                        }
                        if (!await _roleManager.RoleExistsAsync(model.RoleName))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                        }
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                        ModelState.Clear();
                        //var bemail = Task.Factory.StartNew(() => _emailSender.SendBusinessRegisterEmail(model.Email, model.BusinessName));
                        TempData["successMessage"] = "Account Created Successfully!";
                    }
                    #endregion                    
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error in creating account";
            }            
            return View("Register");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                TempData["errorMessage"] = "Invalid Id";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var data = await _context.Users.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    TempData["errorMessage"] = "No Record Found";
                    return RedirectToAction("Index", "Home");
                }
                return View(data);
            }
        }        
        [HttpPost]
        public async Task<IActionResult> Profile(UserViewModal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _context.Users.Where(z => z.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
                    if (data == null)
                    {
                        TempData["errorMessage"] = "Record not exists!";
                        return RedirectToAction("Update", "Home");
                    }
                    else
                    {

                        data.Name = model.Name;
                        data.Email = model.Email;
                        data.PhoneNumber = model.PhoneNumber;
                        _context.Entry(data).State = EntityState.Modified;
                        _context.SaveChanges();
                        ModelState.Clear();
                        TempData["successMessage"] = "Record Updated Successfully";
                        return RedirectToAction("Profile", "Home");
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Please check the mandatory fields";
                    return RedirectToAction("Edit", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Edit", "Home");
            }
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            changePasswordModel.Id = HttpContext.Session.GetString("UserId");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(changePasswordModel.Id);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.Password);
                    if (result.Succeeded)
                    {
                        TempData["successMessage"] = "Password Updated Successfully";
                        return RedirectToAction("ChangePassword", "Home");
                    }
                    TempData["errorMessage"] = "Error in updating the password";
                    return RedirectToAction("ChangePassword", "Home");
                }
                else
                {
                    TempData["errorMessage"] = "No user error found";
                    return RedirectToAction("ChangePassword", "Home");
                }
            }
            else
            {
                TempData["errorMessage"] = "Please check the mandatory fields";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("UserId");
            return View("Index");
        }
        [HttpPost]
        public IActionResult CancelChangePassword()
        {
            return RedirectToAction("ChangePassword", "Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}