using System;
using System.Collections.Generic;
using System.Text;

namespace OF.BasicMapper.Tests.Utility
{
    public class SampleAdmin
    {
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
