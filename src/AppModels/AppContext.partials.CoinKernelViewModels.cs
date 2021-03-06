﻿using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class CoinKernelViewModels : ViewModelBase {
            public static readonly CoinKernelViewModels Instance = new CoinKernelViewModels();

            private readonly Dictionary<Guid, CoinKernelViewModel> _dicById = new Dictionary<Guid, CoinKernelViewModel>();
            private CoinKernelViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(AllCoinKernels));
                    }, location: this.GetType());
                AddEventPath<CoinKernelAddedEvent>("添加了币种内核后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        var coinKernelVm = new CoinKernelViewModel(message.Target);
                        _dicById.Add(message.Target.GetId(), coinKernelVm);
                        OnPropertyChanged(nameof(AllCoinKernels));
                        if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                            coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                        }
                        var kernelVm = coinKernelVm.Kernel;
                        if (kernelVm != null) {
                            kernelVm.OnPropertyChanged(nameof(kernelVm.CoinKernels));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.CoinVms));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.SupportedCoinVms));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.SupportedCoins));
                        }
                    }, location: this.GetType());
                AddEventPath<CoinKernelUpdatedEvent>("更新了币种内核后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        CoinKernelViewModel entity = _dicById[message.Target.GetId()];
                        var supportedGpu = entity.SupportedGpu;
                        Guid dualCoinGroupId = entity.DualCoinGroupId;
                        entity.Update(message.Target);
                        if (supportedGpu != entity.SupportedGpu) {
                            var coinKernels = AllCoinKernels.Where(a => a.KernelId == entity.Id);
                            foreach (var coinKernel in coinKernels) {
                                if (AppContext.Instance.CoinVms.TryGetCoinVm(coinKernel.CoinId, out CoinViewModel coinVm)) {
                                    coinVm.OnPropertyChanged(nameof(coinVm.IsSupported));
                                    coinVm.OnPropertyChanged(nameof(coinVm.CoinKernels));
                                }
                            }
                            var kernelVm = entity.Kernel;
                            kernelVm.OnPropertyChanged(nameof(kernelVm.CoinKernels));
                        }
                    }, location: this.GetType());
                AddEventPath<CoinKernelRemovedEvent>("移除了币种内核后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out CoinKernelViewModel coinKernelVm)) {
                            _dicById.Remove(message.Target.GetId());
                            OnPropertyChanged(nameof(AllCoinKernels));
                            if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                                coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                                coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                            }
                            var kernelVm = coinKernelVm.Kernel;
                            kernelVm.OnPropertyChanged(nameof(kernelVm.CoinKernels));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.CoinVms));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.SupportedCoinVms));
                            kernelVm.OnPropertyChanged(nameof(kernelVm.SupportedCoins));
                        }
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerRoot.Instance.ServerContext.CoinKernelSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new CoinKernelViewModel(item));
                }
            }

            public bool TryGetCoinKernelVm(Guid id, out CoinKernelViewModel vm) {
                return _dicById.TryGetValue(id, out vm);
            }

            public List<CoinKernelViewModel> AllCoinKernels {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
