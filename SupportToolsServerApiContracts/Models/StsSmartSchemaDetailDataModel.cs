namespace SupportToolsServerApiContracts.Models;

//ჭკვიანი სქემის დეტალი: პერიოდის ტიპი (კლიენტის EPeriodType-ის სახელი, მაგალითად Day) და ამ ტიპის რამდენი პერიოდის
//ფაილი შეინახოს
public sealed class StsSmartSchemaDetailDataModel
{
    public required string PeriodType { get; set; }
    public int PreserveCount { get; set; }
}
