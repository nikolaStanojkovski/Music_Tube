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
using MusicTube.Domain.DTO.DomainDTO;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using MusicTube.Repository;
using MusicTube.Service.Interface;

namespace MusicTube.Web.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ISongService songService;
        private readonly IAlbumService albumService;
        private readonly UserManager<MusicTubeUser> userManager;

        public SongsController(ISongService _songService,
            IAlbumService _albumService,
            UserManager<MusicTubeUser> _userManager)
        {
            this.songService = _songService;
            this.userManager = _userManager;
            this.albumService = _albumService;
        }

        // GET: Songs
        public IActionResult Index(Guid? albumId, Guid? songId, string? artistId)
        {
            if (albumId == null && songId == null && artistId == null)
                return View(songService.GetAllSongs());
            else if(artistId != null)
            {
                List<Song> songs = songService.GetSongsForArtist(artistId);
                if (songs.Count == 0)
                    ViewBag.error = "The selected artist still doesn't have any uploaded songs.";

                return View(songs);
            }
            else if(albumId != null)
            {
                List<Song> songs = albumService.GetSongsForAlbum(albumId);
                if (songs.Count == 0)
                    ViewBag.error = "The selected album still doesn't have any songs in it.";

                return View(songs);
            } else
            {
                List<Song> songs = new List<Song>();
                songs.Add(songService.ReadSong(songId));

                return View(songs);
            }
        }

        public async Task<IActionResult> Create()
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            SongDto song = songService.GetCreateDto(user);
            return View(song);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
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

                await songService.CreateSong(user, song, fileName);

                return RedirectToAction("Index", "Songs");
            }
            return View(song);
        }

        public async Task<IActionResult> Edit(Guid? songId)
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            SongDto songDto = songService.GetEditDto(user, songId);
            return View(songDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,Name,Label,AlbumId,Description,Genre")] SongDto song)
        {
            if(ModelState.IsValid)
            {
                var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
                await songService.EditSong(user, song);
                return RedirectToAction("Details", "Songs", new { songId = song.Id });
            }

            return View(song);
        }

        public IActionResult Details(Guid? songId)
        {
            var model = songService.GetDetailsDto(songId);

            return View(model);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(Guid? songId)
        {
            await songService.DeleteSong(songId);

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

            return RedirectToAction("Details", "Songs", new { songId = model.SongId });
        }

        public IActionResult GiveFeedback(Boolean liking, Guid songId, String comment)
        {
            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;
            songService.CreateFeedbackForSong(user, liking, songId, comment);

            return RedirectToAction("Details", "Songs", new { songId = songId });
        }

        public IActionResult GiveReview(Guid songId, String summary, String description, String rating)
        {
            var user = (Listener) userManager.FindByEmailAsync(User.Identity.Name).Result;
            songService.UpdateReviewForSong(user, songId, summary, description, rating);

            return RedirectToAction("Details", "Songs", new { songId = songId });
        }

        public IActionResult FilterSongs(Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter)
        {
            List<Song> songs = songService.FilterSongs(genreFilter, nameFilter, descriptionFilter, labelFilter);
            if (songs == null || songs.Count == 0)
                ViewBag.error = "There aren't any songs with the specified filter.";
            return View("Index", songs);
        }

        public IActionResult SortSongs(Boolean sortCondition)
        {
            List<Song> songs = songService.SortSongs(sortCondition);
            if (songs == null || songs.Count == 0)
                ViewBag.error = "There aren't any songs with the specified filter.";
            return View("Index", songs);
        }

        public IActionResult SearchSongs(String text)
        {
            List<Song> songs = songService.SearchSongs(text);
            if (songs == null || songs.Count == 0)
                ViewBag.error = "There aren't any songs with the specified filter.";
            return View("Index", songs);
        }
    }
}
