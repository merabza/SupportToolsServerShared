using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

//ჭკვიანი სქემა, SupportToolsParameters.SmartSchemas-ის ჩანაწერი: სახელი (dictionary-ის key), ბოლო ფაილების რაოდენობა,
//რომელიც ყოველთვის რჩება, და დეტალები. დეტალების რიგს მნიშვნელობა არ აქვს: პერიოდის ტიპი სქემაში ერთხელ გვხვდება და
//სერვერი დეტალებს ტიპით დალაგებულს აბრუნებს (OrdinalIgnoreCase). ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე.
//upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული
//ვერსია N-ია. განახლება მთელ ჩანაწერს ანაცვლებს, დეტალების ჩათვლით
public sealed class StsSmartSchemaDataModel
{
    public required string Name { get; set; }
    public int LastPreserveCount { get; set; }
    public List<StsSmartSchemaDetailDataModel> Details { get; set; } = [];
    public int Version { get; set; }
}
