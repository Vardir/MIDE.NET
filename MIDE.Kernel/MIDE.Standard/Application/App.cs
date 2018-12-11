using System;

namespace MIDE.Standard.Application
{
    public class App : IDisposable
    {
        private static App instance;
        public static App Instance => instance ?? (instance = new App());

        private bool isRunning;

        private App (){}

        public void Start()
        {
            isRunning = true;
            //TODO: prepare application
            LoadExtensions();
        }
        public void LoadExtensions()
        {
            //TODO: load extensions DLL
        }
        public void Exit()
        {
            if (!isRunning)
                throw new InvalidOperationException("Can not exit application that is not started");
            isRunning = false;
            UnloadSolution();
            UnloadExtensions();
            ClearTemporaryFiles();
            Dispose();
        }

        private void UnloadSolution()
        {

        }
        private void UnloadExtensions()
        {

        }
        private void ClearTemporaryFiles()
        {

        }

        public void Dispose()
        {
            if (isRunning)
                throw new InvalidOperationException("Can not dispose application resources while it's running");
            //TODO: dispose all the application resources
        }
    }
}