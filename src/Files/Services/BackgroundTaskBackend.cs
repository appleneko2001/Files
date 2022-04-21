using System;

namespace Files.Services
{
    // This class is used by the application to execute operations on files, directories and etc.
    // should work on another thread to avoid blocking the UI thread.
    // and keep alive when AvaloniaUI has crashed.
    public class BackgroundTaskBackend
    {
        #region Singleton

        private static BackgroundTaskBackend _instance;
        public static BackgroundTaskBackend Instance => _instance;

        #endregion

        private BackgroundTaskBackend(FilesApp app)
        {
            if(_instance != null)
                throw new InvalidOperationException("BackgroundTaskBackend is a singleton class.");

            app.ApplicationInitializationCompleted += OnApplicationInitializationCompleted;
        }
        
        public static void Initiate(FilesApp app)
        {
            _instance = new BackgroundTaskBackend(app);
        }

        private void OnApplicationInitializationCompleted(object sender, EventArgs e)
        {
            if(sender is not FilesApp app)
                throw new ArgumentException("sender is not a FilesApp instance.");
            
            app.ApplicationInitializationCompleted -= OnApplicationInitializationCompleted;
            app.ApplicationShutdown += OnApplicationShutdown;
        }

        private void OnApplicationShutdown(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}