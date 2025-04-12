﻿namespace SupportToolsServerApiContracts.Models;

public sealed class GitDataDomain
{
    public required string GitProjectName { get; set; }
    public required string GitProjectAddress { get; set; }
    public required string GitProjectFolderName { get; set; }
    public required string GitIgnorePathName { get; set; }
}