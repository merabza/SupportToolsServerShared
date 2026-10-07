using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

//git რეპოზიტორიის ერთი პროექტის ფაილი (csproj ან esproj) SupportTools-ის GitProjectDataModel-ის ველებით
//(GET git/gitprojects, B9). სერვერი მას თავისი კლონის სკანირებით ითვლის, როგორც კლიენტის Update Git Projects.
//ProjectRelativePath Gits ფოლდერის მიმართ შეფარდებითი ფოლდერია \-ით და რეპოზიტორიის ფოლდერის სახელით იწყება.
//DependsOnProjectNames ProjectReference-ების ფაილების სახელებია გაფართოების გარეშე, ანბანით. პროექტის სახელი (ფაილის
//სახელი გაფართოების გარეშე) ორ რეპოზიტორიაში შეიძლება განმეორდეს: სერვერი ორივეს აბრუნებს, ერთს კი კლიენტი ირჩევს
public sealed class StsGitProjectDataModel
{
    public required string GitName { get; set; }
    public required string ProjectRelativePath { get; set; }
    public required string ProjectFileName { get; set; }
    public List<string> DependsOnProjectNames { get; set; } = [];
}
