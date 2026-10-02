using Newtonsoft.Json;
using SupportToolsServerApiContracts.Models;
using Xunit;

namespace SupportToolsServerApiContracts.Tests.Models;

//An older client sends no Version, which reads as 0; the existing POST routes of the server ignore it
public sealed class StsDataModelVersionTests
{
    [Fact]
    public void StsGitDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsGitDataModel>(
            """{"GitProjectName":"RepoA","GitProjectAddress":"git@github.com:x/a.git","GitProjectFolderName":"A","GitIgnorePatternName":"CSharp"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsGitIgnoreFileTypeDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsGitIgnoreFileTypeDataModel>(
            """{"Id":"11111111-1111-1111-1111-111111111111","Name":"CSharp","Content":"bin/"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEditorConfigFileTypeDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsEditorConfigFileTypeDataModel>(
            """{"Name":"default","Content":"root = true"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEnvironmentDataModel_ReadsAMissingDescriptionAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsEnvironmentDataModel>("""{"Name":"Dev"}""")!;

        Assert.Equal("Dev", model.Name);
        Assert.Null(model.Description);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEnvironmentDataModel_RoundTripsTheVersion()
    {
        var model = new StsEnvironmentDataModel { Name = "Prod", Description = "Production", Version = 7 };

        var read = JsonConvert.DeserializeObject<StsEnvironmentDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Prod", read.Name);
        Assert.Equal("Production", read.Description);
        Assert.Equal(7, read.Version);
    }
}
