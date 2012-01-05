namespace RedBadger.Web.ScriptDependencyResolver
{
    using System;
    using System.Collections.Generic;

    public interface IResolver
    {
        IEnumerable<string> Resolve(Func<string, bool> filterPredicate);
    }
}