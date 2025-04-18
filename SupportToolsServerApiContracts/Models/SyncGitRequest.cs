﻿using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

public sealed class SyncGitRequest
{
    public required Dictionary<string, GitDataDomain> Gits { get; set; }
    public required List<GitIgnoreFile> GitIgnoreFiles { get; set; }

}