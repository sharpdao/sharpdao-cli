namespace SharpDao.Cli.Constants;

public static class CommonConstants
{
    public static string BaseStorage => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpDao"); 
    public static string DaoStorage => Path.Combine(BaseStorage, "Daos"); 
    public static string IdentityStorage => Path.Combine(BaseStorage, "Identities"); 
    public static string ProposalStorage => Path.Combine(BaseStorage, "Proposals"); 
}