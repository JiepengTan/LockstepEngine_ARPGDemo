using System.Threading;

namespace Lockstep.FakeServer{
    public class ProxyServer : UnityEngine.MonoBehaviour {
        public Thread thread;

        public void DoStart(){
            thread = new Thread(ServerLauncher.__Main);
            thread.Start();
        }

        private void OnDestroy(){
            if (thread != null) {
                thread.Abort();
                thread = null;
            }
        }
    }
}