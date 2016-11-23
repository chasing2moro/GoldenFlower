using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class StateBase
{
    protected EntityBase _target;
    public abstract void OnEnterState();
    public void SetTarget(EntityBase vTarget)
    {
        _target = vTarget;
    }
    public T GetTarget<T>() where T:EntityBase
    {
        return _target as T;
    }
}

