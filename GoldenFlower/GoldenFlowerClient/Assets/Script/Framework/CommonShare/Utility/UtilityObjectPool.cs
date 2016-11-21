using System.Collections;
using System;
using System.Collections.Generic;

public class UtilityObjectPool {
     static UtilityObjectPool _instance;

    public static void CreateInstance()
    {
        _instance = new UtilityObjectPool();
    }
    public static UtilityObjectPool Instance
    {
        get
        {
            return _instance;
        }
    }



    Dictionary<Type, Queue> _type2InstanceQueue = new Dictionary<Type, Queue>();

    // Use this for initialization
    UtilityObjectPool()
    {

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

    #region 重用Byte[]
    Dictionary<int, Queue<byte[]>> _byteNum2BytesQueue = new Dictionary<int, Queue<byte[]>>();
    Queue<byte[]> GetBytesQueue(int vByteNum)
    {
        Queue<byte[]> bytesQueue;
        if(!_byteNum2BytesQueue.TryGetValue(vByteNum, out bytesQueue))
        {
            bytesQueue = new Queue<byte[]>();
            _byteNum2BytesQueue[vByteNum] = bytesQueue;
        }
        return bytesQueue;
    }
    public byte[] DequeueBytes(int vByteNum)
    {
        Queue<byte[]> bytesQueue = GetBytesQueue(vByteNum);
        if (!bytesQueue.IsNullOrEmpty())
        {
            return bytesQueue.Dequeue();
        }
        else
        {
            return new byte[vByteNum];
        }
    }

    public void EnqueueBytes(byte[] vBytes)
    {
        Queue<byte[]> bytesQueue = GetBytesQueue(vBytes.Length);

        //缓存太多，不再缓存了
        if (bytesQueue.Count > 100)
            return;

        bytesQueue.Enqueue(vBytes);
    }
    #endregion
}


public interface IObjectPool
{
    void OnRecycle();
}