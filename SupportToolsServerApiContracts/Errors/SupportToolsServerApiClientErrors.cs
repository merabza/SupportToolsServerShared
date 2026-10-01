using SystemTools.SharedKernel;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    public static Error GitWithKeyNotFound(string gitKey)
    {
        return Error.NotFound(nameof(GitWithKeyNotFound), $"Git With Key {gitKey} Not Found");
    }

    public static Error GitIgnoreFileTypeWithNameNotFound(string gitIgnoreFileTypeName)
    {
        return Error.NotFound(nameof(GitIgnoreFileTypeWithNameNotFound),
            $"GitIgnore File Type With Name {gitIgnoreFileTypeName} Not Found");
    }

    public static Error GitIgnoreFileTypeIsInUse(string usages)
    {
        return Error.Conflict(nameof(GitIgnoreFileTypeIsInUse), $"GitIgnore File Type Is Used By Gits: {usages}");
    }

    public static Error GitAddressIsInUse(string gitAddress, string gitNames)
    {
        return Error.Conflict(nameof(GitAddressIsInUse), $"Git Address {gitAddress} Is Used By {gitNames}");
    }

    public static Error EditorConfigFileTypeWithNameNotFound(string editorConfigFileTypeName)
    {
        return Error.NotFound(nameof(EditorConfigFileTypeWithNameNotFound),
            $"EditorConfig File Type With Name {editorConfigFileTypeName} Not Found");
    }

    public static Error ValueRequired(string valueName)
    {
        return Error.Problem(nameof(ValueRequired), $"{valueName} Is Required");
    }

    public static Error ValueTooLong(string valueName, int maxLength)
    {
        return Error.Problem(nameof(ValueTooLong), $"{valueName} Is Longer Than {maxLength} Characters");
    }

    public static Error ValuesNotUnique(string valueName)
    {
        return Error.Problem(nameof(ValuesNotUnique), $"{valueName} Values Are Not Unique");
    }
}