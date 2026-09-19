
namespace TeaSpoons.UGuiDesignSystem
{
    public class PercentageBarFormatter : ProgressBarFormatter
    {
        protected override void UpdateContent(int value)
        {
            controller.Content.Label = (value * 100 / controller.MaxValue) + "%";
        }
    }
}
