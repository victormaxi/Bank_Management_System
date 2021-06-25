using _Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _Core.Interfaces
{
   public interface IImageManager
    {
        Task<object> UploadImage(ImageVM imageVM);
    }
}
