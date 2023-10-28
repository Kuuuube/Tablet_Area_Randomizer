using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Timing;

namespace Tablet_Area_Randomizer
{
    [PluginName("Tablet Area Randomizer")]
    public class Tablet_Area_Randomizer : Tablet_Area_Randomizer_Base
    {
        protected HPETDeltaStopwatch randomizerStopwatch = new HPETDeltaStopwatch(true);
        private readonly Random multiplier = new Random();
        Vector2 rand_old = new Vector2();
        float timer_interval_rand;

        public float SignlessClamp(float input, float min, float max)
        {
            int sign = input > 0 ? 1 : -1;
            return Math.Clamp(Math.Abs(input), Math.Abs(min), Math.Abs(max)) * sign;
        }

        public Vector2 Randomizer(Vector2 input)
        {
            if (randomizerStopwatch.Elapsed.TotalMilliseconds >= timer_interval_rand)
            {
                randomizerStopwatch.Restart();

                Vector2 Randomizer_dev_min_max = new Vector2(Randomizer_dev_min_raw / 100, Randomizer_dev_max_raw / 100);

                Vector2 Minimum_area_multiplier = Split_xy ? new Vector2(Minimum_area_multiplier_x_raw / 100, Minimum_area_multiplier_y_raw / 100) : new Vector2(Minimum_area_multiplier_raw / 100, Minimum_area_multiplier_raw / 100);

                Vector2 rands_xy = new Vector2((float)multiplier.NextDouble(), (float)multiplier.NextDouble());
                Vector2 rand_xy = Split_xy ? new Vector2(rands_xy.X, rands_xy.Y) : new Vector2(rands_xy.X, rands_xy.X);

                Vector2 rand_range = new Vector2(SignlessClamp(rand_xy.X * (Randomizer_dev_min_max.Y * 2) - Randomizer_dev_min_max.Y, Randomizer_dev_min_max.X, Randomizer_dev_min_max.Y), SignlessClamp(rand_xy.Y * (Randomizer_dev_min_max.Y * 2) - Randomizer_dev_min_max.Y, Randomizer_dev_min_max.X, Randomizer_dev_min_max.Y));
                Vector2 rand_clamp = new Vector2(Math.Clamp(rand_old.X + rand_range.X, Minimum_area_multiplier.X, 1), Math.Clamp(rand_old.Y + rand_range.Y, Minimum_area_multiplier.Y, 1));

                rand_old = new Vector2(rand_clamp.X, rand_clamp.Y);

                timer_interval_rand = (float)multiplier.NextDouble() * (Time_interval_max - Time_interval_min) + Time_interval_min;
            }
                
            return input *= rand_old;
        }

        public override event Action<IDeviceReport> Emit;

        public override void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                report.Position = Filter(report.Position);
                value = report;
            }

            Emit?.Invoke(value);
        }

        public Vector2 Filter(Vector2 input) => FromUnit(Clamp(Randomizer(ToUnit(input))));

        public override PipelinePosition Position => PipelinePosition.PostTransform;

        [Property("Timer Interval Min"), Unit("ms"), DefaultPropertyValue(100f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Timer Interval Min: The minimum time before the next area change.")]
        public float Time_interval_min { set; get; }

        [Property("Timer Interval Max"), Unit("ms"), DefaultPropertyValue(1000f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Timer Interval Max: The maximum time before the next area change.")]
        public float Time_interval_max { set; get; }

        [Property("Minumum Area Size"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Size: The smallest percent of the current area that can be generated.\n" +
            "This option is not used when Enable Split Width/Height is enabled.")]
        public float Minimum_area_multiplier_raw
        {
            set
            {
                Minimum_area_multiplier_raw_clamp = Math.Clamp(value, 0, 100);
            }
            get => Minimum_area_multiplier_raw_clamp;
        }
        public float Minimum_area_multiplier_raw_clamp;

        [Property("Randomizer Deviation Min"), Unit("%"), DefaultPropertyValue(5f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Randomizer Deviation Min: The minimum percent change for the next generated area.")]
        public float Randomizer_dev_min_raw
        {
            set
            {
                Randomizer_dev_min_raw_clamp = Math.Clamp(value, 0, 100);
            }
            get => Randomizer_dev_min_raw_clamp;
        }
        public float Randomizer_dev_min_raw_clamp;

        [Property("Randomizer Deviation Max"), Unit("%"), DefaultPropertyValue(10f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Randomizer Deviation Max: The maximum percent change for the next generated area.")]
        public float Randomizer_dev_max_raw
        {
            set
            {
                Randomizer_dev_max_raw_clamp = Math.Clamp(value, Randomizer_dev_min_raw + 0.0000001f, 100);
            }
            get => Randomizer_dev_max_raw_clamp;
        }
        public float Randomizer_dev_max_raw_clamp;

        [BooleanProperty("Enable Split Width/Height", ""), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Enable Split Width/Height: Allows for width and height to be generated separately. This will result in randomized area aspect ratios.")]
        public bool Split_xy { set; get; }

        [Property("Minumum Area Multiplier Width"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Multiplier Width: The smallest percent of the current area's width that can be generated.\n" +
            "This option is only used when Enable Split Width/Height is enabled.")]
        public float Minimum_area_multiplier_x_raw
        {
            set
            {
                Minimum_area_multiplier_x_raw_clamp = Math.Clamp(value, 0, 100);
            }
            get => Minimum_area_multiplier_x_raw_clamp;
        }
        public float Minimum_area_multiplier_x_raw_clamp;

        [Property("Minumum Area Multiplier Height"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Multiplier Height: The smallest percent of the current area's height that can be generated.\n" +
            "This option is only used when Enable Split Width/Height is enabled.")]
        public float Minimum_area_multiplier_y_raw
        {
            set
            {
                Minimum_area_multiplier_y_raw_clamp = Math.Clamp(value, 0, 100);
            }
            get => Minimum_area_multiplier_y_raw_clamp;
        }
        public float Minimum_area_multiplier_y_raw_clamp;
    }
}