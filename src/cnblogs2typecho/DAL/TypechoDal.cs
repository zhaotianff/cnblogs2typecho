using cnblogs2typecho.DbHelper;
using cnblogs2typecho.Extension;
using cnblogs2typecho.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace cnblogs2typecho.DAL
{
    public class TypechoDal
    {
        private MariaDbHelper mariaDbHelper;


        public bool OpenTypechoDatabase(string server, string port, string database, string uid, string pwd)
        {
            try
            {
                var connectString = $"Server={server}; Port={port}; Database={database}; Uid={uid}; Pwd={pwd}";

                mariaDbHelper = new MariaDbHelper(connectString);
                mariaDbHelper.Open();

                if (mariaDbHelper.ConnectionState == System.Data.ConnectionState.Open)
                    return true;


                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<Typechometa> GetTypechometa()
        {
            var sql = "Select * from typecho_metas";
            var dt = mariaDbHelper.Query(sql);
            return dt.ToList<Typechometa>();
        }

        public void Close()
        {
            mariaDbHelper.Close();
        }

        private bool AddBlog(Blog blog)
        {
            var sql = @"INSERT INTO typecho_contents (title, slug, created, modified, " +
                "`text`,`order`, authorId, template, " +
                "`type`, `status`, `password`, commentsNum, " +
                "allowComment, allowPing, allowFeed, " +
                "parent,views, agree, likes) VALUES (@title, @slug, @created, @modified, " +
                "@text,@order, @authorId, @template, " +
                "@type,@status, @password, @commentsNum, " +
                "@allowComment, @allowPing, @allowFeed, " +
                "@parent, @views, @agree, @likes)";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@title", blog.Title),
                new MySqlParameter("@slug", blog.Slug),
                new MySqlParameter("@created", (long)(blog.CreateDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds),
                new MySqlParameter("@modified",(long)(blog.ModifyDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds),
                new MySqlParameter("@text", "<!--markdown-->" + blog.Content),
                new MySqlParameter("@order", MySqlDbType.Int32){ Value = 0},
                new MySqlParameter("@authorId", MySqlDbType.Int32){ Value = 1},
                new MySqlParameter("@template", null),
                new MySqlParameter("@type", "post"),
                new MySqlParameter("@status", "publish"),
                new MySqlParameter("@password", null),
                new MySqlParameter("@commentsNum", MySqlDbType.Int32){ Value = 0 },
                new MySqlParameter("@allowComment", '1'),
                new MySqlParameter("@allowPing", '1'),
                new MySqlParameter("@allowFeed", '1'),
                new MySqlParameter("@parent", 0),
                new MySqlParameter("@views",MySqlDbType.Int32){ Value = 0},
                new MySqlParameter("@agree",MySqlDbType.Int32 ){ Value =0},
                new MySqlParameter("@likes",MySqlDbType.Int32){ Value = 0}
            };

            try
            {
                var result = mariaDbHelper.ExecuteSql(sql, parameters);

                if (result <= 0)
                    return false;

                sql = "Select cid from typecho_contents order by cid desc";

                var cidObj = mariaDbHelper.QueryFirst(sql);

                if (cidObj == DBNull.Value)
                {
                    return false;
                }

                return CreateCatetoryAndTag(cidObj.ToString(), blog);
            }
            catch
            {
                return false;
            }
        }

        private bool UpdateBlog(Blog blog)
        {
            var deleteContentResult = 0;
            var deleteTagCategoryResult = 0;

            try
            {
                var sql = "Select cid from typecho_contents Where slug = '" + blog.Slug + "'";
                var cidObj = mariaDbHelper.QueryFirst(sql);

                if (cidObj == DBNull.Value)
                {
                    return false;
                }

                sql = $"Delete from typecho_contents WHERE cid = {cidObj}";
                deleteContentResult = mariaDbHelper.ExecuteSql(sql);
                sql = $"Delete from typecho_relationships Where cid = {cidObj}";
                deleteTagCategoryResult = mariaDbHelper.ExecuteSql(sql);

                if (deleteContentResult == 0 || deleteTagCategoryResult == 0)
                    return false;
            }
            catch
            {
                return false;
            }

            return AddBlog(blog);
        }

        public async Task<bool> AddBlogAsync(Blog blog)
        {
            var result = false;
            await Task.Run(() => {
                result =  AddBlog(blog);
            });
            return result;
        }

        public async Task<bool> UpdateBlogAsync(Blog blog)
        {
            var result = false;
            await Task.Run(() => {
                result = UpdateBlog(blog);
            });
            return result;
        }

        public bool IsExistBlog(string slug)
        {
            var sql = "Select * from typecho_contents Where slug = '" + slug + "'";
            return mariaDbHelper.QueryFirst(sql) != null;
        }

        private bool CreateCatetoryAndTag(string cid,Blog blog)
        {
            ReplaceTagsWithmid(blog);
            ReplaceCatetoryWithmid(blog);

            var sql = "Insert Into typecho_relationships (cid,mid) Values (@cid,@mid)";
            var insertTagResult = 0;
            var insertCatetoryResult = 0;

            foreach (var tag in blog.Tags)
            {
               

                MySqlParameter[] parameters = new MySqlParameter[] 
                {
                    new MySqlParameter("@cid",cid),
                    new MySqlParameter("@mid",tag)
                };

                insertTagResult = mariaDbHelper.ExecuteSql(sql, parameters);
            }

            MySqlParameter[] catetoryParameters = new MySqlParameter[]
            {
                new MySqlParameter("@cid",cid),
                new MySqlParameter("@mid",blog.Category)
            };

            insertCatetoryResult = mariaDbHelper.ExecuteSql(sql, catetoryParameters);

            return insertTagResult > 0 & insertCatetoryResult > 0;
        }

        private void ReplaceTagsWithmid(Blog blog)
        {
            var metas = GetTypechometa();
            var tagList = metas.Where(x => x.type == "tag");

            for(int i = 0;i<blog.Tags.Count();i++)
            {
                var findTagResult = tagList.FirstOrDefault(x => x.name == blog.Tags[i]);
                if (findTagResult != null)
                {
                    blog.Tags[i] = findTagResult.mid.ToString();
                    IncrementMetaCount(findTagResult.mid);
                }
                else
                {
                    blog.Tags[i] = InsertMeta(blog.Tags[i], blog.Tags[i], "tag");
                }
            }
        }

        private void IncrementMetaCount(int mid)
        {
            var sql = $"UPDATE typecho_metas SET `count` = `count` + 1 WHERE MID = {mid}" ;
            mariaDbHelper.ExecuteSql(sql);
        }

        private void ReplaceCatetoryWithmid(Blog blog)
        {
            var metas = GetTypechometa();
            var catetoryList = metas.Where(x => x.type == "category");

            var findCatetoryResult = catetoryList.FirstOrDefault(x => x.name == blog.Category);

            if(findCatetoryResult != null)
            {
                blog.Category = findCatetoryResult.mid.ToString();
                IncrementMetaCount(findCatetoryResult.mid);
            }
            else
            {
                blog.Category = InsertMeta(blog.Category, blog.Category, "category");
            }
        }

        private string InsertMeta(string name,string slug,string type)
        {
            var sql = "INSERT INTO typecho_metas " +
                        "(name, slug, `type`, description, count, `order`, parent) VALUES " +
                        "(@name,@slug,@type,@description,@count,@order,@parent)";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                        new MySqlParameter("@name",name),
                        new MySqlParameter("@slug",slug),
                        new MySqlParameter("@type",type),
                        new MySqlParameter("@description",null),
                        new MySqlParameter("@count",MySqlDbType.Int32){ Value = 1},
                        new MySqlParameter("@order",MySqlDbType.Int32){ Value = 0},
                        new MySqlParameter("@parent",MySqlDbType.Int32){ Value = 0}
            };

            var result = mariaDbHelper.ExecuteSql(sql, parameters);

            if(result>0)
            {
                sql = "select mid from typecho_metas order by mid desc";
                return mariaDbHelper.QueryFirst(sql).ToString();
            }

            return "";
        }
    }
}
