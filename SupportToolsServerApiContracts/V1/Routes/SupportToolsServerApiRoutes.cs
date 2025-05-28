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

        // GET api/git/gitrepos
        public const string GitRepos = "/gitrepos";
       

        // GET api/git/gitrepo/{gitKey}
        public const string GitRepo = "/gitrepo/{gitKey}";

        // POST api/git/updategitrepo/{gitKey}
        public const string UpdateGitRepo = "/updategitrepo/{gitKey}";

        // DELETE api/git/deletegitrepo/{gitKey}
        public const string DeleteGitRepo = "/deletegitrepo/{gitKey}";
        
        //// GET api/git/gitignorefilenames
        //public const string GitIgnoreFileNames = "/gitignorefilenames";

        
        
        // GET api/git/gitignorefiletypeslist
        public const string GitIgnoreFileTypesList = "/gitignorefiletypeslist";

        // POST api/git/addgitignorefiletypenameifnotexists/{gitIgnoreFileTypeName}
        public const string AddGitIgnoreFileTypeNameIfNotExists = "/addgitignorefiletypenameifnotexists/{gitIgnoreFileTypeName}";

        // DELETE api/git/deletegitignorefiletype/{gitIgnoreFileTypeName}
        public const string DeleteGitIgnoreFileType = "/deletegitignorefiletype/{gitIgnoreFileTypeName}";






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