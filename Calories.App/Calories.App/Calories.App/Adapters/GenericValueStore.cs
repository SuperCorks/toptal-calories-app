using System;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;

using Akavache;

namespace Calories.App.Adapters
{
    /// <summary>Parent class of persistent key-value stores with specific value types.</summary>
    /// 
    /// <typeparam name="TValue">The type of value to store.</typeparam>
    public abstract class GenericValueStore<TValue>
    {
        private readonly IBlobCache Store = BlobCache.UserAccount;

        /// <param name="bytes">Can be null.</param>
        protected abstract TValue Deserialize(byte[] bytes);

        protected abstract byte[] Serialize(TValue value);

        /// <summary>
        /// Gets a value from local storage.
        /// </summary>
        /// 
        /// <param name="key">The key associated to the value to retrieve.</param>
        /// 
        /// <exception cref="KeyNotFoundException">If the key doesn't exist in the store.</exception>
        /// 
        /// <returns>
        /// A task that completes when the operation finishes and returns the value read from the local store.
        /// </returns>
        public Task<TValue> this[string key] => this.Store.Get(key).Select(bytes => this.Deserialize(bytes)).ToTask();

        /// <summary>
        /// Gets a value from local storage.
        /// </summary>
        /// 
        /// <param name="key">The key associated to the value to retrieve.</param>
        /// 
        /// <exception cref="KeyNotFoundException">If the key doesn't exist in the store.</exception>
        /// 
        /// <returns>
        /// A task that completes when the operation finishes and returns the value read from the local store.
        /// </returns>
        /// 
        /// <seealso cref="this[string]"/>
        public Task<TValue> Get(string key) => this[key];

        /// <summary>
        /// For debug purposes only.
        /// </summary>
        public Task<IEnumerable<string>> Keys => this.Store.GetAllKeys().ToTask();

        /// <summary>
        /// Sets a value in local storage.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// 
        /// <returns>A task that completes when the operation finishes.</returns>
        public async Task Set(string key, TValue value)
        {
            await this.Store.Insert(key, this.Serialize(value)).ToTask();
        }

        /// <summary>
        /// Removes a key-value pair from the store. Does not throw an exception if the key is not found.
        /// </summary>
        /// 
        /// <param name="key">The key to remove from the store.</param>
        /// 
        /// <returns>A task that completes when the operation finishes.</returns>
        public async Task Remove(string key)
        {
            await this.Store.Invalidate(key).ToTask();
        }

        /// <summary>Clears all the keys in storage.</summary>
        /// 
        /// <returns>A task that completes when the operation finishes.</returns>
        public async Task Clear()
        {
            await this.Store.InvalidateAll().ToTask();
        }
    }
}
