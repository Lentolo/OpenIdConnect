using FluentAssertions;
using TCPOS.Authentication.Utils.Extensions;
using Xunit;

namespace TCPOS.Authentication.Tests
{
    public class Class1
    {
        [Fact]
        public void AA()
        {
            var aaa = new Uri("http://fakehost.com/fakepath1/fakepath2/", UriKind.Absolute);
            
            aaa.MakeAbsolute(new Uri("test", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/fakepath1/fakepath2/test");
            aaa.MakeAbsolute(new Uri("/test", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/test");
            aaa.MakeAbsolute(new Uri("../test", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/fakepath1/test");
            aaa.MakeAbsolute(new Uri("test/", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/fakepath1/fakepath2/test/");
            aaa.MakeAbsolute(new Uri("/test/", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/test/");
            aaa.MakeAbsolute(new Uri("../test/", UriKind.Relative)).AbsoluteUri.Should().Be("http://fakehost.com/fakepath1/test/");
        }
    }
}
