using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Models
{
   public class ImageStore
    {
        public int ImageId { get; set; }
        public string ImageBase64String { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
