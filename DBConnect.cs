namespace MailApp
{
    class DBConnect
    {

        public string host { get; set; }
        public string db { get; set; }
        public string user { get; set; }
        public string password { get; set; }

        public DBConnect() 
        {
            string strPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            INIManager manager = new INIManager(strPath + @"\settings.ini");
            host = manager.GetPrivateString("DatabaseConnect", "host");
            db = manager.GetPrivateString("DatabaseConnect", "database");
            user = manager.GetPrivateString("DatabaseConnect", "user");
            password = manager.GetPrivateString("DatabaseConnect", "password");
        }

    }
}
