using System.Collections.Generic;
using SupportToolsServerApiContracts.Models;

namespace SupportToolsServerApiContracts.V1.Requests;

public sealed class SyncGitRequest
{
    public required List<StsGitDataModel> Gits { get; set; }
    public required List<StsGitIgnoreFileTypeDataModel> GitIgnoreFiles { get; set; }
}