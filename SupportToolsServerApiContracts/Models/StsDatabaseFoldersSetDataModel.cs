namespace SupportToolsServerApiContracts.Models;

//ბაზის სერვერის ფოლდერების ნაკრები: სახელი (კლიენტის dictionary-ის key, კავშირის შიგნით უნიკალური) და ბექაპის,
//მონაცემებისა და ლოგის ფოლდერები. გზები DB სერვერისაა და არ გარდაიქმნება
public sealed class StsDatabaseFoldersSetDataModel
{
    public required string Name { get; set; }
    public string? Backup { get; set; }
    public string? Data { get; set; }
    public string? DataLog { get; set; }
}
