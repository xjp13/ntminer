﻿using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputTranslaterViewModel : ViewModelBase, IKernelOutputTranslater, IEditableViewModel, ISortable {
        private string _regexPattern;
        private Guid _id;
        private string _replacement;
        private int _sortNumber;
        private bool _isPre;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public KernelOutputTranslaterViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelOutputTranslaterViewModel(IKernelOutputTranslater data) : this(data.GetId()) {
            _kernelOutputId = data.KernelOutputId;
            _regexPattern = data.RegexPattern;
            _id = data.GetId();
            _replacement = data.Replacement;
            _sortNumber = data.SortNumber;
            _isPre = data.IsPre;
        }

        public KernelOutputTranslaterViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                int sortNumber = this.SortNumber;
                if (NTMinerRoot.Instance.ServerContext.KernelOutputTranslaterSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelOutputTranslaterCommand(this));
                }
                if (sortNumber != this.SortNumber) {
                    if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new KernelOutputTranslaterEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.RegexPattern}内核输出翻译器吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelOutputTranslaterCommand(this.Id));
                }));
            });
            this.SortUp = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel upOne = AppContext.Instance.KernelOutputTranslaterVms.GetListByKernelId(this.KernelOutputId).GetUpOne(this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    AppContext.Instance.KernelOutputTranslaterVms.OnPropertyChanged(nameof(AppContext.KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                KernelOutputTranslaterViewModel nextOne = AppContext.Instance.KernelOutputTranslaterVms.GetListByKernelId(this.KernelOutputId).GetNextOne(this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateKernelOutputTranslaterCommand(this));
                    AppContext.Instance.KernelOutputTranslaterVms.OnPropertyChanged(nameof(AppContext.KernelOutputTranslaterViewModels.AllKernelOutputTranslaterVms));
                    if (AppContext.Instance.KernelOutputVms.TryGetKernelOutputVm(this.KernelOutputId, out KernelOutputViewModel kernelOutputVm)) {
                        kernelOutputVm.OnPropertyChanged(nameof(kernelOutputVm.KernelOutputTranslaters));
                    }
                }
            });
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private Guid _kernelOutputId;
        public Guid KernelOutputId {
            get => _kernelOutputId;
            set {
                if (_kernelOutputId != value) {
                    _kernelOutputId = value;
                    OnPropertyChanged(nameof(KernelOutputId));
                }
            }
        }

        public string RegexPattern {
            get => _regexPattern;
            set {
                if (_regexPattern != value) {
                    _regexPattern = value;
                    OnPropertyChanged(nameof(RegexPattern));
                }
            }
        }

        public string Replacement {
            get => _replacement;
            set {
                if (_replacement != value) {
                    _replacement = value;
                    OnPropertyChanged(nameof(Replacement));
                }
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }

        public bool IsPre {
            get { return _isPre; }
            set {
                if (_isPre != value) {
                    _isPre = value;
                    OnPropertyChanged(nameof(IsPre));
                    OnPropertyChanged(nameof(IsPreText));
                }
            }
        }

        public string IsPreText {
            get {
                if (IsPre) {
                    return "是";
                }
                return "否";
            }
        }
    }
}
