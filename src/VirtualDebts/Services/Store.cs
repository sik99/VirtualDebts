using VirtualDebts.Models;

namespace VirtualDebts.Services
{
    public delegate void StateChangedHandler();
    public delegate bool StateUpdater(AppState state);

    public class Store
    {
        private readonly object stateLock = new object();
        private AppState state = new AppState();

        public AppState GetState()
        {
            lock (this.stateLock)
            {
                return this.state.Copy();
            }
        }

        public bool Update(StateUpdater transform)
        {
            lock (this.stateLock)
            {
                bool isSuccess = transform(this.state);
                this.StateChanged?.Invoke();
                return isSuccess;
            }
        }

        public event StateChangedHandler StateChanged;
    }
}
