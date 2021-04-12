using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Utility
{
    public class ResponseManager
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime? ExpireDate { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
