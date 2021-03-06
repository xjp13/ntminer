﻿using System;
using WebSocketSharp;

namespace NTMiner {
    class Program {
        static void Main() {
            using (var ws = new WebSocket("ws://localhost:8088/Echo")) {
                ws.OnOpen += (sender, e) => {
                    Write.UserWarn($"WebSocket Open");
                    ws.Send("Hi!");
                };
                ws.OnMessage += (sender, e) => {
                    Write.UserInfo(e.Data);
                };
                ws.OnError += (sender, e) => {
                    Write.UserError(e.Message);
                };
                ws.OnClose += (sender, e) => {
                    Write.UserWarn($"WebSocket Close {e.Code} {e.Reason}");
                };
                ws.Log.Level = LogLevel.Trace;
                ws.Connect();
                Windows.ConsoleHandler.Register(ws.Close);
                Console.WriteLine("\nType 'exit' to exit.\n");
                while (true) {
                    Console.Write("> ");
                    var msg = Console.ReadLine();
                    if (msg == "exit")
                        break;

                    if (!ws.IsAlive) {
                        ws.Connect();
                    }
                    // Send a text message.
                    ws.Send(msg);
                }
            }
        }
    }
}
