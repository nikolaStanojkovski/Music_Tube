using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Web.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly UserManager<MusicTubeUser> userManager;
        private readonly IUserService userService;

        public ArtistsController(UserManager<MusicTubeUser> _userManager,
            IUserService _userService)
        {
            this.userManager = _userManager;
            this.userService = _userService;
        }

        public IActionResult Index(string error = "")
        {
            ViewBag.error = error;
            List<Creator> creators = userService.GetAllCreators();
            return View(creators);
        }

        public IActionResult ViewSongs(string? artistId)
        {
            return RedirectToAction("Index", "Songs", new { artistId = artistId });
        }

        public IActionResult ViewAlbums(string? artistId)
        {
            return RedirectToAction("Index", "Albums", new { artistId = artistId });
        }

        public IActionResult ViewVideos(string? artistId)
        {
            return RedirectToAction("Index", "Videos", new { artistId = artistId });
        }

        [HttpPost]
        public IActionResult ExportArtistsByGenre([Bind("genre")] Genre genre)
        {
            List<Creator> filteredArtists = userService.FilterCreatorsByGenre(genre);

            var fileName = "Artists_Genre_" + genre.ToString() + ".xlsx";
            var contentType = "application/vnd.ms-excel";

            if (filteredArtists.Count == 0)
                return RedirectToAction("Index", new { error = "There are no artists with the specified genre to export." });

            var workbook = WriteToCSV(filteredArtists);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, contentType, fileName);
        }

        [HttpPost]
        public IActionResult ExportAllArtists()
        {
            List<Creator> filteredArtists = userService.GetAllCreators();

            var fileName = "Artists_All_Genres.xlsx";
            var contentType = "application/vnd.ms-excel";

            if (filteredArtists.Count == 0)
                return RedirectToAction("Index", "Artists", new { error = "There are no artists to export." });

            var workbook = WriteToCSV(filteredArtists);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, contentType, fileName);
        }

        private XLWorkbook WriteToCSV(List<Creator> filteredArtists)
        {
            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Album_All_Genres");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Surname";
            worksheet.Cell(1, 4).Value = "Art Name";
            worksheet.Cell(1, 5).Value = "Favourite Genre";
            worksheet.Cell(1, 6).Value = "No. Fans";
            worksheet.Cell(1, 7).Value = "Songs";
            worksheet.Cell(1, 8).Value = "Videos";
            worksheet.Cell(1, 9).Value = "Albums";

            for (int i = 1; i <= filteredArtists.Count; i++)
            {
                var item = filteredArtists[i - 1];
                item = userService.GetCreator(item.Id);

                String artistAlbums = userService.GetAlbumsAsString(item);
                String artistSongs = userService.GetSongsAsString(item);
                String artistVideos = userService.GetVideosAsString(item);

                worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                worksheet.Cell(i + 1, 2).Value = item.Name;
                worksheet.Cell(i + 1, 3).Value = item.Surname;
                worksheet.Cell(i + 1, 4).Value = item.ArtistName;
                worksheet.Cell(i + 1, 5).Value = item.FavouriteGenre;
                worksheet.Cell(i + 1, 6).Value = item.Fans.Count;

                worksheet.Cell(i + 1, 7).Value = artistSongs;
                worksheet.Cell(i + 1, 8).Value = artistVideos;
                worksheet.Cell(i + 1, 9).Value = artistAlbums;
            }

            return workbook;
        }
    }
}
