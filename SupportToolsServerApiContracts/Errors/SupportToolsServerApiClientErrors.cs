using SystemTools.SharedKernel;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    public static Error GitWithKeyNotFound(string gitKey)
    {
        return Error.NotFound(nameof(GitWithKeyNotFound), $"Git With Key {gitKey} Not Found");
    }
}