namespace SharpDao.Cli.Constants;

public static class CommonConstants
{
    public static string BaseStorage => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpDao"); 
    public static string DaoStorage => Path.Combine(BaseStorage, "daos"); 
    public static string IdentityStorage => Path.Combine(BaseStorage, "identities"); 
    public static string ProposalStorage => Path.Combine(DaoStorage, "proposals"); 
}