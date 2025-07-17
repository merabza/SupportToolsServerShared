using System.Collections.Generic;
using SupportToolsServerApiContracts.Models;

namespace SupportToolsServerApiContracts.V1.Requests;

public sealed class SyncGitRequest
{
    public required Dictionary<string, GitDataDto> Gits { get; set; }
    public required List<GitIgnoreFileDto> GitIgnoreFiles { get; set; }
}