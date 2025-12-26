using System;

namespace SupportToolsServerApiContracts.Models;

public sealed class StsGitIgnoreFileTypeDataModel
{
    public Guid RowId { get; set; }
    public required string Name { get; set; }
    public required string Content { get; set; }
}