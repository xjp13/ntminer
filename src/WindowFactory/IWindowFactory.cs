﻿using NTMiner.MinerServer;
using System.Windows;

namespace NTMiner {
    public interface IWindowFactory {
        Window CreateSplashWindow();
        void ShowMainWindow(bool isToggle);
        void ShowMainWindow(Application app, NTMinerAppType appType);
        void Link();
    }
}
