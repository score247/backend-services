using DbUp.Engine;
using DbUp.Engine.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBUp.Deployment.ScriptProvider
{
    public class ScriptProvider : IScriptProvider
    {
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            throw new NotImplementedException();
        }
    }
}
