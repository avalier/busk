using System;

namespace Avalier.Busk
{
    public class Disposable : IDisposable
    {
        private Action _terminate = () => { };
        
        public Disposable(Action terminate = null)
        {
            _terminate = terminate ?? (() => {});
        }

        public void Dispose()
        {
            _terminate?.Invoke();
        }
    }
}