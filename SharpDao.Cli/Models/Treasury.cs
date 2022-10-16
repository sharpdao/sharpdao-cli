using Schema.NET;

namespace SharpDao.Cli.Models;

public class Treasury: MonetaryAmount
{
    public OneOrMany<Dao> Dao { get; set; } //DAO DID Ref
    
    //currency = symbol of the Token
    //name = the name of the Token
    //value = how many to mint now
    //maxValue = max supply
    //validThrough = only if we know were not minting more
    //validFrom = the created date of the DAO
}