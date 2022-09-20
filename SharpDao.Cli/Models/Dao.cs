namespace SharpDao.Cli.Models;

public class Dao
{
    #region Details
    public Guid Id { get; set; }
    public int DaoType { get; set; }
    public string Name { get; set; }
    #endregion
    
    #region Governance
    public decimal Support { get; set; }
    public decimal Approval { get; set; }
    public int VotingDurationDays { get; set; }
    #endregion
    
    #region Token
    public string TokenName { get; set; }
    public string TokenSymbol { get; set; }
    #endregion
}