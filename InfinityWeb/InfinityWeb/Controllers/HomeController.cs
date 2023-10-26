using InfinityWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
                            Name = model.Name
                        };
                        //model.RoleName = UserRoles.Admin;
                        var result = await _userManager.CreateAsync(user, model.Password);

                        if (!result.Succeeded)
                        {
                            TempData["errorMessage"] = "User creation failed! Please check user details and try again.";
                        }
                        if (!await _roleManager.RoleExistsAsync(model.RoleName))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                        }
                        await _userManager.AddToRoleAsync(user, model.RoleName);

                    }
                    #endregion
                    //var bemail = Task.Factory.StartNew(() => _emailSender.SendBusinessRegisterEmail(model.Email, model.BusinessName));
                    TempData["successMessage"] = "Account Created Successfully!";
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
            ModelState.Clear();
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