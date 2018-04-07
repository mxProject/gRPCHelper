using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxProject.Helpers
{

    /// <summary>
    /// 
    /// </summary>
    internal sealed class AsyncLock
    {

        private readonly System.Threading.SemaphoreSlim m_semaphore = new System.Threading.SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> m_releaser;

        /// <summary>
        /// 
        /// </summary>
        public AsyncLock() : this(true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncLock(bool enabled)
        {
            m_Enabled = enabled;
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = false; }
        }
        private bool m_Enabled;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IDisposable> LockAsync()
        {
            if (this.Enabled)
            {
                var wait = m_semaphore.WaitAsync();
                return wait.IsCompleted ?
                        m_releaser :
                        wait.ContinueWith(
                          (_, state) => (IDisposable)state,
                          m_releaser.Result,
                          System.Threading.CancellationToken.None,
                          TaskContinuationOptions.ExecuteSynchronously,
                          TaskScheduler.Default
                        );
            }
            else
            {
                return Task.FromResult<IDisposable>(new NonLock());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }

        private sealed class NonLock : IDisposable
        {
            public void Dispose() { }
        }

    }

}
