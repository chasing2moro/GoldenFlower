using System;
using System.Collections.Generic;


/// <summary>
/// 客户端和服务器公用
/// </summary>
public class UtilityMsg
{
    static Dictionary<CommandName, string> _commandName2Header;

    // 2个下划线开始，就是临时变量
    static string __header;
    static int __index;

    /// <summary>
    /// 客户端：CommandName转成 Header（0001、0002、0003……）发出去
    /// 服务器：根据Command类转成 Header 匹配到此CommandName，然后处理它
    /// </summary>
    /// <param name="vCommandName"></param>
    /// <returns></returns>
    public static string GetHeaderByCommandName(CommandName vCommandName)
    {
        if (_commandName2Header == null)
            _commandName2Header = new Dictionary<CommandName, string>();

        if (!_commandName2Header.TryGetValue(vCommandName, out __header))
        {
            //不够4位，补4个0
            __header = ((int)vCommandName).ToString().PadLeft(4, '0');

            _commandName2Header[vCommandName] = __header;
        }

        return __header;
    }

    public static CommandName GetCommandNameByHeader(string vHeader)
    {
        vHeader = vHeader.TrimStart('0');
        __index = int.Parse(vHeader);
        return ((CommandName)__index);
    }
}

