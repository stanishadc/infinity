﻿using InfinityWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            if (model.Email == null)
            {
                ViewBag.ErrorMessage = "Please enter email";
            }
            else if (model.Password == null)
            {
                ViewBag.ErrorMessage = "Please enter password";
            }
            else
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
                            loginResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
                            loginResponse.Expiration = token.ValidTo;
                            loginResponse.UserId = user.Id;
                            loginResponse.Role = roleName;
                            loginResponse.Status = true;
                            loginResponse.Name = user.Name;
                            var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Lead" });
                            return RedirectToRoute(routeValue);
                        }
                        else
                        {
                            loginResponse.Status = false;
                            ModelState.AddModelError("Error", "Please check the credentials!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "User not exists!");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            return View("Index", model);
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