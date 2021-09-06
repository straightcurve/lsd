using System;
using System.Collections.Generic;

namespace LSD
{
    public interface ICreationalStrategy
    {
        object Create(Type type, IEnumerable<Override> overrides = null);
        TImpl Create<TImpl>(IEnumerable<Override> overrides = null);

        object CreateRecursively(Type type, IEnumerable<Override> overrides = null);
        TImpl CreateRecursively<TImpl>(IEnumerable<Override> overrides = null);
    }
}