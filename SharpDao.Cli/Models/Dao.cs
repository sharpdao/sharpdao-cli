using System.ComponentModel.DataAnnotations;
using Schema.NET;

namespace SharpDao.Cli.Models;

public class Dao: Organization
{
    public OneOrMany<string> Quorum { get; set; } //float (0-1): Minimum participation, or quorum, is the minimum level of participation required for a vote to be valid
    public OneOrMany<string> PassRate { get; set; } //float (0-1): The pass rate is the percentage of votes cast that need to be in favor for the proposal to be accepted.
    public OneOrMany<string> VotingDays { get; set; } //int: The voting period is how long the vote is active.
    
    //name = name of the DAO
    //alternateName = if the DAO goes by another name
    //description = a description of the DAO
    //logo = a logo of the DAO
    //url = the website of the DAO
    //founder = a Person(s) who started the DAO
    //foundingDate = the date when the DAO was created
}