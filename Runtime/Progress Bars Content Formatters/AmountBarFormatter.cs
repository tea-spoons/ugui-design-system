
namespace TeaSpoons.UGuiDesignSystem
{
    public class AmountBarFormatter : ProgressBarFormatter
    {
        protected override void UpdateContent(int value)
        {
            controller.Content.Label = $"{value} / {controller.MaxValue}";
        }
    }
}
