using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Diagnostics;

namespace FinalProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly CmsContext _context;
        public PostsController(CmsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? page = 1)
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            //每頁資料
            const int pageSize = 5;
            //處理頁數
            ViewBag.postsModel = GetPagedProcess(page, pageSize);
            ////填入頁面資料
            //return View(await _context.TablePosts1111675.Skip<Post>(pageSize * ((page ?? 1) - 1)).Take(pageSize).ToListAsync());

            // プレイヤーデータを含む投稿を取得
            var posts = await GetStuffFromDatabase()
                    .Skip(pageSize * ((page ?? 1) - 1))
                    .Take(pageSize)
                    .ToListAsync();

            // ビューにページング済みのデータを渡す
            return View(posts);
        }
        protected IPagedList<Post> GetPagedProcess(int? page, int pageSize)
        {
            // 過濾從client傳送過來有問題頁數
            if (page.HasValue && page < 1)
                return null;
            // 從資料庫取得資料
            var listUnpaged = GetStuffFromDatabase();
            IPagedList<Post> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);
            // 過濾從client傳送過來有問題頁數,包含判斷有問題的頁數邏輯
            if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount)
                return null;
            return pagelist;
        }
        protected IQueryable<Post> GetStuffFromDatabase()
        {
            return _context.TablePosts1111675.Include(p => p.Player); // Playerをインクルード
            //return _context.TablePosts1111675;
        }

        [HttpGet]
        public IActionResult InputQuery()
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************
            return View();
        }

        [HttpGet]
        public IActionResult Query()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Query(string keyword)
        {
            var posts = await (from p in _context.TablePosts1111675.Include(p => p.Player)
                               where p.Title.Contains(keyword) || p.Content.Contains(keyword)
                               orderby p.Title
                               select p).ToListAsync();


            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> SelectQuery()
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            var places = await (from p in _context.TablePosts1111675
                                orderby p.Place
                                select p.Place).Distinct().ToListAsync();
            ViewBag.Mylist = places;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SelectName(string place)
        {
            var posts = await (from p in _context.TablePosts1111675.Include(p => p.Player)
                               where p.Place == place
                               orderby p.Title
                               select p).ToListAsync();

            return View(posts);
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

            if (Id == null || _context.TablePosts1111675 == null)
            {
                var msgObject = new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    error = "無效的請求.必須提供Id編號"
                };

                return new BadRequestObjectResult(msgObject);
            }

            var post = await _context.TablePosts1111675.FirstOrDefaultAsync(m => m.PostId == Id);
            if (post == null)
                return NotFound();
            return View(post);
        }

        //GET
        [HttpGet]
        public IActionResult Create()
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            ViewBag.Players = new SelectList(_context.TablePlayers1111675, "PlayerId", "PlayerName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,PlayerId,Title,Content,Place,MeetingDateTime")] Post post)
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            Debug.WriteLine("Create action called");

            if (ModelState.IsValid)
            {
                Debug.WriteLine("ModelState is valid");
                try
                {
                    _context.TablePosts1111675.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.Players = new SelectList(_context.TablePlayers1111675, "PlayerId", "PlayerName", post.PlayerId);
            return View(post);
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TablePosts1111675 == null)
            {
                return NotFound();
            }

            var post = await _context.TablePosts1111675.FindAsync(id);

            if (post == null)
                return NotFound();

            ViewBag.Players = new SelectList(_context.TablePlayers1111675, "PlayerId", "PlayerName", post.PlayerId);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,PlayerId,Title,Content,Place,MeetingDateTime")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            ModelState.Remove("Player");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.TablePosts1111675.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Players = new SelectList(_context.TablePlayers1111675, "PlayerId", "PlayerName", post.PlayerId);
            return View(post);
        }

        private bool PostExists(int id)
        {
            return _context.TablePosts1111675.Any(e => e.PostId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            // Session管理 *********************************
            if (HttpContext.Session.GetString("Id") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login1", "Players");
            }
            // end *****************************************

            //檢查是否有提供id
            if (id == null || _context.TablePosts1111675 == null)
            {
                return NotFound();
            }

            //以Id找尋員工資料
            var post = await _context.TablePosts1111675.FirstOrDefaultAsync(m => m.PostId == id);

            //如果沒有找到員工,回傳NotFound
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TablePosts1111675 == null)
            {
                return Problem("Entity set 'CmsContext.Table' is null.");
            }

            //以Id找尋Entity,然後刪除
            var post = await _context.TablePosts1111675.FindAsync(id);

            if (post != null)
            {
                // 將該筆資料移除
                _context.TablePosts1111675.Remove(post);
                await _context.SaveChangesAsync(); //將資料異動儲存到資料庫
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
