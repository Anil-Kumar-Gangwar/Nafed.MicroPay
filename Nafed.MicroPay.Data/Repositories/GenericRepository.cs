using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Nafed.MicroPay.Data.Models;
using System.Linq.Expressions;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Data.Entity.Validation;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;


namespace Nafed.MicroPay.Data.Repositories
{
    public class GenericRepository : IGenericRepository
    {
        internal MicroPayEntities context;
        //   internal DbSet<TEntity> dbSet;

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GenericRepository(MicroPayEntities context)
        {
            this.context = context;
            // this.dbSet = context.Set<TEntity>();
        }
        public GenericRepository()
        {
            this.context = new MicroPayEntities();
        }

        public virtual IEnumerable<TEntity> Get<TEntity>(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "") where TEntity : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.AsNoTracking().Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IEnumerable<TEntity> GetPagedResult<TEntity>(int pageNum, int pageSize, out int rowsCount,
            Expression<Func<TEntity, bool>> filter = null,
             string sortOn = "", bool isAscendingOrder = false, string includeProperties = "") where TEntity : class
        {
            IEnumerable<TEntity> pagedResult;
            rowsCount = 0;
            if (pageSize <= 0) pageSize = 20;
            IQueryable<TEntity> query = context.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            rowsCount = query.Count();
            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;
            int excludedRows = (pageNum - 1) * pageSize;

            if (!string.IsNullOrEmpty(sortOn))
            {
                var param = Expression.Parameter(typeof(TEntity), "i");

                MemberExpression property = null;
                string[] fieldNames = sortOn.Split('.');
                foreach (string filed in fieldNames)
                {
                    if (property == null)
                    {
                        property = Expression.Property(param, filed);
                    }
                    else
                    {
                        property = Expression.Property(property, filed);
                    }
                }

                Expression conversion = Expression.Convert(property, typeof(object));//Expression.Property(param, fieldName)
                var mySortExpression = Expression.Lambda<Func<TEntity, object>>(conversion, param).Compile();
                pagedResult = isAscendingOrder ? query.OrderBy(mySortExpression).ToList() : query.OrderByDescending(mySortExpression).ToList();
            }
            else
            {
                pagedResult = query.ToList();
            }
            return pagedResult.Skip(excludedRows).Take(pageSize);
        }

        public virtual TEntity GetByID<TEntity>(object id) where TEntity : class
        {
            return context.Set<TEntity>().Find(id);
        }

        public virtual bool Exists<TEntity>(Expression<Func<TEntity, bool>> filter = null) where TEntity : class
        {
            if (filter == null) return false;
            return (context.Set<TEntity>().AsNoTracking().Any(filter));
        }

        public virtual int Insert<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
                int ret = 0;
                PropertyInfo key = typeof(TEntity).GetProperties().FirstOrDefault(p =>
    p.CustomAttributes.Any(attr => attr.AttributeType == typeof(KeyAttribute)));
                if (key != null)
                {
                    ret = (int)key.GetValue(entity, null);
                }
                return ret;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        
        public virtual bool AddMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
        {
            var flag = false;
            if (entityList == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                context.Set<TEntity>().AddRange(entityList);
                context.SaveChanges();
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        public virtual bool AddMultiple<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class
        {
            var flag = false;
            if (entityList == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                foreach (var item in entityList)
                {
                    context.Entry(item).State = EntityState.Added;
                }
                context.ChangeTracker.DetectChanges();
                context.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        public virtual bool RemoveMultipleEntity<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class
        {
            var flag = false;
            if (removeEntityList == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                context.Set<TEntity>().RemoveRange(removeEntityList);
                context.SaveChanges();
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        public virtual bool DeleteAll<TEntity>(IEnumerable<TEntity> removeEntityList) where TEntity : class
        {
            var flag = false;
            if (removeEntityList == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                foreach (var item in removeEntityList)
                {
                    context.Entry(item).State = EntityState.Deleted;
                }
                context.ChangeTracker.DetectChanges();
                context.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        public virtual void Delete<TEntity>(object id) where TEntity : class
        {
            TEntity entityToDelete = context.Set<TEntity>().Find(id);
            Delete(entityToDelete);
            context.SaveChanges();
        }
          
        public virtual void Delete<TEntity>(TEntity entityToDelete) where TEntity : class
        {
            if (entityToDelete == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {  
                context.Set<TEntity>().Attach(entityToDelete);
                context.Set<TEntity>().Remove(entityToDelete);
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public virtual IEnumerable<T> ExecWithStoreProcedure<T>(string query, params object[] parameters)
        {
            return context.Database.SqlQuery<T>(query, parameters);
        }
        public virtual void ExecuteWithStoreProcedure(string query, params object[] parameters)
        {
            context.Database.ExecuteSqlCommand(query, parameters);
        }
        public virtual void Update<TEntity>(TEntity entityToUpdate) where TEntity : class
        {
            if (entityToUpdate == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                //context.Entry(entityToUpdate).State = EntityState.Modified;
                //context.SaveChanges();

               // context.Configuration.AutoDetectChangesEnabled = false;
                context.Entry(entityToUpdate).State = EntityState.Modified;
              //  context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                context.Entry(entityToUpdate).State = EntityState.Detached;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }
        public virtual DbParameter GetParameter(string paramName, System.Data.DbType dbtype, object value = null, bool bOutput = false)
        {
            var param = new SqlParameter();
            param.ParameterName = paramName;
            if (value != null)
            { param.Value = value; }
            param.DbType = dbtype;
            if (bOutput)
            { param.Direction = System.Data.ParameterDirection.Output; }
            return param;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DataTable GetTablesSchema(string sTableName)
        {
            DataTable dbSqlDataSet = new DataTable();
            try
            {
                SqlCommand dbSqlCommand;
                SqlDataAdapter dbSqlAdapter;
                SqlConnection dbSqlconnection;
                dbSqlconnection = new SqlConnection(context.Database.Connection.ConnectionString);
                dbSqlCommand = new SqlCommand();
                dbSqlCommand.Connection = dbSqlconnection;
                dbSqlCommand.CommandType = CommandType.StoredProcedure;
                dbSqlCommand.CommandText = "[GetSchemaInformation]"; //=== Change ur procedure name
                dbSqlCommand.Parameters.Add("@tableNameList", SqlDbType.VarChar).Value = sTableName;
                dbSqlAdapter = new SqlDataAdapter(dbSqlCommand);
                if (dbSqlconnection.State == ConnectionState.Closed)
                    dbSqlconnection.Open();
                dbSqlAdapter.Fill(dbSqlDataSet);
                dbSqlconnection.Close();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dbSqlDataSet;
        }

        //public virtual IEnumerable<AnonymousSelectList> GetCombo<TEntity>(string query, params object[] parameters) where TEntity : class
        //{
        //    var results = context.SqlQuery<TEntity>(query, parameters);
        //    var sss = results.ToList();
        //    return  new List<AnonymousSelectList>();
        //}


        public virtual IQueryable<TEntity> GetIQueryable<TEntity>(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "") where TEntity : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }
    }

}
