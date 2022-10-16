using Schema.NET;

namespace SharpDao.Cli.Models;

public class Treasury: MonetaryAmount
{
    public OneOrMany<Dao> Dao { get; set; } //DAO DID Ref
    public OneOrMany<Person> Manager { get; set; } //Person(s) who can authorize transfer of funds
    public OneOrMany<string> TokenName { get; set; } //name of the Token
    public OneOrMany<string> TokenSymbol { get; set; } //symbol of the Token

    //value = how many Tokens are in the Treasury
    //maxValue = max supply, optional
    //validThrough = last datetime we can mint more Tokens, optional
    //validFrom = the created date of the DAO
}