﻿using NTMiner.Core;
using NTMiner.Notifications;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {

    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private INotificationMessageManager _manager;
        private Visibility _isBtnRunAsAdministratorVisible = Visibility.Collapsed;
        private bool _isDaemonRunning = true;
        private string _serverJsonVersion;

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private MainWindowViewModel() {
            this.StartMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StartMine(CommandLineArgs.WorkId);
            });
            this.StopMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StopMineAsync();
            });
            if (DevMode.IsDevMode) {
                VirtualRoot.On<Per10SecondEvent>(
                    "在开发者调试区展示守护进程的运行状态",
                    LogEnum.None,
                    action: message => {
                        NTMinerDaemonService.Instance.GetDaemonVersionAsync(thatVersion => {
                            this.IsDaemonRunning = !string.IsNullOrEmpty(thatVersion);
                        });
                    });
                VirtualRoot.On<ServerJsonVersionChangedEvent>(
                    "在开发者调试区展示ServerJsonVersion",
                    LogEnum.Console,
                    action: message => {
                        this.ServerJsonVersion = NTMinerRoot.JsonFileVersion;
                    });
                this._serverJsonVersion = NTMinerRoot.JsonFileVersion;
            }
        }

        public double Height {
            get {
                return AppStatic.MainWindowHeight;
            }
        }

        public double Width {
            get {
                return AppStatic.MainWindowWidth;
            }
        }

        public Visibility IsBtnRunAsAdministratorVisible {
            get => _isBtnRunAsAdministratorVisible;
            set {
                if (_isBtnRunAsAdministratorVisible != value) {
                    _isBtnRunAsAdministratorVisible = value;
                    OnPropertyChanged(nameof(IsBtnRunAsAdministratorVisible));
                }
            }
        }

        public bool IsDaemonRunning {
            get { return _isDaemonRunning; }
            set {
                if (_isDaemonRunning != value) {
                    _isDaemonRunning = value;
                    OnPropertyChanged(nameof(IsDaemonRunning));
                }
            }
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public StateBarViewModel StateBarVm {
            get {
                return StateBarViewModel.Current;
            }
        }

        public Guid WorkId {
            get {
                return CommandLineArgs.WorkId;
            }
        }

        public bool IsReadOnly {
            get {
                if (WorkId != Guid.Empty) {
                    return true;
                }
                return false;
            }
        }

        public INotificationMessageManager Manager {
            get {
                if (_manager == null) {
                    _manager = new NotificationMessageManager();
                }
                return _manager;
            }
        }

        public string ServerJsonVersion {
            get => _serverJsonVersion;
            set {
                if (_serverJsonVersion != value) {
                    _serverJsonVersion = value;
                    OnPropertyChanged(nameof(ServerJsonVersion));
                }
            }
        }

        public string GpuSetInfo {
            get {
                return NTMinerRoot.Current.GpuSetInfo;
            }
        }
    }
}
