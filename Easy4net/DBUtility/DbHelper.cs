
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Easy4net.Common;
using Easy4net.Session;

namespace Easy4net.DBUtility
{
    public class DBHelper
    {
        Session.Session session;
        public DBHelper()
        {
            session = Session.Session.PriviteInstance();
        }

        public static DBHelper getInstance()
        {
            return new DBHelper();
        }

        /// <summary>
        /// 根据主键ID获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public T Get<T>(object id) where T : new()
        {
            return session.Get<T>(id);
        }

        /// <summary>
        /// 插入对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entity">需要插入的数据对象</param>
        /// <returns></returns>
        public int Save<T>(T entity)
        {
            return session.Insert<T>(entity);
        }

        /// <summary>
        /// 批量插入对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entityList">需要插入的数据对象集合</param>
        /// <returns></returns>
        public int Save<T>(List<T> entityList)
        {
            return session.Insert<T>(entityList);
        }

        /// <summary>
        /// 更新对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entity">需要更新的数据对象集合</param>
        /// <returns></returns>
        public int Update<T>(T entity)
        {
            return session.Update<T>(entity);
        }

        /// <summary>
        /// 批量更新对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entityList">需要更新的数据对象集合</param>
        /// <returns></returns>
        public int Update<T>(List<T> entityList)
        {
            return session.Update<T>(entityList);
        }

        /// <summary>
        /// 删除对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entity">需要删除的数据对象</param>
        /// <returns></returns>
        public int Remove<T>(T entity)
        {
            return session.Delete<T>(entity);
        }

        /// <summary>
        /// 批量删除对象数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entityList">需要删除的数据对象集合</param>
        /// <returns></returns>
        public int Remove<T>(List<T> entityList)
        {
            return session.Delete<T>(entityList);
        }

        /// <summary>
        /// 根据主键ID删除数据
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public int Remove<T>(object id) where T : new()
        {
            return session.Delete<T>(id);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public int ExcuteSQL(string strSQL, ParamMap param)
        {
            return session.ExcuteSQL(strSQL, param);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public object ExecuteScalar(string strSQL)
        {
            return session.ExecuteScalar(strSQL);
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public int ExcuteSQL(string strSQL)
        {
            return session.ExcuteSQL(strSQL);
        }

        /// <summary>
        /// 根据SQL查询数量
        /// </summary>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public int Count(string strSQL)
        {
            return session.Count(strSQL);
        }

        /// <summary>
        /// 根据SQL查询记录数
        /// </summary>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public int Count(string strSQL, ParamMap param)
        {
            return session.Count(strSQL, param);
        }

        /// <summary>
        /// 根据SQL查询数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public List<T> FindBySql<T>(string strSQL) where T : new()
        {
            return session.Find<T>(strSQL);
        }

        /// <summary>
        /// 根据SQL查询数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public List<T> FindAll<T>(string tableName) where T : new()
        {
            return session.Find<T>(string.Format("select * from {0}", tableName));
        }

        /// <summary>
        /// 根据SQL查询数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public List<T> FindBySql<T>(string strSQL, ParamMap param) where T : new()
        {
            return session.Find<T>(strSQL, param);
        }

        /// <summary>
        /// 分页查询返回分页对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public PageResult<T> FindPage<T>(string strSQL, ParamMap param) where T : new()
        {
            return session.FindPage<T>(strSQL, param);
        }

        /// <summary>
        /// 分页查询返回分页对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public PageResult<T> FindPageForTable<T>(string tableName, ParamMap param) where T : new()
        {
            return session.FindPage<T>(string.Format("select * from {0}", tableName), param);
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public T FindOne<T>(string strSQL) where T : new()
        {
            return session.Find<T>(strSQL).FirstOrDefault();
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <returns></returns>
        public T FindOneByID<T>(string tableName,string id) where T : new()
        {
            return session.Find<T>(string.Format("select * from {0} where id='{1}'", tableName,id)).FirstOrDefault();
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strSQL">SQL命令</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public T FindOne<T>(string strSQL, ParamMap param) where T : new()
        {
            return session.Find<T>(strSQL, param).FirstOrDefault();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction()
        {
            session.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            session.Commit();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            session.Rollback();
        }
    }
}

