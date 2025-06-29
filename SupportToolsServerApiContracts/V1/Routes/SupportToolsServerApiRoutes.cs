namespace SupportToolsServerApiContracts.V1.Routes;

public static class SupportToolsServerApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    public const string ApiBase = Root + "/" + Version;

    public static class Git
    {
        public const string GitBase = "/git";

        // POST api/v1/git/uploadgitrepos
        public const string UploadGitRepos = "/uploadgitrepos";

        // GET api/v1/git/gitrepos
        public const string GitRepos = "/gitrepos";
       

        // GET api/v1/git/gitrepo/{gitKey}
        public const string GitRepo = "/gitrepo/{gitKey}";

        // POST api/v1/git/updategitrepo/{gitKey}
        public const string UpdateGitRepo = "/updategitrepo/{gitKey}";

        // DELETE api/v1/git/deletegitrepo/{gitKey}
        public const string DeleteGitRepo = "/deletegitrepo/{gitKey}";
        
        //// GET api/v1/git/gitignorefilenames
        //public const string GitIgnoreFileNames = "/gitignorefilenames";

        
        
        // GET api/v1/git/gitignorefiletypeslist
        public const string GitIgnoreFileTypesList = "/gitignorefiletypeslist";

        // POST api/v1/git/addgitignorefiletypenameifnotexists/{gitIgnoreFileTypeName}
        public const string AddGitIgnoreFileTypeNameIfNotExists = "/addgitignorefiletypenameifnotexists/{gitIgnoreFileTypeName}";

        // DELETE api/v1/git/deletegitignorefiletype/{gitIgnoreFileTypeName}
        public const string DeleteGitIgnoreFileType = "/deletegitignorefiletype/{gitIgnoreFileTypeName}";






        //// GET api/v1/databases/getdatabasenames
        //public const string TestGitRepos = "/testgitrepos";

        ////// POST api/v1/databases/createbackup/{databaseName}
        //public const string CreateBackupPrefix = "/createbackup";
        //public const string CreateBackup = CreateBackupPrefix + "/{databaseName}/{dbServerFoldersSetName}";

        ////// POST api/v1/databases/executecommand/{databaseName}
        //public const string ExecuteCommandPrefix = "/executecommand";
        //public const string ExecuteCommand = ExecuteCommandPrefix + "/{databaseName?}";

        ////// GET api/v1/databases/getdatabasenames
        //public const string GetDatabaseNames = "/getdatabasenames";

        ////// GET api/v1/databases/getdatabasefolderssetnames
        //public const string GetDatabaseFoldersSetNames = "/getdatabasefolderssetnames";

        ////// GET api/v1/databases/getdatabaseconnectionnames
        //public const string GetDatabaseConnectionNames = "/getdatabaseconnectionnames";

        ////// GET api/v1/databases/isdatabaseexists/{databaseName}
        //public const string IsDatabaseExistsPrefix = "/isdatabaseexists";
        //public const string IsDatabaseExists = IsDatabaseExistsPrefix + "/{databaseName}";

        ////// PUT api/v1/databases/restorebackup/{databaseName}
        //public const string RestoreBackupPrefix = "/restorebackup";
        //public const string RestoreBackup = RestoreBackupPrefix + "/{databaseName}/{dbServerFoldersSetName}";

        ////// POST api/v1/databases/recompileprocedures/{databaseName}
        //public const string RecompileProceduresPrefix = "/recompileprocedures";
        //public const string RecompileProcedures = RecompileProceduresPrefix + "/{databaseName}";

        ////// POST api/v1/databases/recompileprocedures/{databaseName}
        //public const string ChangeDatabaseRecoveryModelPrefix = "/changedatabaserecoverymodel";

        //public const string ChangeDatabaseRecoveryModel =
        //    ChangeDatabaseRecoveryModelPrefix + "/{databaseName}/{databaseRecoveryModel}";

        ////// GET api/v1/databases/testconnection/{databaseName?}
        //public const string TestConnectionPrefix = "/testconnection";
        //public const string TestConnection = TestConnectionPrefix + "/{databaseName?}";

        ////// POST api/v1/databases/updatestatistics/{databaseName}
        //public const string UpdateStatisticsPrefix = "/updatestatistics";
        //public const string UpdateStatistics = UpdateStatisticsPrefix + "/{databaseName}";
    }
}