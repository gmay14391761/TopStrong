using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Easy4net.Session
{
    public class SessionFactory
    {
        public static Session GetSession()
        {
            Session session = SessionThreadLocal.Get();
            if (session == null)
            {
                session = Session.PriviteInstance();
                SessionThreadLocal.Set(session);
            }

            return session;
        }
    }
}
