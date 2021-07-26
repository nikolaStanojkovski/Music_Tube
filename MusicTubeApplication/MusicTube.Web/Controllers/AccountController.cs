using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Service.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicTube.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<MusicTubeUser> userManager;
        private readonly SignInManager<MusicTubeUser> signInManager;

        private readonly IUserService userService;

        public AccountController(UserManager<MusicTubeUser> _userManager,
            SignInManager<MusicTubeUser> _signInManager,
            IUserService _userService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            userService = _userService;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = userService.GetUserRegistrationDto(null, userManager);

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([Bind("Name,Surname,Genre,FavouriteArtistId,Email,Password,ConfirmPassword")] UserRegistrationDto request, Boolean newsLetters, String role)
        {
            request.NewsletterSubscribed = newsLetters;
            request.Role = role;
            if (ModelState.IsValid)
            {
                var userEmailCheck = await userManager.FindByEmailAsync(request.Email);
                if (userEmailCheck == null)
                {
                    var user = userService.CreateNewUser(request);
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, request.Role);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists");
                    return View(request);
                }
            }
            return View(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed");
                    return View(model);
                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                    true, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Settings()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var model = userService.GetUserSettings(user, userManager);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SettingsProfilePicture([FromForm(Name = "file")] IFormFile file)
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            if (file != null)
            {
                var fileName = user.Id + ".png";

                string pathToUpload = $"{Directory.GetCurrentDirectory()}\\wwwroot\\custom\\img\\profile-pictures\\{fileName}";

                using (FileStream fileStream = System.IO.File.Create(pathToUpload))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }

                UserSettingsDto model = new UserSettingsDto
                {
                    ImageURL = fileName
                };
                userService.UpdateUserPersonalInformation(user, model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SettingsPersonalInfo([Bind("Name,Surname,NewsletterSubscribed,FavouriteGenre,FavouriteArtistId")] UserSettingsDto model, Boolean newsLetters)
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            model.NewsletterSubscribed = newsLetters;
            userService.UpdateUserPersonalInformation(user, model);

            return RedirectToAction("Settings", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> SettingsCreatorInfo([Bind("ArtistName,ArtistDescription")] UserSettingsDto model)
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            userService.UpdateUserPersonalInformation(user, model);

            return RedirectToAction("Settings", "Account");
        }
    
        public IActionResult RemoveReview(Guid? reviewId)
        {
            userService.RemoveReview(reviewId);

            return RedirectToAction("Settings", "Account");
        }

        public async Task<IActionResult> PremiumPlan()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var model = userService.GetPremiumDto((Creator)user);

            return View(model);
        }

        public IActionResult PayPremiumPlan(string stripeEmail, string stripeToken, Int64 sum)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();

            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = sum * 100,
                Description = "MusicTube Application Payment",
                Currency = "eur",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                userService.SetPremium((Creator)user, sum);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Error", "Home");

        }
    }
}
