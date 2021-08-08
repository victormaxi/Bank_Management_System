using _Core.Interfaces;
using _Core.Models;
using _Core.Utility;
using _Core.ViewModels;
using _Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _Domain.ImageManager
{
    public class ImageManager : IImageManager
    {

        private readonly ApplicationDbContext _dbContext;

        public ImageManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<object> UserImageExist(string userId)
        {
           try
            {
                var image = await _dbContext.ImageFiles.FindAsync(userId);

                if (image != null)
                {
                    return new ResponseManager
                    {
                        Message = "User has Image",
                        IsSuccess = true
                    };
                }

                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async  Task<object> UploadImage(ImageVM imageVM)
        {
            try
            {
                var image = new ImageFile
                {
                    
                    ImagePath = imageVM.ImagePath,

                    Id = imageVM.UserId,
                    CreatedOn = DateTime.UtcNow
                    
                };

                await _dbContext.ImageFiles.AddAsync(image);

                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return image;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
