using Schema.NET;

namespace SharpDao.Cli.Models;

public class ProposalVote: ChooseAction
{
    //agent = the Person voting
    //subjectOf = the Proposal
    //object = the selected ProposalChoice(s)
    //actionOption = the list of ProposalChoice items from which we selected our Object
}