using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class UtiltyCommon
{
    public static  bool IsNullOrEmpty(this Array vArray)
    {
        return vArray == null || vArray.Length == 0;
    }
}

