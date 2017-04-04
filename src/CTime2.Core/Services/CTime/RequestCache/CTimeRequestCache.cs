using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UwCore.Common;
using UwCore.Services.Clock;

namespace CTime2.Core.Services.CTime.RequestCache
{
    public class CTimeRequestCache : ICTimeRequestCache
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

        private readonly IClock _clock;

        private readonly ConcurrentDictionary<string, ValueHolder> _cache;

        public CTimeRequestCache(IClock clock)
        {
            Guard.NotNull(clock, nameof(clock));

            this._clock = clock;

            this._cache = new ConcurrentDictionary<string, ValueHolder>();
        }

        public void Cache(string function, Dictionary<string, string> data, string response)
        {
            Guard.NotNullOrWhiteSpace(function, nameof(function));
            Guard.NotNull(data, nameof(data));

            var key = this.ComputeKey(function, data);
            var holder = ValueHolder.Create(response, this._clock.Now());

            this._cache.AddOrUpdate(key, holder, (_, __) => holder);
        }

        public bool TryGetCached(string function, Dictionary<string, string> data, out string response)
        {
            Guard.NotNullOrWhiteSpace(function, nameof(function));
            Guard.NotNull(data, nameof(data));

            response = null;

            var key = this.ComputeKey(function, data);

            ValueHolder holder;

            if (this._cache.TryGetValue(key, out holder) == false)
                return false;
            
            if (holder.Time.Add(CacheDuration) <= this._clock.Now())
            {
                //Value timed out, remove it from the cache
                this._cache.TryRemove(key, out holder);
                return false;
            }

            response = holder.CachedValue;
            return true;
        }

        public void Clear()
        {
            this._cache.Clear();
        }

        private string ComputeKey(string function, Dictionary<string, string> data)
        {
            Guard.NotNullOrWhiteSpace(function, nameof(function));
            Guard.NotNull(data, nameof(data));

            var keyObject = JObject.FromObject(new
            {
                Function = function,
                Data = data
            });

            return keyObject.ToString();
        }

        private class ValueHolder
        {
            public static ValueHolder Create(string value, DateTimeOffset time)
            {
                return new ValueHolder
                {
                    CachedValue = value,
                    Time = time
                };
            }

            public string CachedValue { get; private set; }
            public DateTimeOffset Time { get; private set; }
        }
    }
}