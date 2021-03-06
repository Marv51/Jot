﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jot.Storage.Serialization;
using Jot.Storage;

namespace Jot.Storage
{
    public class StoreData
    {
        public string Serialized { get; set; }
        public Type OriginalType { get; set; }

        public StoreData(string data, Type originalType)
        {
            Serialized = data;
            OriginalType = originalType;
        }

        public static StoreData NullData = new StoreData(null, typeof(object));
    }

    public abstract class PersistentStoreBase : IObjectStore
    {
        public ISerializer Serializer { get; private set; }
        public bool CacheObjects { get; set; }
        public bool RemoveBadData { get; set; }

        public abstract bool ContainsKey(string identifier);
        public abstract void Remove(string identifier);

        protected abstract StoreData GetData(string identifier);
        protected abstract void SetData(StoreData data, string identifier);

        Dictionary<string, object> _createdInstances = new Dictionary<string, object>();

        public PersistentStoreBase(ISerializer serializer)
        {
            Serializer = serializer;
            CacheObjects = true;
            RemoveBadData = true;
        }

        public void Persist(object value, string key)
        {
            _createdInstances[key] = value;
            if (value == null)
                SetData(new StoreData(null, null), key);//todo: handle null in SetData implementations
            else
                SetData(new StoreData(Serializer.Serialize(value), value.GetType()), key);
        }

        public object Retrieve(string key)
        {
            if (!CacheObjects || !_createdInstances.ContainsKey(key))
            {
                try
                {
                    StoreData data = GetData(key);
                    if (data.Serialized == null)
                        return null;
                    else
                        _createdInstances[key] = Serializer.Deserialize(data.Serialized, data.OriginalType);
                }
                catch
                {
                    if (RemoveBadData)
                        Remove(key);
                    throw;
                }
            }
            return _createdInstances[key];
        }
    }
}
