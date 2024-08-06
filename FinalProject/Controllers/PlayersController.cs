using FinalProject.Data;
using FinalProject.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers
{
    public class PlayersController : Controller
    {
        private readonly CmsContext _context;
        public PlayersController(CmsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            var users = await _context.TablePlayers1111675.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoginCheck()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginCheck(int playerId, string playerPassword)
        {
            var users = await (from p in _context.TablePlayers1111675
                               where p.PlayerId == playerId && p.PlayerPassword == playerPassword
                               orderby p.PlayerName
                               select p).ToListAsync();


            if (users.Count != 0)
                return RedirectToAction("Index");

            return View(users);
        }

        [HttpGet]
        public IActionResult Page1()
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Page2()
        {
            HttpContext.Session.SetString("Id", "Logined");
            TempData["message"] = "Logged in!";

            return View();
        }

        [HttpGet]
        public IActionResult Login1()
        {
            TempData["message"] = null; // Clear any previous message
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login1(string playerId, string playerPassword)
        {
            if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerPassword))
            {
                TempData["message"] = "Please enter account and password!";
                return RedirectToAction("Login1");
            }

            var userId = int.TryParse(playerId, out int parsedPlayerId) ? parsedPlayerId : (int?)null;
            var users = await _context.TablePlayers1111675
                .Where(p => p.PlayerId == userId && p.PlayerPassword == playerPassword)
                .OrderBy(p => p.PlayerName)
                .ToListAsync();

            if (users.Any())
            {
                HttpContext.Session.SetString("Id", playerId);
                TempData["message"] = "Logged in!";
                return RedirectToAction("Index", "Posts");
            }
            else
            {
                TempData["message"] = "Login failed!";
                return RedirectToAction("Login1");
            }
        }


        public async Task<IActionResult> Details(int? Id)
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            if (Id == null || _context.TablePlayers1111675 == null)
            {
                var msgObject = new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    error = "無效的請求.必須提供Id編號"
                };

                return new BadRequestObjectResult(msgObject);
            }

            var player = await _context.TablePlayers1111675.FirstOrDefaultAsync(m => m.PlayerId == Id);
            if (player == null)
                return NotFound();
            return View(player);
        }

        //GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerId,PlayerName,City,Sex,Birthday,Email,Photo,PlayerPassword")] Player player)
        {

            //用ModelState.IsValid判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                //將entity加入DbSet
                _context.TablePlayers1111675.Add(player);
                //將資料異動儲存到資料庫
                await _context.SaveChangesAsync();
                //導向至Index動作方法
                return RedirectToAction(nameof(Index));
            }

            return View(player);
        }

    }




}
