using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO.DomainDTO;
using MusicTube.Domain.Enumerations;
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
        private readonly ISongService songService;
        private readonly UserManager<MusicTubeUser> userManager;

        public VideosController(IVideoService _videoService,
            ISongService _songService,
            UserManager<MusicTubeUser> _userManager)
        {
            this.videoService = _videoService;
            this.songService = _songService;
            this.userManager = _userManager;
        }

        // GET: Videos
        public IActionResult Index(Guid? videoId, string? artistId)
        {
            ViewBag.AllSongs = songService.GetAllSongs();

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
            VideoDto video = videoService.GetVideoDto(user);
            return View(video);
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

        public IActionResult Details(Guid? videoId)
        {
            var model = videoService.GetDetailsDto(videoId);

            return View(model);
        }

        public IActionResult GiveFeedback(Boolean liking, Guid videoId, String comment)
        {
            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;
            videoService.CreateFeedbackForVideo(user, liking, videoId, comment);

            return RedirectToAction("Details", new { videoId = videoId });
        }

        public IActionResult GiveReview(Guid videoId, String summary, String description, String rating)
        {
            var user = (Listener)userManager.FindByEmailAsync(User.Identity.Name).Result;
            videoService.UpdateReviewForVideo(user, videoId, summary, description, rating);

            return RedirectToAction("Details", new { videoId = videoId });
        }

        public IActionResult Delete(Guid? videoId)
        {
            videoService.DeleteVideo(videoId);

            return RedirectToAction("Details", new { videoId = videoId });
        }

        public IActionResult FilterVideos(Guid? songFilter, Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter)
        {
            ViewBag.AllSongs = songService.GetAllSongs();

            List<Video> videos = videoService.FilterVideos(genreFilter, songFilter, nameFilter, descriptionFilter, labelFilter);
            if (videos == null || videos.Count == 0)
                ViewBag.error = "There aren't any videos with the specified filter.";
            return View("Index", videos);
        }

        public IActionResult SortVideos(Boolean? sortCondition)
        {
            ViewBag.AllSongs = songService.GetAllSongs();

            List<Video> videos = videoService.SortVideos(sortCondition);

            if (sortCondition == null || videos == null || videos.Count == 0)
                ViewBag.error = "There aren't any videos with the specified filter.";
           
            return View("Index", videos);
        }

        public IActionResult SearchVideos(String? text)
        {
            ViewBag.AllSongs = songService.GetAllSongs();

            List<Video> videos = videoService.SearchVideos(text);
            if (text == null || videos == null || videos.Count == 0)
                ViewBag.error = "There aren't any videos with the specified filter.";
            return View("Index", videos);
        }
    }
}
