using System.Collections;
using System;
using System.Collections.Generic;

public class UtilityObjectPool {
     static UtilityObjectPool _instance;

    public static UtilityObjectPool Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UtilityObjectPool();
            return _instance;
        }
    }


	
    Dictionary<Type, Queue> _type2InstanceQueue;

    // Use this for initialization
    UtilityObjectPool()
    {
        _type2InstanceQueue = new Dictionary<Type, Queue>();
    }

    Queue GetQueueByType(Type vType)
    {
        Queue queue;
        if(!_type2InstanceQueue.TryGetValue(vType, out queue))
        {
            queue = new Queue();
            _type2InstanceQueue[vType] = queue;
        }
        return queue;
    }

    public void EnqueueWithRecycle<T>(T vInstance) where T : IObjectPool
    {
        //重置一些变量值， 下次可以无障碍使用
        vInstance.OnRecycle();

        //进入 队列
        Enqueue(vInstance);
    }
    
    public void Enqueue<T>(T vInstance)
    {
        Queue queue = GetQueueByType(typeof(T));
        queue.Enqueue(vInstance);
    }

    public T Dequeue<T>() where T:  new()
    {
        Queue queue = GetQueueByType(typeof(T));
        T instance;
        if (queue.Count == 0)
        {
            instance = new T();
        }
        else
        {
            instance = (T)queue.Dequeue();
        }
        return instance;
    }
}


public interface IObjectPool
{
    void OnRecycle();
}