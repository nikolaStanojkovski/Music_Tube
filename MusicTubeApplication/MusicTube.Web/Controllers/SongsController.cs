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
        public IActionResult Index()
        {
            return View(songService.GetAllSongs());
        }

        public async Task<IActionResult> Create()
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            SongDto song = songService.GetSongDto(user);
            return View(song);
        }

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

                songService.CreateSong(user, song, fileName);

                return RedirectToAction("Index", "Songs");
            }
            return View(song);
        }

        // GET: Songs/Details/5
        public IActionResult Details(Guid? songId)
        {
            Song song = songService.ReadSong(songId);

            return View(song);
        }

        // GET: Songs/Delete/5
        public IActionResult Delete(Guid? songId)
        {
            songService.DeleteSong(songId);

            return RedirectToAction("Index", "Songs");
        }

        public async Task<IActionResult> AddSongToAlbum(Guid? songId)
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            SongAlbumDto model = songService.GetSongAlbumDto((Creator)user, songId);

            return View(model);
        }

        [HttpPost]
        public IActionResult AddSongToAlbum([Bind("SongId,AlbumId")] SongAlbumDto model)
        {
            songService.AddSongToAlbum(model);

            return RedirectToAction("Index", "Songs");
        }
    }
}
