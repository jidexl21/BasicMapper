using FluentAssertions;
using OF.BasicMapper.Tests.Utility;
using System;
using System.Collections.Generic;
using Xunit;

namespace OF.BasicMapper.Tests
{
    public class UnitTest1
    {
        private readonly IBasicMapper mapper;
        public UnitTest1()
        {
            mapper = new BasicMapper();
        }
        [Fact]
        public void CanPerformBasicMapping()
        {
            var user = new SamplePerson { 
                UserName = "John Doe", 
                //DateOfBirth = "2019-01-01"
            };
            var admin = mapper.MapFields<SampleAdmin>(user);
            admin.Should().NotBeNull();
            admin.UserName.Should().Be(user.UserName);
            admin.Email.Should().Be(user.Email);
           
        }
        [Fact]
        public void CanMapFromObject()
        {
            var c = new {
                UserName = "Jane Eyre"
            };
            var user = mapper.MapFields<SampleAdmin>(c);
            user.UserName.Should().Be("Jane Eyre");
        }

        [Fact]
        public void CanMapToObject()
        {
            var user = new SamplePerson
            {
                UserName = "John Doe",
            };
            object to = new { UserName = "" };
            var result = mapper.MapFields(user, to);
            //result
            var res = result.GetType().GetProperty("UserName").GetValue(result);
            //.Should().Be(user.UserName);
        }
    }
}
