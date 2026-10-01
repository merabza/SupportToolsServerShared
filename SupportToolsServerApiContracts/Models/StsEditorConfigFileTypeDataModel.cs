namespace SupportToolsServerApiContracts.Models;

//.editorconfig შაბლონი. სერვერი ჩანაწერებს სახელით ადარებს, ამიტომ Id კლიენტს არ სჭირდება
public sealed class StsEditorConfigFileTypeDataModel
{
    public required string Name { get; set; }
    public required string Content { get; set; }
}