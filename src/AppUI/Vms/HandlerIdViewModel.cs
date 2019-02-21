﻿using System;

namespace NTMiner.Vms {
    public class HandlerIdViewModel : ViewModelBase, IHandlerId {
        private Type _messageType;
        private Type _location;
        private LogEnum _logType;
        private string _description;
        private string _handlerPath;

        public HandlerIdViewModel(IHandlerId data) {
            _messageType = data.MessageType;
            _location = data.Location;
            _handlerPath = data.HandlerPath;
            _logType = data.LogType;
            _description = data.Description;
        }

        public void Update(IHandlerId data) {
            this.MessageType = data.MessageType;
            this.Description = data.Description;
            this.LogType = data.LogType;
            this.Location = data.Location;
        }

        public Type MessageType {
            get { return _messageType; }
            set {
                if (_messageType != value) {
                    _messageType = value;
                    OnPropertyChanged(nameof(MessageType));
                }
            }
        }

        public Type Location {
            get => _location;
            set {
                if (_location != value) {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public LogEnum LogType {
            get => _logType;
            set {
                if (_logType != value) {
                    _logType = value;
                    OnPropertyChanged(nameof(LogType));
                }
            }
        }

        public EnumItem<LogEnum> LogTypeItem {
            get {
                return this.LogType.GetEnumItem();
            }
            set {
                if (this.LogType != value.Value) {
                    this.LogType = value.Value;
                    OnPropertyChanged(nameof(LogTypeItem));
                }
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string HandlerPath {
            get => _handlerPath;
            set {
                _handlerPath = value;
                OnPropertyChanged(nameof(HandlerPath));
            }
        }
    }
}
