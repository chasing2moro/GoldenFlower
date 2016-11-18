using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum CommandName
{
    //此字段服务器使用，客户端不要使用
    MyCommandBase = 0,
    ECHO = 1,//一定要1开始
    ADD,
    MUTL
}

