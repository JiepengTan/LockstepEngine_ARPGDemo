using System;
using System.Threading;
using Lockstep.Network;
using UnityEngine;

namespace Lockstep.FakeServer{
    public class ServerLauncher {
        private static Server server;

        public static void __Main(){
            // 异步方法全部会回掉到主线程
            OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(contex);
            try {
                DoAwake();
                while (true) {
                    try {
                        Thread.Sleep(3);
                        contex.Update();
                        server.Update();
                    }
                    catch (ThreadAbortException e) {
                        return;
                    }
                    catch (Exception e) {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (ThreadAbortException e) {
                return;
            }
            catch (Exception e) {
                Log.Error(e.ToString());
            }
        }

        static void DoAwake(){
            server = new Server();
            server.Start();
        }
    }
}