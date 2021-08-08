using _Core.Interfaces;
using _Core.Utility;
using _Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
                if (upload != null)
                {
                    return Ok(upload);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("checkUserImage/{userId}")]
        public async Task<IActionResult> CheckUserImage(string userId)
        {
            if (ModelState.IsValid)
            {
                var checkImageExist = await _imageManager.UserImageExist(userId);
                  if (checkImageExist == null)
                  {
                    return BadRequest(new ResponseManager()
                    {
                        Message = "No image found",
                        IsSuccess = false
                    });
                  }
                return Ok(new ResponseManager()
                {
                    Message = "User image found",
                    IsSuccess = true
                });
            }
            return BadRequest();
        }
    }
}
