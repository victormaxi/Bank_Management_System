using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Core.Interfaces;
using _Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly IImageManager _imageManager;

        public ImageController(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        [HttpPost("imageUpload")]

        public async Task<IActionResult> ImageUpload(ImageVM imageVM)
        {
            if (ModelState.IsValid)
            {
                var upload = await _imageManager.UploadImage(imageVM);
                if(upload != null)
                {
                    return Ok(upload);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
