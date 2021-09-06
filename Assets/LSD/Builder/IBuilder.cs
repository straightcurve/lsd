using System;

namespace LSD.Builder
{
    public interface IBuilder<TImpl>
    {
        TImpl Build();
        IBuilder<TImpl> New();
        IBuilder<TImpl> Clone<TOriginal>(TOriginal instance) where TOriginal : TImpl, ICloneable;
        IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency);
    }
}