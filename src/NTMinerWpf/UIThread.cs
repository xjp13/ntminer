﻿using System;
using System.Windows.Threading;

namespace NTMiner {
    public static class UIThread {
        private static Action<Action> _executor = action => action();

        public static void InitializeWithDispatcher() {
            var dispatcher = Dispatcher.CurrentDispatcher;
            Write.UIThreadId = dispatcher.Thread.ManagedThreadId;
            _executor = action => {
                if (action == null) {
                    return;
                }
                if (dispatcher.CheckAccess()) {
                    action();
                }
                else {
                    dispatcher.BeginInvoke(action);
                }
            };
        }

        /// <summary>
        /// 在UI线程上执行给定的行为。注意action不应是Vm上的方法，如果是Vm上的方法必须包裹一次。
        /// </summary>
        public static void Execute(this Action action) {
            _executor(action);
        }
    }
}