using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Repository;
using MusicTube.Service.Interface;

namespace MusicTube.Web.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ISongService songService;
        private readonly UserManager<MusicTubeUser> userManager;

        public SongsController(ApplicationDbContext context,
            ISongService _songService,
            UserManager<MusicTubeUser> _userManager)
        {
            _context = context;
            this.songService = _songService;
            this.userManager = _userManager;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            return View(songService.GetAllSongs());
        }

        // GET: Songs/Create
        public async Task<IActionResult> Create()
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            SongDto song = songService.GetSongDto(user);
            return View(song);
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Label,AlbumId,Description,Genre")] SongDto song, IFormFile songToUpload)
        {
            if (ModelState.IsValid && songToUpload != null)
            {
                var user = (Creator) await userManager.FindByEmailAsync(User.Identity.Name);

                var fileName = songToUpload.FileName;
                string pathToUpload = $"{Directory.GetCurrentDirectory()}\\wwwroot\\custom\\files\\audio\\{fileName}";
                using (FileStream fileStream = System.IO.File.Create(pathToUpload))
                {
                    songToUpload.CopyTo(fileStream);
                    fileStream.Flush();
                }

                songService.CreateNewSong(user, song, fileName);

                return RedirectToAction("Index", "Songs");
            }
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Set<Creator>(), "Id", "Id", song.CreatorId);
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "AlbumArranger", song.AlbumId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Length,AudioQuality,AudioSampleRate,AudioURL,AlbumId,Name,Description,Label,Genre,CreatorId,Id")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Set<Creator>(), "Id", "Id", song.CreatorId);
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "AlbumArranger", song.AlbumId);
            return View(song);
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Creator)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Creator)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var song = await _context.Songs.FindAsync(id);
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(Guid id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
