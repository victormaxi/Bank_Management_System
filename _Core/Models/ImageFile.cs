using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace _Core.Models
{
   public class ImageFile
    {
        
        public Guid ImageId { get; set; } = Guid.NewGuid();
        public string ImagePath { get; set; }
        public DateTime? CreatedOn { get; set; }
        
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
