using System;

namespace LSD.Builder
{
    public interface IBuilder<TImpl>
    {
        TImpl Build();
        IBuilder<TImpl> FromNew();
        IBuilder<TImpl> Clone<TOriginal>(TOriginal instance) where TOriginal : TImpl, ICloneable;
        IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency);
    }
}