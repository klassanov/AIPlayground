using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIPlugins.Plugins
{
    public class DateTimePlugin
    {
        //Best practice: use snake case for function names
        //Shall be registered
        [KernelFunction("get_current_date_time")]
        [Description("Get the current date and time.")]
        public string GetCurrentDateTime() => DateTime.Now.ToString();

        [KernelFunction("get_current_day_of_week")]
        [Description("Get the current day of the week.")]
        public string GetCurrentDayOfWeek() => DateTime.Now.DayOfWeek.ToString();
    }
}
