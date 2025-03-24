using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

public class SyncGitRequest
{
    public Dictionary<string, GitDataDomain> Gits { get; set; }
    public List<GitIgnoreFile> GitIgnoreFiles { get; set; }

}