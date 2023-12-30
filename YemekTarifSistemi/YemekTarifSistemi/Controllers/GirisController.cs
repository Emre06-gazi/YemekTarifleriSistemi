using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekTarifSistemi.Models;

namespace YemekTarifSistemi.Controllers
{
    public class GirisController : Controller
    {
        public IActionResult GirisYap()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GirisYap(Kullanicilar k, string ReturnUrl)
        {

            YemektarifleriDbContext db = new YemektarifleriDbContext();
            var kullanici = db.Kullanicilars.FirstOrDefault(kul=>kul.Eposta == k.Eposta && kul.Parola==k.Parola && kul.Silindi==false && kul.Aktif == true);
            if(kullanici != null)
            {
                string yetki = (bool)kullanici.Yetki ? "Yonetici" : "Uye";

                var talepler = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, kullanici.Eposta.ToString()),
                    new Claim(ClaimTypes.Role, yetki),
                    new Claim(ClaimTypes.NameIdentifier,kullanici.KullaniciId.ToString())
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(talepler, "Login");   
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if(!String.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    if ((bool)kullanici.Yetki)
                    {
                        return Redirect("/YonetimPanel/Index");
                    }
                    else
                    {
                        return Redirect("/Home/Index");
                    }
                }
            }

            return View();
        }
        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
