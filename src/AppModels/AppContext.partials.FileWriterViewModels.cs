﻿using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class FileWriterViewModels : ViewModelBase {
            public static readonly FileWriterViewModels Instance = new FileWriterViewModels();
            private readonly Dictionary<Guid, FileWriterViewModel> _dicById = new Dictionary<Guid, FileWriterViewModel>();
            public ICommand Add { get; private set; }
            private FileWriterViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                this.Add = new DelegateCommand(() => {
                    new FileWriterViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChangeds();
                    }, location: this.GetType());
                AddEventPath<FileWriterAddedEvent>("添加了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            FileWriterViewModel groupVm = new FileWriterViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), groupVm);
                            OnPropertyChangeds();
                        }
                    }, location: this.GetType());
                AddEventPath<FileWriterUpdatedEvent>("更新了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            FileWriterViewModel entity = _dicById[message.Target.GetId()];
                            entity.Update(message.Target);
                        }
                    }, location: this.GetType());
                AddEventPath<FileWriterRemovedEvent>("删除了文件书写器后调整VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChangeds();
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
                foreach (var item in NTMinerRoot.Instance.ServerContext.FileWriterSet.AsEnumerable()) {
                    FileWriterViewModel groupVm = new FileWriterViewModel(item);
                    _dicById.Add(item.GetId(), groupVm);
                }
            }

            private void OnPropertyChangeds() {
                OnPropertyChanged(nameof(List));
            }

            public bool TryGetFileWriterVm(Guid groupId, out FileWriterViewModel groupVm) {
                return _dicById.TryGetValue(groupId, out groupVm);
            }

            public List<FileWriterViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
