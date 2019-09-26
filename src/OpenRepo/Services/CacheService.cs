using System;
using System.Diagnostics;
namespace OpenRepo.Services
{
    public class CacheService<T>
    {
        private readonly long m_lifetime;
        private readonly Func<T> m_factory;
        private readonly Stopwatch m_stopwatch = new Stopwatch();
        private T m_cachedValue;

        public CacheService(long lifetime, Func<T> factory)
        {
            m_lifetime = lifetime;
            m_factory = factory;
            m_cachedValue = factory();
            m_stopwatch.Start();
        }

        public T GetValue()
        {
            if (m_stopwatch.ElapsedMilliseconds < m_lifetime) return m_cachedValue;
            m_stopwatch.Restart();
            m_cachedValue = m_factory();
            return m_cachedValue;
        }
    }
}
