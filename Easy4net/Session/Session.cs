using Easy4net.Common;
using Easy4net.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Easy4net.Session
{
    public class Session
    {
        private IDbTransaction m_Transaction = null;

        private Session() { }

        public static Session PriviteInstance()
        {
            Session session = new Session();
            return session;
        }
        public static Session GetCurrentSession()
        {
            Session session = SessionFactory.GetSession();
            return session;
        }

        public static Session OpenSession()
        {
            Session session = new Session();
            return session;
        }

        public void BeginTransaction()
        {
            m_Transaction = DbFactory.CreateDbTransaction();
        }

        public void Commit()
        {
            if (m_Transaction != null && m_Transaction.Connection != null)
            {
                if (m_Transaction.Connection.State != ConnectionState.Closed)
                {
                    m_Transaction.Commit();
                }
            }
        }

        public void Rollback()
        {
            if (m_Transaction != null && m_Transaction.Connection != null)
            {
                if (m_Transaction.Connection.State != ConnectionState.Closed)
                {
                    m_Transaction.Rollback();
                }
            }
        }

        private IDbTransaction GetTransaction()
        {
            return m_Transaction;
        }

        #region 将实体数据保存到数据库
        public int Insert<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;

            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                Type classType = entity.GetType();
                //从实体对象的属性配置上获取对应的表信息
                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.INSERT, properties);

                //获取SQL语句
                String strSql = EntityHelper.GetInsertSql(tableInfo);

                //获取参数
                IDbDataParameter[] parms = tableInfo.GetParameters();

                //Access数据库执行不需要命名参数
                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    //执行Insert命令
                    strSql = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSql, parms);
                    val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSql, parms);

                    //如果是Access数据库，另外执行获取自动生成的ID
                    String autoSql = EntityHelper.GetAutoSql();
                    val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, autoSql);
                }
                else
                {
                    //执行Insert命令
                    val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSql, parms);
                }

                //把自动生成的主键ID赋值给返回的对象
                if (AdoHelper.DbType == DatabaseType.SQLSERVER || AdoHelper.DbType == DatabaseType.MYSQL || AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    PropertyInfo propertyInfo = EntityHelper.GetPrimaryKeyPropertyInfo(entity, properties);
                    ReflectionHelper.SetPropertyValue(entity, propertyInfo, val);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量保存
        public int Insert<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;

            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                //从实体对象的属性配置上获取对应的表信息
                T firstEntity = entityList[0];
                Type classType = firstEntity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.INSERT, properties);

                //获取SQL语句
                String strSQL = EntityHelper.GetInsertSql(tableInfo);
                foreach (T entity in entityList)
                {
                    //从实体对象的属性配置上获取对应的表信息
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.INSERT, properties);

                    //获取参数
                    IDbDataParameter[] parms = tableInfo.GetParameters();

                    //Access数据库执行不需要命名参数
                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        //执行Insert命令
                        strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                        val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSQL);

                        //如果是Access数据库，另外执行获取自动生成的ID
                        String autoSql = EntityHelper.GetAutoSql();
                        val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, autoSql);
                    }
                    else
                    {
                        //执行Insert命令
                        val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSQL, parms);
                    }

                    //把自动生成的主键ID赋值给返回的对象
                    if (AdoHelper.DbType == DatabaseType.SQLSERVER || AdoHelper.DbType == DatabaseType.MYSQL || AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        PropertyInfo propertyInfo = EntityHelper.GetPrimaryKeyPropertyInfo(entity, properties);
                        ReflectionHelper.SetPropertyValue(entity, propertyInfo, val);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 将实体数据修改到数据库
        public int Update<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;

            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                Type classType = entity.GetType();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.UPDATE, properties);

                String strSQL = EntityHelper.GetUpdateSql(tableInfo);

                tableInfo.Columns.Add(tableInfo.Id.Key, tableInfo.Id.Value);
                IDbDataParameter[] parms = tableInfo.GetParameters();
                
                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }

                //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSql, parms);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量更新
        public int Update<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                T firstEntity = entityList[0];
                Type classType = firstEntity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(firstEntity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.UPDATE, properties);
                String strSQL = EntityHelper.GetUpdateSql(tableInfo);
                
                tableInfo.Columns.Add(tableInfo.Id.Key, tableInfo.Id.Value);
                IDbDataParameter[] parms = tableInfo.GetParameters();

                foreach (T entity in entityList)
                {
                    TableInfo table = EntityHelper.GetTableInfo(entity, DbOperateType.UPDATE, properties);

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                        val = AdoHelper.ExecuteNonQuery(connection, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(connection, CommandType.Text, strSQL, parms);
                    }

                    //val = AdoHelper.ExecuteNonQuery(connection, CommandType.Text, strSQL, parms);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 执行SQL语句
        public int ExcuteSQL(string strSQL, ParamMap param)
        {
            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                IDbDataParameter[] parms = param.toDbParameters();

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(strSQL, parms);
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }

                //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 执行SQL语句
        public int ExcuteSQL(string strSQL)
        {
            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion
        #region 执行SQL语句
        public object ExecuteScalar(string strSQL)
        {
            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                val = AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSQL);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return val;
        }
        #endregion

        #region 删除实体对应数据库中的数据
        public int Delete<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                Type classType = entity.GetType();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = tableInfo.Id.Value;

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }

                //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量删除
        public int Delete<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                T firstEntity = entityList[0];
                Type classType = firstEntity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(firstEntity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                foreach (T entity in entityList)
                {
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                    IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                    parms[0].ParameterName = tableInfo.Id.Key;
                    parms[0].Value = tableInfo.Id.Value;

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                        val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                    }

                    //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 根据主键id删除实体对应数据库中的数据
        public int Delete<T>(object id) where T : new()
        {
            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                T entity = new T();
                Type classType = entity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }

                //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量根据主键id删除数据
        public int Delete<T>(object[] ids) where T : new()
        {
            if (ids == null || ids.Length == 0) return 0;

            object val = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                connection = GetConnection();
                transaction = GetTransaction();

                T entity = new T();
                Type classType = entity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                foreach (object id in ids)
                {
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                    IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                    parms[0].ParameterName = tableInfo.Id.Key;
                    parms[0].Value = id;

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                        val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                    }

                    //val = AdoHelper.ExecuteNonQuery(connection, transaction, CommandType.Text, strSQL, parms);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (m_Transaction == null)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 通过自定义SQL语句查询记录数
        public int Count(string strSQL)
        {
            int count = 0;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            bool closeConnection = GetWillConnectionState();
            try
            {
                connection = GetConnection();
                transaction = GetTransaction();
                count = Convert.ToInt32(AdoHelper.ExecuteScalar(connection, transaction, CommandType.Text, strSQL));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            return count;
        }
        #endregion

        #region 通过自定义SQL语句查询记录数
        public int Count(string strSql, ParamMap param)
        {
            int count = 0;
            IDbConnection connection = null;
            bool closeConnection = GetWillConnectionState();
            try
            {
                connection = GetConnection();

                strSql = strSql.ToLower();
                String columns = SQLBuilderHelper.fetchColumns(strSql);

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSql = SQLBuilderHelper.builderAccessSQL(strSql, param.toDbParameters());
                    count = Convert.ToInt32(AdoHelper.ExecuteScalar(connection, CommandType.Text, strSql));
                }
                else 
                {
                    count = Convert.ToInt32(AdoHelper.ExecuteScalar(connection, CommandType.Text, strSql, param.toDbParameters()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }

            return count;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> Find<T>(string strSql) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                connection = GetConnection();
                transaction = GetTransaction();
                bool closeConnection = GetWillConnectionState();

                strSql = strSql.ToLower();
                String columns = SQLBuilderHelper.fetchColumns(strSql);

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);

                sdr = AdoHelper.ExecuteReader(closeConnection,transaction, connection, CommandType.Text, strSql, null);
                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> Find<T>(string strSQL, ParamMap param) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSQL = strSQL.ToLower();
                String columns = SQLBuilderHelper.fetchColumns(strSQL);

                T entity = new T();
                Type classType = entity.GetType();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                if (param.IsPage && !SQLBuilderHelper.isPage(strSQL))
                {
                    strSQL = SQLBuilderHelper.builderPageSQL(strSQL, param.OrderFields, param.IsDesc);
                }

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, param.toDbParameters());
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());
                }

                //sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());

                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list;
        }
        #endregion

        #region 分页查询返回分页结果
        public PageResult<T> FindPage<T>(string strSQL, ParamMap param) where T : new()
        {
            PageResult<T> pageResult = new PageResult<T>();
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSQL = strSQL.ToLower();
                String countSQL = SQLBuilderHelper.builderCountSQL(strSQL);
                String columns = SQLBuilderHelper.fetchColumns(strSQL);

                T entity = new T();
                Type classType = entity.GetType();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                if (param.IsPage && !SQLBuilderHelper.isPage(strSQL))
                {
                    strSQL = SQLBuilderHelper.builderPageSQL(strSQL, param.OrderFields, param.IsDesc);
                }

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, param.toDbParameters());
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());
                }

                //sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());

                int count = this.Count(countSQL, param);
                list = EntityHelper.toList<T>(sdr, tableInfo, properties);

                pageResult.Total = count;
                pageResult.DataList = list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return pageResult;
        }
        #endregion

        #region 通过主键ID查询数据
        public T Get<T>(object id) where T : new()
        {
            List<T> list = new List<T>();

            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                T entity = new T();
                Type classType = entity.GetType();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(classType);

                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                String strSQL = EntityHelper.GetFindByIdSql(tableInfo);
                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(classType, tableInfo, strSQL, parms);
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, parms);
                }

                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list.FirstOrDefault();
        }
        #endregion

        private IDbConnection GetConnection()
        {
            //获取数据库连接，如果开启了事务，从事务中获取
            IDbConnection connection = null;
            if (m_Transaction != null)
            {
                connection = m_Transaction.Connection;
            }
            else
            {
                connection = DbFactory.CreateDbConnection(AdoHelper.ConnectionString);
            }

            return connection;
        }

        private bool GetWillConnectionState()
        {
            return m_Transaction == null;
        }

    }
}
