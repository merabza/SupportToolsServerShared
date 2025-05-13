namespace SupportToolsServerApiContracts.V1.Routes;

public static class SupportToolsServerApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    public const string ApiBase = Root + "/" + Version;

    public static class Git
    {
        public const string GitBase = "/git";

        // POST api/git/uploadgitrepos
        public const string UploadGitRepos = "/uploadgitrepos";

        // GET api/git/getgitrepos
        public const string GetGitRepos = "/getgitrepos";

        // GET api/git/getgitrepo
        public const string GetGitRepo = "/getgitrepo";

        // GET api/git/updategitrepo
        public const string UpdateGitRepo = "/updategitrepo";

        // GET api/git/deletegitrepo
        public const string DeleteGitRepo = "/deletegitrepo";

        
        

        



        //// GET api/databases/getdatabasenames
        //public const string TestGitRepos = "/testgitrepos";

        ////// POST api/databases/createbackup/{databaseName}
        //public const string CreateBackupPrefix = "/createbackup";
        //public const string CreateBackup = CreateBackupPrefix + "/{databaseName}/{dbServerFoldersSetName}";

        ////// POST api/databases/executecommand/{databaseName}
        //public const string ExecuteCommandPrefix = "/executecommand";
        //public const string ExecuteCommand = ExecuteCommandPrefix + "/{databaseName?}";

        ////// GET api/databases/getdatabasenames
        //public const string GetDatabaseNames = "/getdatabasenames";

        ////// GET api/databases/getdatabasefolderssetnames
        //public const string GetDatabaseFoldersSetNames = "/getdatabasefolderssetnames";

        ////// GET api/databases/getdatabaseconnectionnames
        //public const string GetDatabaseConnectionNames = "/getdatabaseconnectionnames";

        ////// GET api/databases/isdatabaseexists/{databaseName}
        //public const string IsDatabaseExistsPrefix = "/isdatabaseexists";
        //public const string IsDatabaseExists = IsDatabaseExistsPrefix + "/{databaseName}";

        ////// PUT api/databases/restorebackup/{databaseName}
        //public const string RestoreBackupPrefix = "/restorebackup";
        //public const string RestoreBackup = RestoreBackupPrefix + "/{databaseName}/{dbServerFoldersSetName}";

        ////// POST api/databases/recompileprocedures/{databaseName}
        //public const string RecompileProceduresPrefix = "/recompileprocedures";
        //public const string RecompileProcedures = RecompileProceduresPrefix + "/{databaseName}";

        ////// POST api/databases/recompileprocedures/{databaseName}
        //public const string ChangeDatabaseRecoveryModelPrefix = "/changedatabaserecoverymodel";

        //public const string ChangeDatabaseRecoveryModel =
        //    ChangeDatabaseRecoveryModelPrefix + "/{databaseName}/{databaseRecoveryModel}";

        ////// GET api/databases/testconnection/{databaseName?}
        //public const string TestConnectionPrefix = "/testconnection";
        //public const string TestConnection = TestConnectionPrefix + "/{databaseName?}";

        ////// POST api/databases/updatestatistics/{databaseName}
        //public const string UpdateStatisticsPrefix = "/updatestatistics";
        //public const string UpdateStatistics = UpdateStatisticsPrefix + "/{databaseName}";
    }
}