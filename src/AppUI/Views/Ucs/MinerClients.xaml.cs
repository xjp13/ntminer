﻿using NTMiner.Bus;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClients : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Miner",
                Width = SystemParameters.FullPrimaryScreenWidth * 0.9,
                Height = SystemParameters.FullPrimaryScreenHeight * 0.8,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new MinerClients(window), fixedSize: false);
        }

        public MinerClientsViewModel Vm {
            get {
                return (MinerClientsViewModel)this.DataContext;
            }
        }

        private readonly ContainerWindow _window;
        private MinerClients(ContainerWindow window) {
            _window = window;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Vm.QueryMinerClients();
            DelegateHandler<Per10SecondEvent> refreshMinerClients = VirtualRoot.On<Per10SecondEvent>(
                "周期刷新在线客户端列表",
                LogEnum.Console,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.LoadClients();
                    });
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                VirtualRoot.UnPath(refreshMinerClients);
            };
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                _window?.DragMove();
            }
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel vm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(null);
            e.Handled = true;
        }
    }
}
