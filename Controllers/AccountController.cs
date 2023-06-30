using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    public class AccountController : Controller
    {
        AccountDAO dao;
        public AccountController()
        {
            dao = new AccountDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ActLogin(string username, string password)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            bool isAuthenticated = false;
            var data = dao.getKaryawan(username);

            if (data != null)
            {
                // data ada masuk ke pengecekan password
                if (data.password == password)
                {
                    // data password sama
                    isAuthenticated = true;
                    identity = new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, data.nama),
                                    new Claim(ClaimTypes.Role, data.deskripsi),
                                    new Claim("username", data.npp),
                                    new Claim("role", data.deskripsi),
                                    new Claim("menu", GenerateMenu(data.deskripsi))
                                }, CookieAuthenticationDefaults.AuthenticationScheme);
                }
                else
                {
                    // password salah
                    TempData["error"] = "Password salah!";
                }
            }
            else
            {
                // data karyawan tidak ditemukan
                TempData["error"] = "Data tidak ditemukan!";
            }

            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                
                if(data.deskripsi == "Pranata Komputer" || data.deskripsi == "Dosen" || data.deskripsi == "Pustakawan" || data.deskripsi == "Administrasi" )
                {
                    return RedirectToAction("SlipGaji", "GajiHonorarium");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }   
            }
            else
            {
                return RedirectToAction("Login");
            }
            //else if (isAuthenticated)
            //{
            //    var principal = new ClaimsPrincipal(identity);
            //    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //    return RedirectToAction("SlipGaji", "GajiHonorarium");
            //}
           
        }

        public IActionResult LogOut()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        protected string GenerateMenu(string npp)
        {
            string menu = "";
            List<MDLMENU> menus = new List<MDLMENU>();
            List<MDLSUBMENU> submenus = new List<MDLSUBMENU>();

            menus = dao.GetMenuKaryawan(npp);
            submenus = dao.GetSubMenuKaryawan(npp);

            if (menu != null)
            {
                foreach (var row in menus)
                {
                    menu += $"<li class='nav-item'><a href = '#' class='nav-link'><i class='nav-icon fas fa-circle'></i><p>{row.DESKRIPSI}<i class='fas fa-angle-left right'></i></p></a><ul class='nav nav-treeview'>";
                    var filtersub = submenus.Where(x => x.ID_SI_MENU == row.ID_SI_MENU).ToList();

                    foreach (var submenu in filtersub)
                    {
                        menu += $"<li class='nav-item'><a href='{submenu.LINK}' class='nav-link'><i class='far fa-circle nav-icon'></i><p>{submenu.DESKRIPSI}</p></a></li>";
                    }

                    menu += "</ul></li> ";
                }
            }

            return menu;
        }
    }
}
