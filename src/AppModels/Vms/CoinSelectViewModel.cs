﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _selectedResult;
        public readonly Action<CoinViewModel> OnOk;
        private readonly IEnumerable<CoinViewModel> _coins;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public CoinSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public CoinSelectViewModel(
            IEnumerable<CoinViewModel> coins,
            CoinViewModel selected,
            Action<CoinViewModel> onOk,
            bool isPromoteHotCoin = false) {
            _coins = coins;
            _selectedResult = selected;
            OnOk = onOk;
            IsPromoteHotCoin = isPromoteHotCoin;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public bool IsPromoteHotCoin {
            get; set;
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
        }

        public CoinViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<CoinViewModel> QueryResults {
            get {
                if (!string.IsNullOrEmpty(Keyword)) {
                    return _coins.Where(a =>
                        (a.Code != null && a.Code.IgnoreCaseContains(Keyword)) ||
                        (a.CnName != null && a.CnName.IgnoreCaseContains(Keyword)) ||
                        (a.EnName != null && a.EnName.IgnoreCaseContains(Keyword))).OrderBy(a => a.Code).ToList();
                }
                return _coins.OrderBy(a => a.Code).ToList();
            }
        }

        public List<CoinViewModel> HotCoins {
            get {
                if (!IsPromoteHotCoin) {
                    return new List<CoinViewModel>();
                }
                return _coins.Where(a => a.IsHot).OrderBy(a => a.Code).ToList();
            }
        }
    }
}
