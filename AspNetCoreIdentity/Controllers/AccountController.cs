using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Helper;
using AspNetCoreIdentity.Models.Account;
using AspNetCoreIdentity.Models.RabbitMQModel;
using AspNetCoreIdentity.RabbitMQ.Interface;
using AspNetCoreIdentity.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMessageSend _messageSend;

        private readonly IRabbitMQProducer _rabbitMQProducer;
        private readonly IRabbitMQConsume _rabbitMQConsume;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IMessageSend messageSend, IRabbitMQProducer rabbitMQProducer, IRabbitMQConsume rabbitMQConsume)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _messageSend = messageSend;
            _rabbitMQProducer = rabbitMQProducer;
            _rabbitMQConsume = rabbitMQConsume;
        }


        #region Register
        public IActionResult Register()
        {
            return View(new UserCreateModel());
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCreateModel model)
        {
            if (!ModelState.IsValid) return View(model); ;

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            if (model.Password == null) return BadRequest();

            var identity = await _userManager.CreateAsync(user, model.Password);

            if (identity.Succeeded)
            {
                var role = new IdentityRole
                {
                    Name = "SuperAdmin"
                };

                await _userManager.AddToRoleAsync(user, "SuperAdmin");

                var appUser = await _userManager.FindByEmailAsync(user.Email);

                if (appUser == null) return View(model);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action(nameof(VerifyEmail), "Account", new { userId = user.Id, token = code }, Request.Scheme, Request.Host.ToString());

                //_messageSend.MimeKitConfrim(appUser, url, code);

                //RabbitMQ
                var message = new RegisterRabbitMQModel { AppUser = user,Url = url,Token = code };

                _rabbitMQProducer.ConfrimEmail(message);
                _rabbitMQConsume.ConsumeConfrimEmail();

                TempData["Email"] = "E-mail'e gelmis linki tesdiqliyin.";
                return View();
            }

            foreach (var error in identity.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();

            AppUser? user = await _userManager.FindByIdAsync(userId);

            if (user is null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);

            TempData["Ok"] = "Qeydiyyat ugurla tamamlandi.";
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View(new UserLoginModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new AppUser();

            user = await _userManager.FindByEmailAsync(model.UsernameorEmail);
            if (user == null)
                user = await _userManager.FindByNameAsync(model.UsernameorEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Istifadeci tapilmadi");
                return View(model);
            }

            var identity = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            
            if (identity.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (identity.IsLockedOut)
            {
                var lockedtime = await _userManager.GetLockoutEndDateAsync(user);
                var minute = (lockedtime.Value.UtcDateTime - DateTime.UtcNow).Minutes;

                ModelState.AddModelError("", $"Cox sayda yalnis sifre daxil etdiyinize gore hesabiniz muveqqeti olaraq baglanmisdir." +
                                                $"Hesab {minute} deq sonra yeniden aktiv olacaq.");
            }
            else
            {
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", $"Qeydiyyat zamani yazdiginiz e-mail'i tesdiq edin." +
                                                    $"Eks halda hesaba giris ede bilmeyeceksiz." +
                                                    $"E-mail unvani: {user.Email}");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "UsernameorEmail ve Password yalnisdir.");
                }

                //if (user != null)
                //{
                //    var failedcount = await _userManager.GetAccessFailedCountAsync(user);
                //    var count = _userManager.Options.Lockout.MaxFailedAccessAttempts - failedcount;

                //    ModelState.AddModelError("", $"{count} defe de yalnis giris etseniz hesabiniz muveqqeti olarag baglanacaq.");
                //}
            }
            return View(model);
        }
        #endregion


        #region Logout AccessDenied
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion


        #region Forgot Password
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) return View(model);

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token = code }, Request.Scheme);

            _messageSend.MimeMessageResetPassword(user, url, code);

            TempData["ForgotPassword"] = "Email'e gelen linki klik edin.";
            return View();
        }

        public IActionResult ResetPassword(string Token, string Email)
        {
            if (Token == null || Email == null)
            {
                TempData["ForgotPassword"] = "Bele bir istifadeci tapilmadi.";
                return RedirectToAction("ForgotPassword", "Account");
            }
            var reset = new ResetPasswordModel { Email = Email };

            return View(reset);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            //Validation
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || model.Token == null)
            {
                TempData["Ok"] = "Bele bir istifadeci tapilmadi.";
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Ok"] = "Sifre ugurla deyismisdir.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        #endregion
    }
}
