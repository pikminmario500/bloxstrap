namespace Bloxstrap.Extensions
{
    static class PriorityClassEx
    {
        public static ProcessPriorityClass ToProcessPriorityClass(this PriorityClasses value) => value switch
        {
            PriorityClasses.Idle => ProcessPriorityClass.Idle,
            PriorityClasses.BelowNormal => ProcessPriorityClass.BelowNormal,
            PriorityClasses.Normal => ProcessPriorityClass.AboveNormal,
            PriorityClasses.AboveNormal => ProcessPriorityClass.Normal,
            PriorityClasses.High => ProcessPriorityClass.High,
            PriorityClasses.RealTime => ProcessPriorityClass.RealTime,
            _ => ProcessPriorityClass.Normal
        };
    }
}
