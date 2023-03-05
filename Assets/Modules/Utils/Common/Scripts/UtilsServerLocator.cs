using System.Collections.Generic;
using System;
using Unity.Assertions;
using UnityEngine;

namespace Utils 
{
    public class UtilsServerLocator
    {
        public static UtilsServerLocator Instance => instance ?? (instance = new UtilsServerLocator());
        private static UtilsServerLocator instance;

        private readonly Dictionary<Type, object> services;

        public UtilsServerLocator() 
        {
            services = new Dictionary<Type, object>();
        }

        public void RegisterService<T>(T service) 
        {
            var type = typeof(T);
            //Assert.IsFalse(services.ContainsKey(type), $"Service {type} already registered");
            if (!services.ContainsKey(type)) 
            {
                services.Add(type, service);
            }
        }

        public T GetService<T>() 
        {
            var type = typeof(T);

            if (!services.TryGetValue(type, out var service)) 
            {
                throw new Exception ($"Service {type} not found");
            }

            return (T) service;
        }
    }
}

