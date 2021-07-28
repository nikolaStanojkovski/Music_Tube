using ClosedXML.Excel;
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
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly UserManager<MusicTubeUser> userManager;
        private readonly IAlbumService albumService;
        private readonly IUserService userService;

        public AlbumsController(IAlbumService _albumService,
            UserManager<MusicTubeUser> _userManager,
            IUserService _userService)
        {
            this.albumService = _albumService;
            this.userManager = _userManager;
            this.userService = _userService;
        }

        public IActionResult Index(string? artistId, string error = "")
        {
            if(artistId == null)
            {
                ViewBag.error = error;

                return View(albumService.GetAllAlbums());
            } else
            {
                List<Album> albums = albumService.GetAlbumsForUser(userService.GetCreator(artistId));
                if (albums == null || albums.Count == 0)
                    ViewBag.error = "The selected artist still doesn't have any uploaded albums.";
                return View(albums);
            }
        }

        public IActionResult Create()
        {
            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;
            
            if (albumService.CheckAlbumLimit((Creator)user))
                return RedirectToAction("Index", new { error = "Sorry, your album creation limit is reached, make a subscription again to enable album creation again." });
            else
                return View(albumService.GetAlbumDto((Creator)user));
        }

        [HttpPost]
        public IActionResult Create([Bind("PremiumUserId,AlbumName,AlbumCoverArt,AlbumArranger,AlbumComposer,AlbumProducer,AlbumMixMasterEngineer,AlbumGenre")] AlbumDto model)
        {
            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;
            albumService.AddNewAlbum((Creator)user, model);

            return RedirectToAction("Index", "Albums");
        }

        public IActionResult Delete(Guid? albumId)
        {
            albumService.DeleteAlbum(albumId);

            return RedirectToAction("Index", "Albums");
        }

        public IActionResult ViewSongs(Guid? albumId)
        {
            return RedirectToAction("Index", "Songs", new { albumId = albumId });
        }


        [HttpPost]
        public IActionResult ExportAlbumsByGenre([Bind("genre")] Genre genre)
        {
            List<Album> filteredAlbums = albumService.FilterAlbumsByGenre(genre);

            var fileName = "Albums_Genre_" + genre.ToString() + ".xlsx";
            var contentType = "application/vnd.ms-excel";

            if (filteredAlbums.Count == 0)
                return RedirectToAction("Index", new { error = "There are no albums with the specified genre to export." });

            var workbook = WriteToCSV(filteredAlbums);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, contentType, fileName);
        }

        [HttpPost]
        public IActionResult ExportAllAlbums()
        {
            List<Album> filteredAlbums = albumService.GetAllAlbums();

            var fileName = "Album_All_Genres.xlsx";
            var contentType = "application/vnd.ms-excel";

            if (filteredAlbums.Count == 0)
                return RedirectToAction("Index", "Albums", new { error = "There are no albums to export." });

            var workbook = WriteToCSV(filteredAlbums);

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, contentType, fileName);
        }

        private XLWorkbook WriteToCSV(List<Album> filteredAlbums)
        {
            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Album_All_Genres");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Creator";
            worksheet.Cell(1, 4).Value = "Art URL";
            worksheet.Cell(1, 5).Value = "Genre";
            worksheet.Cell(1, 6).Value = "Arranger";
            worksheet.Cell(1, 7).Value = "Composer";
            worksheet.Cell(1, 8).Value = "Producer";
            worksheet.Cell(1, 9).Value = "Mix/Master Engineer";
            worksheet.Cell(1, 10).Value = "Songs";

            for (int i = 1; i <= filteredAlbums.Count; i++)
            {
                var item = filteredAlbums[i - 1];

                var premiumUser = userService.ReadPremiumUser(item.PremiumUserId);
                List<Song> albumSongs = albumService.GetSongsForAlbum(item.Id);

                worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                worksheet.Cell(i + 1, 2).Value = item.AlbumName;
                worksheet.Cell(i + 1, 3).Value = premiumUser.ArtistName;
                worksheet.Cell(i + 1, 4).Value = item.AlbumCoverArt;
                worksheet.Cell(i + 1, 5).Value = item.AlbumGenre;

                worksheet.Cell(i + 1, 6).Value = item.AlbumArranger;
                worksheet.Cell(i + 1, 7).Value = item.AlbumComposer;
                worksheet.Cell(i + 1, 8).Value = item.AlbumProducer;
                worksheet.Cell(i + 1, 9).Value = item.AlbumMixMasterEngineer;

                if(albumSongs == null || albumSongs.Count == 0)
                {
                    worksheet.Cell(i + 1, 10).Value = "There are still no songs in this album";
                } else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 1; j <= albumSongs.Count; j++)
                    {
                        Song song = albumSongs[j - 1];
                        StringBuilder innerSb = new StringBuilder();
                        innerSb.Append(j).Append(". ").Append(song.Name).Append(" (")
                            .Append(song.Description).Append(") - Genre: ").Append(song.Genre)
                            .Append(", Label: ").Append(song.Label);

                        sb.AppendLine(innerSb.ToString());
                    }
                    worksheet.Cell(i + 1, 10).Value = sb.ToString();
                }
                
            }

            return workbook;
        }

    }
}
