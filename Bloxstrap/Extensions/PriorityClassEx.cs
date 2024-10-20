namespace Bloxstrap.Extensions
{
    static class PriorityClassEx
    {
        public static ProcessPriorityClass ToProcessPriorityClass(this PriorityClasses value) => value switch
        {
            PriorityClasses.Idle => ProcessPriorityClass.Idle,
            PriorityClasses.BelowNormal => ProcessPriorityClass.BelowNormal,
            PriorityClasses.Normal => ProcessPriorityClass.Normal,
            PriorityClasses.AboveNormal => ProcessPriorityClass.AboveNormal,
            PriorityClasses.High => ProcessPriorityClass.High,
            PriorityClasses.RealTime => ProcessPriorityClass.RealTime,
            _ => ProcessPriorityClass.Normal
        };
    }
}
