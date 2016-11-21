using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UtilityInterface
{
    public static void StartUp()
    {

        UtilityObjectPool.CreateInstance();
#if !UNITY_CLIENT
        UtilityDataBase.CreateInstance();
#endif
    }
}

