using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Api.Models {
    public class RedisOptions {
        public RedisOptions (IConfiguration configuration) {
            SetConfig (configuration);
        }
        
        public DistributedCacheEntryOptions CacheOptions { get; private set; }
        private IServer Server { get; set; }
        public string InstanceName { get; private set; }
        public string ConnectionString { get; private set; }
        public ConnectionMultiplexer Connection { get; private set; }

        private void SetConfig (IConfiguration configuration) {
            // Cache options
            CacheOptions = new DistributedCacheEntryOptions ();
            CacheOptions.SetAbsoluteExpiration (TimeSpan.FromMinutes (60));

            // Variables
            ConnectionString = configuration.GetConnectionString ("RedisConnection");
            InstanceName = configuration.GetSection ("InstanceName").Value;

            // Connection
            ConfigurationOptions options = ConfigurationOptions.Parse (ConnectionString);
            Connection = ConnectionMultiplexer.Connect (options);

            // Server
            EndPoint endPoint = Connection.GetEndPoints ().First ();
            Server = Connection.GetServer (endPoint);
        }

        public string[] GetAllKeys () {
            var keys = Server.Keys (pattern: $"*{InstanceName}*").ToArray ();
            var lstResult = new List<string> ();
            foreach (var key in keys) lstResult.Add (key.ToString ().Replace (InstanceName, ""));

            return lstResult.ToArray ();
        }

        public T[] GetAll<T> (IDistributedCache distributedCache) {
            var keys = Server.Keys (pattern: $"*{InstanceName}*").ToArray ();
            var lstResult = new List<T> ();
            foreach (var key in keys) lstResult.Add (
                JsonConvert.DeserializeObject<T> (
                    distributedCache.GetString (
                        key.ToString ().Replace (InstanceName, "")
                    )
                )
            );

            return lstResult.ToArray ();
        }
    }
}