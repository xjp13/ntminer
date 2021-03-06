﻿using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class KernelInputViewModels : ViewModelBase {
            public static readonly KernelInputViewModels Instance = new KernelInputViewModels();
            private readonly Dictionary<Guid, KernelInputViewModel> _dicById = new Dictionary<Guid, KernelInputViewModel>();

            private KernelInputViewModels() {
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
                        OnPropertyChangeds();
                    }, location: this.GetType());
                AddEventPath<KernelInputAddedEvent>("添加了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        var vm = new KernelInputViewModel(message.Target);
                        _dicById.Add(message.Target.GetId(), vm);
                        OnPropertyChangeds();
                    }, location: this.GetType());
                AddEventPath<KernelInputUpdatedEvent>("更新了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            var item = _dicById[message.Target.GetId()];
                            if (item != null) {
                                bool isSupportDualMine = item.IsSupportDualMine;
                                string args = item.Args;
                                item.Update(message.Target);
                                if (args != item.Args) {
                                    CoinViewModel coinVm = AppContext.Instance.MinerProfileVm.CoinVm;
                                    if (coinVm != null && coinVm.CoinKernel != null && coinVm.CoinKernel.Kernel.KernelInputId == item.Id) {
                                        NTMinerRoot.RefreshArgsAssembly.Invoke();
                                    }
                                }
                                if (isSupportDualMine != item.IsSupportDualMine) {
                                    foreach (var coinKernelVm in AppContext.Instance.CoinKernelVms.AllCoinKernels.Where(a => a.KernelId == message.Target.GetId())) {
                                        coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                                    }
                                }
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<KernelInputRemovedEvent>("移除了内核输入后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            _dicById.Remove(message.Target.GetId());
                            OnPropertyChangeds();
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
                foreach (var item in NTMinerRoot.Instance.ServerContext.KernelInputSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new KernelInputViewModel(item));
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(AllKernelInputVms));
                OnPropertyChanged(nameof(PleaseSelectVms));
            }

            public bool TryGetKernelInputVm(Guid id, out KernelInputViewModel kernelInputVm) {
                return _dicById.TryGetValue(id, out kernelInputVm);
            }

            public List<KernelInputViewModel> AllKernelInputVms {
                get {
                    return _dicById.Values.OrderBy(a => a.Name).ToList();
                }
            }

            private IEnumerable<KernelInputViewModel> GetPleaseSelectVms() {
                yield return KernelInputViewModel.PleaseSelect;
                foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                    yield return item;
                }
            }

            public List<KernelInputViewModel> PleaseSelectVms {
                get {
                    return GetPleaseSelectVms().ToList();
                }
            }
        }
    }
}
