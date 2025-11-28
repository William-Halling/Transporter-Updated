using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }

    private Dictionary<Type, object> services = new Dictionary<Type, object>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void RegisterService<T>(T service) where T : class
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"Service of type {type} already registered. Replacing with new service.");
            services[type] = service;
        }
        else
        {
            services.Add(type, service);
            Debug.Log($"Registered service: {type.Name}");
        }
    }


    public T GetService<T>() where T : class
    {
        Type type = typeof(T);


        if (services.ContainsKey(type))
        {
            return services[type] as T;
        }


        Debug.LogError($"Service of type {type} not registered.");
        
        
        return null;
    }



    public bool HasService<T>() where T : class
    {
        return services.ContainsKey(typeof(T));
    }
}