using Schema.NET;

namespace SharpDao.Cli.Models;

public class Proposal: CreativeWork
{
    public OneOrMany<ProposalChoice> PotentialChoices { get; set; }
    
    //sourceOrganization = DAO this Proposal applies to
    //author = Person(s) who wrote Proposal
    //name/headline = title of Proposal
    //text = main text of the Proposal
    //version = current Version of the Proposal
    //expires = date this Proposal ends its voting
    //potentialAction = the Action to be taken if passed
}