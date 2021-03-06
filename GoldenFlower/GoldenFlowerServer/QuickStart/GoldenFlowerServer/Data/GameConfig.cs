﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameConfig
{
    /// <summary>
    /// 所有的 目录结尾都要 / 结束
    /// </summary>
    public static string RootDir = "../";

    /// <summary>
    /// 数据库目录
    /// </summary>
    public static string DataBasePath = RootDir + "Sqlite/goldenflower.db";

    /// <summary>
    /// 配置表目录
    /// </summary>
    public static string ConfigDir = RootDir + "Config/";
}

