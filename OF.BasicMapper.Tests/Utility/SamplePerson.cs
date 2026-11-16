using System;
using System.Collections.Generic;
using System.Text;

namespace OF.BasicMapper.Tests.Utility
{
    public class SamplePerson
    {
        public string UserName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
