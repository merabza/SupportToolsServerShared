namespace SupportToolsServerApiContracts.Models;

//git რეპოზიტორიის remote-ის მიმდინარე ვერსია (GET git/gitremoteheads): default branch-ის სახელი და მისი commit-ის
//იდენტიფიკატორი, როგორც სერვერმა git ls-remote-ით ბოლოს გაიგო (ქეში ვადით). თუ შემოწმება ვერ მოხერხდა, BranchName და
//CommitId ცარიელია და ErrorDescription შეცდომას აღწერს: ასეთ რეპოზიტორიას კლიენტი თვითონ ამოწმებს (fetch)
public sealed class StsGitRemoteHeadDataModel
{
    public required string GitName { get; set; }
    public string? BranchName { get; set; }
    public string? CommitId { get; set; }
    public string? ErrorDescription { get; set; }
}