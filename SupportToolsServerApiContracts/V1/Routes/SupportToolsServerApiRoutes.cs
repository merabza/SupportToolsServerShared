namespace SupportToolsServerApiContracts.V1.Routes;

public static class SupportToolsServerApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    public const string ApiBase = Root + "/" + Version;

    //რეესტრის არეალები (SupportToolsServer-ის CLAUDE.md, Registry conventions). სერვერი group-ს ApiBase + Base-ზე
    //map-ავს, კლიენტი კი key-ს Uri.EscapeDataString-ით უმატებს Base-ს ან …Prefix-ს
    public static class Environments
    {
        public const string Base = "/environments";

        // GET api/v1/environments
        public const string List = "";

        // GET api/v1/environments/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/environments/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/environments/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class Runtimes
    {
        public const string Base = "/runtimes";

        // GET api/v1/runtimes
        public const string List = "";

        // GET api/v1/runtimes/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/runtimes/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/runtimes/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class NpmPackages
    {
        public const string Base = "/npmpackages";

        // GET api/v1/npmpackages
        public const string List = "";

        // GET api/v1/npmpackages/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/npmpackages/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/npmpackages/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class ReactAppTemplates
    {
        public const string Base = "/reactapptemplates";

        // GET api/v1/reactapptemplates
        public const string List = "";

        // GET api/v1/reactapptemplates/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/reactapptemplates/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/reactapptemplates/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class DotnetTools
    {
        public const string Base = "/dotnettools";

        // GET api/v1/dotnettools
        public const string List = "";

        // GET api/v1/dotnettools/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/dotnettools/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/dotnettools/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class SmartSchemas
    {
        public const string Base = "/smartschemas";

        // GET api/v1/smartschemas
        public const string List = "";

        // GET api/v1/smartschemas/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/smartschemas/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/smartschemas/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class FileStorages
    {
        public const string Base = "/filestorages";

        // GET api/v1/filestorages
        public const string List = "";

        // GET api/v1/filestorages/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/filestorages/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/filestorages/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class ApiClients
    {
        public const string Base = "/apiclients";

        // GET api/v1/apiclients
        public const string List = "";

        // GET api/v1/apiclients/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/apiclients/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/apiclients/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    public static class DatabaseServerConnections
    {
        public const string Base = "/databaseserverconnections";

        // GET api/v1/databaseserverconnections
        public const string List = "";

        // GET api/v1/databaseserverconnections/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/databaseserverconnections/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/databaseserverconnections/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    //გადარქმევის route არ არის: სერვერის გადარქმევა წაშლა და ახლის შექმნაა
    public static class Servers
    {
        public const string Base = "/servers";

        // GET api/v1/servers
        public const string List = "";

        // GET api/v1/servers/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/servers/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/servers/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    //singleton: ერთადერთი ჩანაწერი, key-ის გარეშე. სანამ ის შეიქმნება, GET ცარიელ კონტრაქტს აბრუნებს Version = 0-ით.
    //წაშლის route არ არის
    public static class GlobalSettings
    {
        public const string Base = "/settings/global";

        // GET api/v1/settings/global
        public const string Get = "";

        // POST api/v1/settings/global/update
        public const string Update = "/update";
    }

    //singleton, GlobalSettings-ის მსგავსად
    public static class ProjectCreatorSettings
    {
        public const string Base = "/settings/projectcreator";

        // GET api/v1/settings/projectcreator
        public const string Get = "";

        // POST api/v1/settings/projectcreator/update
        public const string Update = "/update";
    }

    public static class ProjectTemplates
    {
        public const string Base = "/projecttemplates";

        // GET api/v1/projecttemplates
        public const string List = "";

        // GET api/v1/projecttemplates/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/projecttemplates/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/projecttemplates/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    //პროექტები: აგრეგატი შვილებითა და ბაზის პარამეტრებით. GET-ის სიაც სრულ აგრეგატებს აბრუნებს
    public static class Projects
    {
        public const string Base = "/projects";

        // GET api/v1/projects
        public const string List = "";

        // GET api/v1/projects/{key}
        public const string ByKey = "/{key}";

        // POST api/v1/projects/update/{key}
        public const string UpdatePrefix = "/update";
        public const string Update = UpdatePrefix + "/{key}";

        // DELETE api/v1/projects/delete/{key}?version=N
        public const string DeletePrefix = "/delete";
        public const string Delete = DeletePrefix + "/{key}";
    }

    //საიდუმლო ფაილები. გზა route-ის key-ში ვერ ჩაჯდება (\, :), ამიტომ GET-სა და DELETE-ს ის query-ში (path) გადაეცემა,
    //Uri.EscapeDataString-ით, POST-ს კი ტანში. სია მხოლოდ მეტამონაცემებია, შიგთავსი content-ით მოდის
    public static class StoredFiles
    {
        public const string Base = "/files";

        // GET api/v1/files
        public const string List = "";

        // GET api/v1/files/content?path=...
        public const string Content = "/content";

        // POST api/v1/files/update
        public const string Update = "/update";

        // DELETE api/v1/files/delete?path=...&version=N
        public const string Delete = "/delete";
    }

    public static class Git
    {
        public const string GitBase = "/git";

        // POST api/v1/git/uploadgitrepos
        public const string UploadGitRepos = "/uploadgitrepos";

        // GET api/v1/git/gitrepos
        public const string GitRepos = "/gitrepos";

        // GET api/v1/git/gitrepo/{key}
        public const string GitRepoPrefix = "/gitrepo";
        public const string GitRepo = GitRepoPrefix + "/{key}";

        // POST api/v1/git/updategitrepo/{key}
        public const string UpdateGitRepoPrefix = "/updategitrepo";
        public const string UpdateGitRepo = UpdateGitRepoPrefix + "/{key}";

        // DELETE api/v1/git/deletegitrepo/{key}
        public const string DeleteGitRepoPrefix = "/deletegitrepo";
        public const string DeleteGitRepo = DeleteGitRepoPrefix + "/{key}";

        //// GET api/v1/git/gitignorefilenames
        //public const string GitIgnoreFileNames = "/gitignorefilenames";

        // GET api/v1/git/gitignorefiletypeslist
        public const string GitIgnoreFileTypesList = "/gitignorefiletypeslist";

        // POST api/v1/git/updategitignorefiletype/{key}
        public const string UpdateGitIgnoreFileTypePrefix = "/updategitignorefiletype";
        public const string UpdateGitIgnoreFileType = UpdateGitIgnoreFileTypePrefix + "/{key}";

        // POST api/v1/git/syncupgitignorefiletypes/{merge?}
        public const string SyncUpGitIgnoreFileTypesPrefix = "/syncupgitignorefiletypes";
        public const string SyncUpGitIgnoreFileTypes = SyncUpGitIgnoreFileTypesPrefix + "/{merge?}";

        //// POST api/v1/git/mergeupgitignorefiletypes
        //public const string MergeUpGitIgnoreFileTypes = "/mergeupgitignorefiletypes";

        // DELETE api/v1/git/deletegitignorefiletype/{key}
        public const string DeleteGitIgnoreFileTypePrefix = "/deletegitignorefiletype";
        public const string DeleteGitIgnoreFileType = DeleteGitIgnoreFileTypePrefix + "/{key}";

        // GET api/v1/git/editorconfigfiletypeslist
        public const string EditorConfigFileTypesList = "/editorconfigfiletypeslist";

        // POST api/v1/git/syncupeditorconfigfiletypes/{merge?}
        public const string SyncUpEditorConfigFileTypesPrefix = "/syncupeditorconfigfiletypes";
        public const string SyncUpEditorConfigFileTypes = SyncUpEditorConfigFileTypesPrefix + "/{merge?}";

        // DELETE api/v1/git/deleteeditorconfigfiletype/{key}
        public const string DeleteEditorConfigFileTypePrefix = "/deleteeditorconfigfiletype";
        public const string DeleteEditorConfigFileType = DeleteEditorConfigFileTypePrefix + "/{key}";

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