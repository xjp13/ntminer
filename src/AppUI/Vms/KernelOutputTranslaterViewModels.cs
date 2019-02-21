﻿using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModels : ViewModelBase {
        public static readonly KernelOutputTranslaterViewModels Current = new KernelOutputTranslaterViewModels();

        private readonly Dictionary<Guid, List<KernelOutputTranslaterViewModel>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputTranslaterViewModel>>();
        private readonly Dictionary<Guid, KernelOutputTranslaterViewModel> _dicById = new Dictionary<Guid, KernelOutputTranslaterViewModel>();

        private KernelOutputTranslaterViewModels() {
            VirtualRoot.On<KernelOutputTranslaterAddedEvent>(
                "添加了内核输出翻译器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        if (!_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                            _dicByKernelOutputId.Add(message.Source.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                        }
                        var vm = new KernelOutputTranslaterViewModel(message.Source);
                        _dicByKernelOutputId[message.Source.KernelOutputId].Add(vm);
                        _dicById.Add(message.Source.GetId(), vm);
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                });
            VirtualRoot.On<KernelOutputTranslaterUpdatedEvent>(
                "更新了内核输出翻译器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            item.Update(message.Source);
                        }
                    }
                });
            VirtualRoot.On<KernelOutputTranslaterRemovedEvent>(
                "移除了内核输出翻译器后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_dicByKernelOutputId.ContainsKey(message.Source.KernelOutputId)) {
                        var item = _dicByKernelOutputId[message.Source.KernelOutputId].FirstOrDefault(a => a.Id == message.Source.GetId());
                        if (item != null) {
                            _dicByKernelOutputId[message.Source.KernelOutputId].Remove(item);
                        }
                    }
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                    }
                    KernelOutputViewModel kernelOutputVm;
                    if (KernelOutputViewModels.Current.TryGetKernelOutputVm(message.Source.KernelOutputId, out kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                });
            Init();
        }

        private void Init() {
            foreach (var item in NTMinerRoot.Current.KernelOutputTranslaterSet) {
                if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                    _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputTranslaterViewModel>());
                }
                var vm = new KernelOutputTranslaterViewModel(item);
                _dicByKernelOutputId[item.KernelOutputId].Add(vm);
                _dicById.Add(item.GetId(), vm);
            }
        }

        public IEnumerable<KernelOutputTranslaterViewModel> AllKernelOutputTranslaterVms {
            get {
                return _dicById.Values;
            }
        }

        public IEnumerable<KernelOutputTranslaterViewModel> GetListByKernelId(Guid kernelId) {
            if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                return _dicByKernelOutputId[kernelId];
            }
            return new List<KernelOutputTranslaterViewModel>();
        }
    }
}
