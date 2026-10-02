using System.Collections.Generic;
using SystemTools.SharedKernel;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    //რეესტრის ზოგადი შეცდომები (SupportToolsServer-ის CLAUDE.md, Registry conventions). entityName ჩანაწერის ტიპის
    //სახელია (მაგ. Environment). SupportTools-ის სინქრონიზაციის ძრავა ConcurrencyConflict-სა და
    //RecordWithNameNotFound-ს კოდით ცნობს (RegistrySyncServerErrorCodes), ამიტომ ამ მეთოდების სახელები არ იცვლება
    public static Error RecordWithNameNotFound(string entityName, string name)
    {
        return Error.NotFound(nameof(RecordWithNameNotFound), $"{entityName} With Name {name} Not Found");
    }

    public static Error RecordIsInUse(string entityName, string name, IEnumerable<string> usages)
    {
        return Error.Conflict(nameof(RecordIsInUse), $"{entityName} {name} Is Used By: {string.Join(", ", usages)}");
    }

    //upsert-ის ან წაშლის მოსალოდნელი ვერსია სერვერზე შენახულს არ ემთხვევა. 0 ნიშნავს, რომ ჩანაწერი არ უნდა არსებობდეს
    public static Error ConcurrencyConflict(string entityName, string name, int expectedVersion, int actualVersion)
    {
        return Error.Conflict(nameof(ConcurrencyConflict),
            $"{entityName} {name} Version Conflict: Expected {expectedVersion}, Actual {actualVersion}");
    }

    //კონტრაქტი სხვა აგრეგატის არარსებულ ჩანაწერს მიმართავს სახელით (მაგალითად, Server.Runtime)
    public static Error ReferencedRecordsNotFound(string entityName, IEnumerable<string> names)
    {
        return Error.NotFound(nameof(ReferencedRecordsNotFound),
            $"Referenced {entityName} Records Not Found: {string.Join(", ", names)}");
    }

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

    public static Error InvalidGitFolderName(string valueName)
    {
        return Error.Problem(nameof(InvalidGitFolderName), $"{valueName} Is Not A Valid Relative Folder Path");
    }

    public static Error InvalidGitAddress(string valueName)
    {
        return Error.Problem(nameof(InvalidGitAddress),
            $"{valueName} Is Not A Valid Git Address (git@host:path, ssh:// Or https://)");
    }
}