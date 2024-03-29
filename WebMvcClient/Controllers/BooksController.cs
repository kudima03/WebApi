﻿using WebMvcClient.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMvcClient.Services;
using Microsoft.AspNetCore.Authentication;

namespace WebMvcClient.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly IBooksHttpClient _booksHttpClient;

        public BooksController(IBooksHttpClient booksHttpClient)
        {
            _booksHttpClient = booksHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            var a = await HttpContext.GetTokenAsync("access_token");
            var books = await _booksHttpClient.GetBooksAsync();
            return Ok();
        }
    }
}
