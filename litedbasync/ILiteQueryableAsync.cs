using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace litedbasync
{
    public interface ILiteQueryableAsync<T> : ILiteQueryableAsyncResult<T>
    {
        ILiteQueryableAsync<T> Include(BsonExpression path);
        ILiteQueryableAsync<T> Include(List<BsonExpression> paths);
        ILiteQueryableAsync<T> Include<K>(Expression<Func<T, K>> path);

        ILiteQueryableAsync<T> Where(BsonExpression predicate);
        ILiteQueryableAsync<T> Where(string predicate, BsonDocument parameters);
        ILiteQueryableAsync<T> Where(string predicate, params BsonValue[] args);
        ILiteQueryableAsync<T> Where(Expression<Func<T, bool>> predicate);

        ILiteQueryableAsync<T> OrderBy(BsonExpression keySelector, int order = 1);
        ILiteQueryableAsync<T> OrderBy<K>(Expression<Func<T, K>> keySelector, int order = 1);
        ILiteQueryableAsync<T> OrderByDescending(BsonExpression keySelector);
        ILiteQueryableAsync<T> OrderByDescending<K>(Expression<Func<T, K>> keySelector);

        ILiteQueryableAsync<T> GroupBy(BsonExpression keySelector);
        ILiteQueryableAsync<T> Having(BsonExpression predicate);

        ILiteQueryableAsyncResult<BsonDocument> Select(BsonExpression selector);
        ILiteQueryableAsyncResult<K> Select<K>(Expression<Func<T, K>> selector);
    }
}
