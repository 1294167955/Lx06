using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class Comment
    {
        private Comment() { }
        private static Comment _instance = new Comment();
        public static Comment Instance
        {
            get
            {
                return _instance;
            }
        }
        string cns = AppConfigurtaionServices.Configuration.GetConnectionString("cns");
        public Model.Comment GetModel(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from Comment where commentId=@id";
                return cn.QueryFirstOrDefault<Model.Comment>(sql, new { id = id });
            }
        }
        public IEnumerable<Model.Comment> GetAll()
        {
            using(IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "select * from Comment";
                return cn.Query<Model.Comment>(sql);
            }
        }
        public int GetCount()
        {
            using (IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "select count(1) from Comment";
                return cn.ExecuteScalar<int>(sql);
            }
        }
        public IEnumerable<Model.CommentNo> GetPage(Model.Page page)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by commentTime desc) as num, Comment.*,workName from Comment join workinfo on Comment.workId=workinfo.workId)";
                sql += "select * from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize";
                return cn.Query<Model.CommentNo>(sql,page);
            }
        }
        public int GetWorkCount(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from Comment where workId=@workId";
                return cn.ExecuteScalar<int>(sql,new { workId=id});
            }
        }
        public IEnumerable<Model.CommentNo> GetWorkPage(Model.CommentPage page)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by commentTime desc) as num, Comment.* from Comment where Comment.workId=@workId)";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.CommentNo>(sql, page);
            }
        }
        public int Add(Model.Comment comment)
        {
            using(IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "insert into Comment(commentID,workId,userName,CommentContent,CommentTime)" + "values(@commentID,@workId,@userName,@commentContent,@commentTime);";
                sql += "SELECT @@IDENTITY";
                return cn.ExecuteScalar<int>(sql, comment);
            }
        }
        public int Update(Model.Comment comment)
        {
            using(IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "update Comment set CommentContent=@CommentContent,CommentTime=@CommentContent where commentID=@commentID";
                return cn.Execute(sql, comment);
            }
        }
        public int Delete(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "delete from comment where commentId=@id";
                return cn.Execute(sql, new{id=id});
            }
        }
    }
}
