namespace SupportToolsServerApiContracts.Models;

public sealed class GitDataDomain
{
    //// ReSharper disable once ConvertToPrimaryConstructor
    //public GitDataDomain(string gitProjectName, string gitProjectAddress, string gitProjectFolderName, string gitIgnorePathName)
    //{
    //    GitProjectName = gitProjectName;
    //    GitProjectAddress = gitProjectAddress;
    //    GitProjectFolderName = gitProjectFolderName;
    //    GitIgnorePathName = gitIgnorePathName;
    //}

    //public GitDataDomain(string gitProjectAddress, string gitProjectFolderName, string gitIgnorePathName)
    //{
    //    GitProjectName = gitProjectFolderName;
    //    GitProjectAddress = gitProjectAddress;
    //    GitProjectFolderName = gitProjectFolderName;
    //    GitIgnorePathName = gitIgnorePathName;
    //}

    public string GitProjectName { get; }
    public string GitProjectAddress { get; set; }
    public string GitProjectFolderName { get; set; }
    public string GitIgnorePathName { get; }
}