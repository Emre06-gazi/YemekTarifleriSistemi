using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using XAct.Security;
using XSystem.Security.Cryptography;
using YemekTarifSistemi.Models;

namespace YemekTarifSistemi.Controllers
{
    [Authorize(Roles ="Yonetici")]
    public class YonetimPanelController : Controller
    {
        YemektarifleriDbContext db = new YemektarifleriDbContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Bilgilerim()
        {
            int kulid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var kullanici = db.Kullanicilars.Find(kulid);
            return View(kullanici);
        }
        public IActionResult BilgilerimGuncelle(Kullanicilar kw)
        {
            var kullanici = db.Kullanicilars.Where(k => k.Silindi == false && k.KullaniciId == kw.KullaniciId).FirstOrDefault();
            kullanici.Aktif = kw.Aktif;
            kullanici.Adi = kw.Adi;
            kullanici.Soyadi = kw.Soyadi;
            kullanici.Eposta = kw.Eposta;
            kullanici.Telefon = kw.Telefon;
            if (!String.IsNullOrEmpty(kw.Parola.Trim()))
            {
                kullanici.Parola = kw.Parola;
            }
            kullanici.Yetki = kw.Yetki;
            db.Kullanicilars.Update(kullanici);
            db.SaveChanges();
            return RedirectToAction("Bilgilerim");
        }
        public IActionResult Sayfalar()
        {
            var sayfalar = db.Sayfalars.Where(s => s.Silindi == false).OrderBy(s => s.Baslik).ToList();
            return View(sayfalar);
        }
        public IActionResult SayfaEkle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SayfaEkle(Sayfalar s)
        {
            s.Silindi = false;
            db.Sayfalars.Add(s);
            db.SaveChanges();
            return RedirectToAction("Sayfalar");
        }
        public IActionResult SayfaGetir(int id)
        {
            var sayfa = db.Sayfalars.Where(s => s.Silindi == false && s.SayfaId == id).FirstOrDefault();
            return View("SayfaGuncelle", sayfa); //SayfaGetir.cshtml'deki action methoduna yönlendir
        }
        public IActionResult SayfaGuncelle(Sayfalar sw)
        {
            var sayfa = db.Sayfalars.Where(s => s.Silindi == false && s.SayfaId == sw.SayfaId).FirstOrDefault();
            sayfa.Baslik = sw.Baslik;
            sayfa.Icerik = sw.Icerik;
            sayfa.Aktif = sw.Aktif;
            db.Sayfalars.Update(sayfa);
            db.SaveChanges();
            return RedirectToAction("Sayfalar");
        }
        public IActionResult SayfaSil(int id)
        {
            var sayfa = db.Sayfalars.Where(sw => sw.Silindi == false && sw.SayfaId == id).FirstOrDefault();
            sayfa.Silindi = true; //Silindi olarak gösterdim
            db.Sayfalars.Update(sayfa);
            db.SaveChanges();
            return RedirectToAction("Sayfalar");
        }
        public IActionResult Kategoriler()
        {
            var kategoriler = db.Kategorilers.Where(k => k.Silindi == false).OrderBy(k => k.Kategoriadi).ToList();
            return View(kategoriler);
        }
        public IActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult KategoriEkle(Kategoriler k)
        {
            k.Silindi = false;
            db.Kategorilers.Add(k);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }
        public IActionResult KategoriGetir(int id)
        {
            var kategori = db.Kategorilers.Where(k => k.Silindi == false && k.KategoriId == id).FirstOrDefault();
            return View("KategoriGuncelle", kategori); //KategoriGetir.cshtml'deki action methoduna yönlendir
        }
        public IActionResult KategoriGuncelle(Kategoriler kw)
        {
            var kategori = db.Kategorilers.Where(k => k.Silindi == false && k.KategoriId == kw.KategoriId).FirstOrDefault();
            kategori.Kategoriadi = kw.Kategoriadi;
            kategori.Aktif = kw.Aktif;
            db.Kategorilers.Update(kategori);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }
        public IActionResult KategoriYemekler(int id)
        {
            var yemekler = db.YemekTarifleris.Include(k => k.Kategori).Where(y => y.Silindi == false && y.KategoriId == id).ToList();
            return View("KategoriYemekler", yemekler); //KategoriGetir.cshtml'deki action methoduna yönlendir
        }
        public IActionResult KategoriSil(int id)
        {
            var kategori = db.Kategorilers.Where(k => k.Silindi == false && k.KategoriId == id).FirstOrDefault();
            kategori.Silindi = true;
            db.Kategorilers.Update(kategori);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }
        public IActionResult Tarifler()
        {
            var tarifler = db.YemekTarifleris.Include(k => k.Kategori).Where(t => t.Silindi == false).OrderBy(t => t.Yemekadi).ToList(); //İnclude ile kategori adını getirdik.
            return View(tarifler);
        }
        public IActionResult TarifEkle()
        {
            var kategoriler = (from k in db.Kategorilers.Where(k => k.Silindi == false && k.Aktif == true).ToList()
                               select new SelectListItem
                               {
                                   Text = k.Kategoriadi,
                                   Value = k.KategoriId.ToString(),
                               });
            ViewBag.KategoriId = kategoriler; // ViewBag ile veriyi Controllerdan View'a taşıyorum.
            return View();
        }
        [HttpPost]
        public IActionResult TarifEkle(YemekTarifleri t)
        {
            t.Silindi = false;
            t.Eklemetarihi = DateTime.Now;
            db.YemekTarifleris.Add(t);
            db.SaveChanges();
            return RedirectToAction("Tarifler");
        }
        public IActionResult TarifGetir(int id)
        {
            var tarif = db.YemekTarifleris.Include(k => k.Kategori).Where(t => t.Silindi == false && t.TarifId == id).FirstOrDefault();
            var kategoriler = (from k in db.Kategorilers.Where(k => k.Silindi == false && k.Aktif == true).ToList()
                               select new SelectListItem
                               {
                                   Text = k.Kategoriadi,
                                   Value = k.KategoriId.ToString(),
                               });
            ViewBag.KategoriId = kategoriler; // ViewBag ile veriyi Controllerdan View'a taşıyorum.
            return View("TarifGuncelle", tarif); //TarifGuncelle.cshtml'deki action methoduna yönlendir
        }
        public IActionResult TarifGuncelle(YemekTarifleri tw)
        {
            var tarif = db.YemekTarifleris.Where(t => t.Silindi == false && t.TarifId == tw.TarifId).FirstOrDefault();
            tarif.Yemekadi = tw.Yemekadi;
            tarif.Tarif = tw.Tarif;
            tarif.Sira = tw.Sira;
            tarif.Aktif = tw.Aktif;
            tarif.KategoriId = tw.KategoriId;
            db.YemekTarifleris.Update(tarif);
            db.SaveChanges();
            return RedirectToAction("Tarifler");
        }
        public IActionResult TarifYorumlari(int id)
        {
            var yorumlar = db.Yorumlars.Where(t => t.Silindi == false && t.TarifId == id).ToList();
            return View("Yorumlar", yorumlar); //Yorumlar.cshtml'deki action methoduna yönlendir
        }

        public IActionResult TarifSil(int id)
        {
            var tarif = db.YemekTarifleris.Where(t => t.Silindi == false && t.TarifId == id).FirstOrDefault();
            tarif.Silindi = true;
            db.YemekTarifleris.Update(tarif);
            db.SaveChanges();
            return RedirectToAction("Tarifler");
        }
        [HttpGet]
        public IActionResult Yorumlar()
        {
            var yorumlar = db.Yorumlars.Include(t => t.TarifNavigation).Include(u => u.Uye).Where(y => y.Silindi == false).OrderByDescending(y => y.Eklemetarihi).ToList(); //Yorum yapılan tarihe göre sıralamak için OrderByDescending
            return View(yorumlar);
        }
        [HttpPost]
        public IActionResult Yorumlar(string listelemetürü) //Yorum Filtreleme
        {
            var yorumlar = db.Yorumlars.Include(t => t.TarifNavigation).Include(u => u.Uye).Where(y => y.Silindi == false).OrderByDescending(y => y.Eklemetarihi).ToList(); //Yorum yapılan tarihe göre sıralamak için OrderByDescending
            switch (listelemetürü)
            {
                case "Onayli":
                    yorumlar = db.Yorumlars.Include(t => t.TarifNavigation).Include(u => u.Uye).Where(y => y.Silindi == false && y.Aktif == true).OrderByDescending(y => y.Eklemetarihi).ToList();
                    break;
                case "Onayisz":
                    yorumlar = db.Yorumlars.Include(t => t.TarifNavigation).Include(u => u.Uye).Where(y => y.Silindi == false && y.Aktif == false).OrderByDescending(y => y.Eklemetarihi).ToList();
                    break;
            }
            return View(yorumlar);
        }
        public IActionResult YorumSil(int id)
        {
            var yorum = db.Yorumlars.Where(y => y.Silindi == false && y.YorumId == id).FirstOrDefault();
            yorum.Silindi = true;
            db.Yorumlars.Update(yorum);
            db.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
        public IActionResult YorumOnayla(int id)
        {
            var yorum = db.Yorumlars.Where(y => y.Silindi == false && y.YorumId == id).FirstOrDefault();
            yorum.Aktif = Convert.ToBoolean((-1 * Convert.ToInt32(yorum.Aktif)) + 1);  //İnce bir hesap (yorum.Aktif ' in 0 ve 1 gelme durumuna göre ters çevirme işlemi. (0 geliyorsa 1, 1 se 0)
            db.Yorumlars.Update(yorum);
            db.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
        public IActionResult Kullanicilar()
        {
            var kullanicilar = db.Kullanicilars.Where(k => k.Silindi == false).OrderBy(k => k.Yetki).OrderBy(k => k.Adi).OrderBy(k => k.Soyadi).ToList();
            return View(kullanicilar);
        }
        public IActionResult KullaniciEkle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult KullaniciEkle(Kullanicilar k)
        {
            k.Silindi = false;
            db.Kullanicilars.Add(k);
            db.SaveChanges();
            return RedirectToAction("Kullanicilar");
        }
        public IActionResult KullaniciGetir(int id)
        {
            var kullanici = db.Kullanicilars.Where(k => k.Silindi == false && k.KullaniciId == id).FirstOrDefault();
            return View("KullaniciGuncelle", kullanici); 
        }
        public IActionResult KullaniciGuncelle(Kullanicilar kw)
        {
            var kullanici = db.Kullanicilars.Where(k=>k.Silindi == false && k.KullaniciId == kw.KullaniciId).FirstOrDefault();
            kullanici.Aktif = kw.Aktif;
            kullanici.Adi = kw.Adi;
            kullanici.Soyadi = kw.Soyadi;
            kullanici.Eposta = kw.Eposta;
            kullanici.Telefon = kw.Telefon;
            if (!String.IsNullOrEmpty(kw.Parola.Trim()))
            {
                kullanici.Parola = kw.Parola;
            }
            kullanici.Yetki = kw.Yetki;
            db.Kullanicilars.Update(kullanici);
            db.SaveChanges();
            return RedirectToAction("Kullanicilar");
        }

        public IActionResult KullaniciSil(int id)
        {
            var kullanici = db.Kullanicilars.Where(k => k.Silindi == false && k.KullaniciId == id).FirstOrDefault();
            kullanici.Silindi = true;
            db.Kullanicilars.Update(kullanici);
            db.SaveChanges();
            return RedirectToAction("Kullanicilar");
        }
        public IActionResult CikisYap()
        {
            return View();
        }
    }
}
