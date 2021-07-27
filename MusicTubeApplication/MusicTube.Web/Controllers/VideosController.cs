using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicTube.Web.Controllers
{
    public class VideosController : Controller
    {
        private readonly IVideoService videoService;
        private readonly UserManager<MusicTubeUser> userManager;

        public VideosController(IVideoService _videoService,
            UserManager<MusicTubeUser> _userManager)
        {
            this.videoService = _videoService;
            this.userManager = _userManager;
        }

        // GET: Videos
        public IActionResult Index(Guid? videoId, string? artistId)
        {
            if (videoId == null && artistId == null)
                return View(videoService.GetAllVideos());
            else if (artistId != null)
            {
                List<Video> videos = videoService.GetVideosForArtist(artistId);
                if (videos.Count == 0)
                    ViewBag.error = "The selected artist still doesn't have any uploaded videos.";

                return View(videos);
            }
            else
            {
                List<Video> videos = new List<Video>();
                videos.Add(videoService.ReadVideo(videoId));

                return View(videos);
            }
        }

        // CREATE: Video
        public async Task<IActionResult> Create()
        {
            var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);
            VideoDto song = videoService.GetVideoDto(user);
            return View(song);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([Bind("Name,Label,SongId,Description,Genre")] VideoDto video, IFormFile videoToUpload)
        {
            if (ModelState.IsValid && videoToUpload != null)
            {
                var user = (Creator)await userManager.FindByEmailAsync(User.Identity.Name);

                var fileName = videoToUpload.FileName;
                string pathToUpload = $"{Directory.GetCurrentDirectory()}\\wwwroot\\custom\\files\\video\\{fileName}";
                using (FileStream fileStream = System.IO.File.Create(pathToUpload))
                {
                    videoToUpload.CopyTo(fileStream);
                    fileStream.Flush();
                }

                videoService.CreateVideo(user, video, fileName);

                return RedirectToAction("Index", "Videos");
            }
            return View(video);
        }
    }
}
