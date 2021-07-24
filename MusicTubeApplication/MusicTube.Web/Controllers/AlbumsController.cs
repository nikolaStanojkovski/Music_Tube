﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicTube.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly UserManager<MusicTubeUser> userManager;
        private readonly IAlbumService albumService;

        public AlbumsController(IAlbumService _albumService,
            UserManager<MusicTubeUser> _userManager)
        {
            this.albumService = _albumService;
            this.userManager = _userManager;
        }

        public IActionResult Index()
        {
            return View(albumService.GetAllAlbums());
        }

        public IActionResult Create()
        {
            var user = userManager.FindByEmailAsync(User.Identity.Name).Result;

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
            return null;
        }
    }
}