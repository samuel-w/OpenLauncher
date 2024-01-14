using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IntelOrca.OpenLauncher.Core;
using Xunit;

namespace IntelOrca.OpenLauncher.Tests
{
    public class BuildServiceTests
    {
        [Theory]
        [InlineData("macos", "v22.05.1", 4157592, "2022-05-17T20:06:15Z")]
        public async Task GetBuildsAsync_OpenLoco_v22_05_1(string system, string version, int size, string publishtime)
        {
            var buildService = new BuildService();
            var builds = await buildService.GetBuildsAsync(Game.OpenLoco, includeDevelop: false);
            var build = builds.First(x => x.Version == version && x.Assets.Any(t => IsMatchingSystemAsset(system, t)));
            var buildAsset  = build.Assets.First(ba => ba.Platform == OSPlatform.OSX);

            Assert.Equal(version, build.Version);
            Assert.Equal(DateTime.Parse(publishtime).ToUniversalTime(), build.PublishedAt);
            Assert.Equal($"OpenLoco-{version}-{system}.zip", buildAsset.Name);
            Assert.Equal(new Uri($"https://github.com/OpenLoco/OpenLoco/releases/download/{version}/OpenLoco-{version}-{system}.zip"), buildAsset.Uri);
            Assert.Equal("application/x-zip-compressed", buildAsset.ContentType);
            Assert.Equal(size, buildAsset.Size);
        }

        private static bool IsMatchingSystemAsset(string system, BuildAsset t) => t.Uri.AbsoluteUri.Contains($"{system}.zip");
    }
}
