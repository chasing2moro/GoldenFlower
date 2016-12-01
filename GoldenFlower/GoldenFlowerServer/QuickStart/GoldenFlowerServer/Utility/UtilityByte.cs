using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



public class UtilityByte
{
    public static int GetInt(byte[] bytes)
    {
       return System.BitConverter.ToInt32(bytes, 0);
    }
}
