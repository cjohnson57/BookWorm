using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Data.SqlClient;
using BookWormSite.Models;

namespace BookWormSite.Classes
{
    //Source:
    // https://www.codeproject.com/Articles/1228738/Basic-Setup-of-Fluent-NHibernate

    class NHibernateHelper
    {
        private static readonly ISessionFactory _sessionFactory;

        private static IConfiguration config;

        static NHibernateHelper()
        {
            _sessionFactory = FluentConfigure();
        }
        public static ISession GetCurrentSession()
        {
            return _sessionFactory.OpenSession();
        }
        public static void CloseSession()
        {
            _sessionFactory.Close();
        }
        public static void CloseSessionFactory()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Close();
            }
        }

        public static ISessionFactory FluentConfigure()
        {
            return Fluently.Configure()
                //which database
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(Startup.StaticConfig.GetValue<string>("ConnectionString")))
                //2nd level cache
                .Cache(c => c.UseQueryCache().UseSecondLevelCache().ProviderClass<NHibernate.Cache.HashtableCacheProvider>())
                //find/set the mappings
                //.Mappings(m => m.FluentMappings.AddFromAssemblyOf<CustomerMapping>())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .BuildSessionFactory();
        }
    }
}
