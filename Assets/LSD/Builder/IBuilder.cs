namespace LSD.Builder
{
    public interface IBuilder<TImpl>
    {
        TImpl Build();
        IBuilder<TImpl> FromNew();
        IBuilder<TImpl> Override<TDependency, TIn>(TDependency dependency);
    }
}