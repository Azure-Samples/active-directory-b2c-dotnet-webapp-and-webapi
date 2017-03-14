using Microsoft.Identity.Client;

using System;
using System.Web;

namespace TaskWebApp
{
    /*
     * This object is a naive representation of the token cache
     */ 
    public class NaiveSessionCache : TokenCache
    {
        private static readonly object FileLock = new object();
        string UserObjectId = string.Empty;
        string CacheId = string.Empty;
        HttpContextBase httpContext = null;

        public NaiveSessionCache(string userId, HttpContextBase httpcontext)
        {
            UserObjectId = userId;
            CacheId = UserObjectId + "_TokenCache";
            httpContext = httpcontext;

            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            Load();
        }

        /*
         * Load the cache from the persistent store
         */
        public void Load()
        {
            lock (FileLock)
            {
                try
                {
                    this.Deserialize((byte[])httpContext.Session[CacheId]);
                }
                catch (NullReferenceException e)
                {
                    Console.Out.WriteLine("Problem looking up the current session: " + e.ToString());
                }

            }
        }

        /* 
         * Write changes to the persistent store
         */
        public void Persist()
        {
            lock (FileLock)
            {
                try
                {
                    // reflect changes in the persistent store
                    httpContext.Session[CacheId] = this.Serialize();
                    // once the write operation took place, restore the HasStateChanged bit to false
                    this.HasStateChanged = false;
                }
                catch (NullReferenceException e)
                {
                    Console.Out.WriteLine("Problem setting the file lock: " + e.ToString());
                }
            }
        }

        // Clear/empty the cache
        public override void Clear(String clientId)
        {
            base.Clear(clientId);
            httpContext.Session.Remove(CacheId);
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.HasStateChanged)
            {
                Persist();
            }
        }
    }
}