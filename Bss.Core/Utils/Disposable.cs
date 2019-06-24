using System;

namespace Bss.Core.Utils
{

    public class BBooleanDisposable : IDisposable
    {
        private volatile bool _isDisposed;

        public bool IsDisposed => _isDisposed;

        public void Dispose()
        {
            _isDisposed = true;
        }
    }

    public static class Disposable
    {
        public static IDisposable Empty => EmptyDisposable.Instance;

        public static IDisposable Create(Action action) => new ActionDisposable(action);

        private class ActionDisposable : IDisposable
        {
            private readonly Action _disposable;

            public ActionDisposable(Action disposable)
            {
                _disposable = disposable;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                        _disposable?.Invoke();
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }
            #endregion
        }


        private class EmptyDisposable : IDisposable
        {
            private EmptyDisposable() { }

            public static EmptyDisposable Instance { get; } = new EmptyDisposable();

            public void Dispose()
            {

            }
        }

    }
}
