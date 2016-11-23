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
    MUTL,

    /// <summary>
    /// register account
    /// req 
    /// rep
    /// </summary>
    REGISTERACCOUNT,

    /// <summary>
    /// login account
    /// req 
    /// rep
    /// </summary>
    LOGIN,


    /// <summary>
    /// join in battle
    /// req 
    /// rep
    /// </summary>
    JOININBATTLE,

    /// <summary>
    /// bet
    /// req 
    /// rep
    /// </summary>
    BET,
}

