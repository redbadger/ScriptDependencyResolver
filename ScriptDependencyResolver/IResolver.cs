namespace ScriptDependencyResolver
{
    using System;
    using System.Collections.Generic;

    public interface IResolver
    {
        IEnumerable<string> Resolve(Func<string, bool> predicate);
    }
}