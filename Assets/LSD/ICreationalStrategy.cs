using System;

namespace LSD
{
    public interface ICreationalStrategy
    {
        object Create(Type type);
    }
}