using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestSystem.API.DTOs;
using System;
using System.Linq;
using System.Text;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    //[Authorize]
    public class CityController : BaseController
    {
        private static readonly string[] Cities = new[]
        {
            "Atlanta", "New York", "Pune", "Bengaluru"
        };
        private IWebHostEnvironment _environment;

        private readonly DataContext dataContext;

        private readonly ILogger<CityController> logger;

        public CityController(ILogger<CityController> logger, DataContext dataContext, IWebHostEnvironment environment)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cities = dataContext.Cities.ToList() ;

            List<CityDTO> dtos = new List<CityDTO>();

            foreach (City city in cities)
            {
                (string url, string base64String) image = ConvertImageToBase64();
                dtos.Add(new CityDTO() { Id = city.Id, Name = city.Name, ImageUrl = image.url, CityImage = image.base64String });
            }

            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Cities[id];
        }

        private (string url, string base64String) ConvertImageToBase64(string imageUrl=null)
        {
            string base64String = string.Empty;
            var fileUrl = Path.Combine("Resources", "images");
            try
            {
                
                var webRootPath = this._environment.WebRootPath;
                var path = Path.Combine(webRootPath, fileUrl);

                if (imageUrl == null)
                {
                    imageUrl = "defaultProfileImage.png";
                    fileUrl = Path.Combine(fileUrl,imageUrl);
                }

                if (Directory.Exists(path))
                {
                    var fileWithPath = Path.Combine(path, imageUrl);
                    byte[] fileArray = System.IO.File.ReadAllBytes(fileWithPath);
                    base64String = Convert.ToBase64String(fileArray);
                }
            }
            catch (Exception ex)
            {
                return (fileUrl ,"Error has occurred");
            }

            return (fileUrl,base64String);
        }
    }
}
