// Version 1.2
// Date: 2014-03-27
// http://sh.codeplex.com
// Dedicated to Public Domain

//框架网页http://www.codeproject.com/Articles/746191/SQLite-Helper-Csharp
//库下载网页http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
//一定要把SQLite.Interop.dll 考到bin下面
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;

namespace System.Data.SQLite
{
    public enum ColType
    {
        Text,
        DateTime,
        Integer,
        Decimal,
        BLOB
    }

    public class SQLiteHelper
    {
        SQLiteCommand cmd = null;

        public SQLiteHelper(SQLiteCommand command)
        {
            cmd = command;
        }

        #region DB Info

        public DataTable GetTableStatus()
        {
            return Select("SELECT * FROM sqlite_master;");
        }

        public DataTable GetTableList()
        {
            DataTable dt = GetTableStatus();
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Tables");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string t = dt.Rows[i]["name"] + "";
                if (t != "sqlite_sequence")
                    dt2.Rows.Add(t);
            }
            return dt2;
        }

        public DataTable GetColumnStatus(string tableName)
        {
            return Select(string.Format("PRAGMA table_info(`{0}`);", tableName));
        }

        public DataTable ShowDatabase()
        {
            return Select("PRAGMA database_list;");
        }

        #endregion

        #region Query

        public void BeginTransaction()
        {
            cmd.CommandText = "begin transaction;";
            cmd.ExecuteNonQuery();
        }

        public void Commit()
        {
            cmd.CommandText = "commit;";
            cmd.ExecuteNonQuery();
        }

        public void Rollback()
        {
            cmd.CommandText = "rollback";
            cmd.ExecuteNonQuery();
        }

        public DataTable Select(string sql)
        {
            return Select(sql, new List<SQLiteParameter>());
        }

        public DataTable Select(string sql, Dictionary<string, object> dicParameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(dicParameters);
            return Select(sql, lst);
        }

        public DataTable Select(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void Execute(string sql)
        {
            Execute(sql, new List<SQLiteParameter>());
        }

        public void Execute(string sql, Dictionary<string, object> dicParameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(dicParameters);
            Execute(sql, lst);
        }

        public void Execute(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }
            cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            cmd.CommandText = sql;
            return cmd.ExecuteScalar();
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> dicParameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(dicParameters);
            return ExecuteScalar(sql, lst);
        }

        public object ExecuteScalar(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            return cmd.ExecuteScalar();
        }

        public dataType ExecuteScalar<dataType>(string sql, Dictionary<string, object> dicParameters = null)
        {
            List<SQLiteParameter> lst = null;
            if (dicParameters != null)
            {
                lst = new List<SQLiteParameter>();
                foreach (KeyValuePair<string, object> kv in dicParameters)
                {
                    lst.Add(new SQLiteParameter(kv.Key, kv.Value));
                }
            }
            return ExecuteScalar<dataType>(sql, lst);
        }

        public dataType ExecuteScalar<dataType>(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            return (dataType)Convert.ChangeType(cmd.ExecuteScalar(), typeof(dataType));
        }

        public dataType ExecuteScalar<dataType>(string sql)
        {
            cmd.CommandText = sql;
            return (dataType)Convert.ChangeType(cmd.ExecuteScalar(), typeof(dataType));
        }

        private List<SQLiteParameter> GetParametersList(Dictionary<string, object> dicParameters)
        {
            List<SQLiteParameter> lst = new List<SQLiteParameter>();
            if (dicParameters != null)
            {
                foreach (KeyValuePair<string, object> kv in dicParameters)
                {
                    lst.Add(new SQLiteParameter(kv.Key, kv.Value));
                }
            }
            return lst;
        }

        public string Escape(string data)
        {
            data = data.Replace("'", "''");
            data = data.Replace("\\", "\\\\");
            return data;
        }

        public void Insert(string tableName, Dictionary<string, object> dic)
        {
            StringBuilder sbCol = new System.Text.StringBuilder();
            StringBuilder sbVal = new System.Text.StringBuilder();

            foreach (KeyValuePair<string, object> kv in dic)
            {
                if (sbCol.Length == 0)
                {
                    sbCol.Append("insert into ");
                    sbCol.Append(tableName);
                    sbCol.Append("(");
                }
                else
                {
                    sbCol.Append(",");
                }

                sbCol.Append("`");
                sbCol.Append(kv.Key);
                sbCol.Append("`");

                if (sbVal.Length == 0)
                {
                    sbVal.Append(" values(");
                }
                else
                {
                    sbVal.Append(", ");
                }

                sbVal.Append("@v");
                sbVal.Append(kv.Key);
            }

            sbCol.Append(") ");
            sbVal.Append(");");

            cmd.CommandText = sbCol.ToString() + sbVal.ToString();

            foreach (KeyValuePair<string, object> kv in dic)
            {
                cmd.Parameters.AddWithValue("@v" + kv.Key, kv.Value);
            }

            cmd.ExecuteNonQuery();
        }

        public void Update(string tableName, Dictionary<string, object> dicData, string colCond, object varCond)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic[colCond] = varCond;
            Update(tableName, dicData, dic);
        }

        public void Update(string tableName, Dictionary<string, object> dicData, Dictionary<string, object> dicCond)
        {
            if (dicData.Count == 0)
                throw new Exception("dicData is empty.");

            StringBuilder sbData = new System.Text.StringBuilder();

            Dictionary<string, object> _dicTypeSource = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> kv1 in dicData)
            {
                _dicTypeSource[kv1.Key] = null;
            }

            foreach (KeyValuePair<string, object> kv2 in dicCond)
            {
                if (!_dicTypeSource.ContainsKey(kv2.Key))
                    _dicTypeSource[kv2.Key] = null;
            }

            sbData.Append("update `");
            sbData.Append(tableName);
            sbData.Append("` set ");

            bool firstRecord = true;

            foreach (KeyValuePair<string, object> kv in dicData)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                    sbData.Append(",");

                sbData.Append("`");
                sbData.Append(kv.Key);
                sbData.Append("` = ");

                sbData.Append("@v");
                sbData.Append(kv.Key);
            }

            sbData.Append(" where ");

            firstRecord = true;

            foreach (KeyValuePair<string, object> kv in dicCond)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                {
                    sbData.Append(" and ");
                }

                sbData.Append("`");
                sbData.Append(kv.Key);
                sbData.Append("` = ");

                sbData.Append("@c");
                sbData.Append(kv.Key);
            }

            sbData.Append(";");

            cmd.CommandText = sbData.ToString();

            foreach (KeyValuePair<string, object> kv in dicData)
            {
                cmd.Parameters.AddWithValue("@v" + kv.Key, kv.Value);
            }

            foreach (KeyValuePair<string, object> kv in dicCond)
            {
                cmd.Parameters.AddWithValue("@c" + kv.Key, kv.Value);
            }

            cmd.ExecuteNonQuery();
        }

        public long LastInsertRowId()
        {
            return ExecuteScalar<long>("select last_insert_rowid();");
        }

        #endregion

        #region Utilities

        public void CreateTable(SQLiteTable table)
        {
            StringBuilder sb = new Text.StringBuilder();
            sb.Append("create table if not exists `");
            sb.Append(table.TableName);
            sb.AppendLine("`(");

            bool firstRecord = true;

            foreach (SQLiteColumn col in table.Columns)
            {
                if (col.ColumnName.Trim().Length == 0)
                {
                    throw new Exception("Column name cannot be blank.");
                }

                if (firstRecord)
                    firstRecord = false;
                else
                    sb.AppendLine(",");

                sb.Append(col.ColumnName);
                sb.Append(" ");

                if (col.AutoIncrement)
                {

                    sb.Append("integer primary key autoincrement");
                    continue;
                }

                switch (col.ColDataType)
                {
                    case ColType.Text:
                        sb.Append("text"); break;
                    case ColType.Integer:
                        sb.Append("integer"); break;
                    case ColType.Decimal:
                        sb.Append("decimal"); break;
                    case ColType.DateTime:
                        sb.Append("datetime"); break;
                    case ColType.BLOB:
                        sb.Append("blob"); break;
                }

                if (col.PrimaryKey)
                    sb.Append(" primary key");
                else if (col.NotNull)
                    sb.Append(" not null");
                else if (col.DefaultValue.Length > 0)
                {
                    sb.Append(" default ");

                    if (col.DefaultValue.Contains(" ") || col.ColDataType == ColType.Text || col.ColDataType == ColType.DateTime)
                    {
                        sb.Append("'");
                        sb.Append(col.DefaultValue);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(col.DefaultValue);
                    }
                }
            }

            sb.AppendLine(");");

            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery();
        }

        public void RenameTable(string tableFrom, string tableTo)
        {
            cmd.CommandText = string.Format("alter table `{0}` rename to `{1}`;", tableFrom, tableTo);
            cmd.ExecuteNonQuery();
        }

        public void CopyAllData(string tableFrom, string tableTo)
        {
            DataTable dt1 = Select(string.Format("select * from `{0}` where 1 = 2;", tableFrom));
            DataTable dt2 = Select(string.Format("select * from `{0}` where 1 = 2;", tableTo));

            Dictionary<string, bool> dic = new Dictionary<string, bool>();

            foreach (DataColumn dc in dt1.Columns)
            {
                if (dt2.Columns.Contains(dc.ColumnName))
                {
                    if (!dic.ContainsKey(dc.ColumnName))
                    {
                        dic[dc.ColumnName] = true;
                    }
                }
            }

            foreach (DataColumn dc in dt2.Columns)
            {
                if (dt1.Columns.Contains(dc.ColumnName))
                {
                    if (!dic.ContainsKey(dc.ColumnName))
                    {
                        dic[dc.ColumnName] = true;
                    }
                }
            }

            StringBuilder sb = new Text.StringBuilder();

            foreach (KeyValuePair<string, bool> kv in dic)
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append("`");
                sb.Append(kv.Key);
                sb.Append("`");
            }

            StringBuilder sb2 = new Text.StringBuilder();
            sb2.Append("insert into `");
            sb2.Append(tableTo);
            sb2.Append("`(");
            sb2.Append(sb.ToString());
            sb2.Append(") select ");
            sb2.Append(sb.ToString());
            sb2.Append(" from `");
            sb2.Append(tableFrom);
            sb2.Append("`;");

            cmd.CommandText = sb2.ToString();
            cmd.ExecuteNonQuery();
        }

        public void DropTable(string table)
        {
            cmd.CommandText = string.Format("drop table if exists `{0}`", table);
            cmd.ExecuteNonQuery();
        }

        public void UpdateTableStructure(string targetTable, SQLiteTable newStructure)
        {
            newStructure.TableName = targetTable + "_temp";

            CreateTable(newStructure);

            CopyAllData(targetTable, newStructure.TableName);

            DropTable(targetTable);

            RenameTable(newStructure.TableName, targetTable);
        }

        public void AttachDatabase(string database, string alias)
        {
            Execute(string.Format("attach '{0}' as {1};", database, alias));
        }

        public void DetachDatabase(string alias)
        {
            Execute(string.Format("detach {0};", alias));
        }

        #endregion

    }
}

#if false
using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;

public class SQLiteHelper
{
    /// <summary>
    /// 数据库连接定义
    /// </summary>
    private SqliteConnection dbConnection;

    /// <summary>
    /// SQL命令定义
    /// </summary>
    private SqliteCommand dbCommand;

    /// <summary>
    /// 数据读取定义
    /// </summary>
    private SqliteDataReader dataReader;

    /// <summary>
    /// 构造函数    
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SQLiteHelper(string connectionString)
    {
        try{
            //构造数据库连接
            dbConnection=new SqliteConnection(connectionString);
            //打开数据库
            dbConnection.Open();
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SqliteDataReader ExecuteQuery(string queryString)
    {
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = queryString;
        dataReader = dbCommand.ExecuteReader();
        return dataReader;
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void CloseConnection()
    {
        //销毁Command
        if(dbCommand != null){
            dbCommand.Cancel();
        }
        dbCommand = null;

        //销毁Reader
        if(dataReader != null){
            dataReader.Close();
        }
        dataReader = null;

        //销毁Connection
        if(dbConnection != null){
            dbConnection.Close();
        }
        dbConnection = null;
    }

    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    public SqliteDataReader ReadFullTable(string tableName)
    {
        string queryString = "SELECT * FROM " + tableName;
        return ExecuteQuery (queryString);
    }

    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public SqliteDataReader InsertValues(string tableName,string[] values)
    {
        //获取数据表中字段数目
        int fieldCount=ReadFullTable(tableName).FieldCount;
        //当插入的数据长度不等于字段数目时引发异常
        if(values.Length!=fieldCount){
            throw new SqliteException("values.Length!=fieldCount");
        }

        string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for(int i=1; i<values.Length; i++)
        {
            queryString+=", " + values[i];
        }
        queryString += " )";
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字对应的值</param>
    public SqliteDataReader UpdateValues(string tableName,string[] colNames,string[] colValues,string key,string operation,string value)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length");
        }

        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+=", " + colNames[i] + "=" + colValues[i];
        }
        queryString += " WHERE " + key + operation + value;
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesOR(string tableName,string[] colNames,string[] operations,string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+="OR " + colNames[i] + operations[0] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesAND(string tableName,string[] colNames,string[] operations,string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if(colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length) {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for(int i=1; i<colValues.Length; i++) 
        {
            queryString+=" AND " + colNames[i] + operations[i] + colValues[i];
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public SqliteDataReader CreateTable(string tableName,string[] colNames,string[] colTypes)
    {
        string queryString = "CREATE TABLE " + tableName + "( " + colNames [0] + " " + colTypes [0];
        for (int i=1; i<colNames.Length; i++) 
        {
            queryString+=", " + colNames[i] + " " + colTypes[i];
        }
        queryString+= "  ) ";
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    public SqliteDataReader ReadTable(string tableName,string[] items,string[] colNames,string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items [0];
        for (int i=1; i<items.Length; i++) 
        {
            queryString+=", " + items[i];
        }
        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " +  operations[0] + " " + colValues[0];
        for (int i=0; i<colNames.Length; i++) 
        {
            queryString+=" AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
        }
        return ExecuteQuery(queryString);
    }
}
#endif