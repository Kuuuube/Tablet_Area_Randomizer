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

        public static float SignlessClamp(float input, float min, float max)
        {
            int sign = input > 0 ? 1 : -1;
            return Math.Clamp(Math.Abs(input), Math.Abs(min), Math.Abs(max)) * sign;
        }

        public Vector2 Randomizer(Vector2 input)
        {
            if (randomizerStopwatch.Elapsed.TotalMilliseconds >= timer_interval_rand)
            {
                randomizerStopwatch.Restart();

                Vector2 randomizer_deviation = new Vector2(randomizer_deviation_min, randomizer_deviation_max);

                Vector2 Minimum_area_multiplier = split_xy ? new Vector2(minimum_area_multiplier_x, minimum_area_multiplier_y) : new Vector2(minimum_area_multiplier_xy, minimum_area_multiplier_xy);

                Vector2 rands_xy = new Vector2((float)multiplier.NextDouble(), (float)multiplier.NextDouble());
                Vector2 rand_xy = split_xy ? new Vector2(rands_xy.X, rands_xy.Y) : new Vector2(rands_xy.X, rands_xy.X);

                Vector2 rand_range = new Vector2(SignlessClamp(rand_xy.X * (randomizer_deviation.Y * 2) - randomizer_deviation.Y, randomizer_deviation.X, randomizer_deviation.Y), SignlessClamp(rand_xy.Y * (randomizer_deviation.Y * 2) - randomizer_deviation.Y, randomizer_deviation.X, randomizer_deviation.Y));
                Vector2 rand_clamp = new Vector2(Math.Clamp(rand_old.X + rand_range.X, Minimum_area_multiplier.X, 1), Math.Clamp(rand_old.Y + rand_range.Y, Minimum_area_multiplier.Y, 1));

                rand_old = new Vector2(rand_clamp.X, rand_clamp.Y);

                timer_interval_rand = (float)multiplier.NextDouble() * (time_interval_max - time_interval_min) + time_interval_min;
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
        public float time_interval_min { set; get; }

        [Property("Timer Interval Max"), Unit("ms"), DefaultPropertyValue(1000f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Timer Interval Max: The maximum time before the next area change.")]
        public float time_interval_max { set; get; }

        [Property("Minumum Area Size"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Size: The smallest percent of the current area that can be generated.\n" +
            "This option is not used when Enable Split Width/Height is enabled.")]
        public float Minimum_area_multiplier_raw
        {
            set
            {
                minimum_area_multiplier_xy = Math.Clamp(value, 0, 100) / 100;
            }
            get => minimum_area_multiplier_xy;
        }
        public float minimum_area_multiplier_xy;

        [Property("Randomizer Deviation Min"), Unit("%"), DefaultPropertyValue(5f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Randomizer Deviation Min: The minimum percent change for the next generated area.")]
        public float Randomizer_dev_min_raw
        {
            set
            {
                randomizer_deviation_min = Math.Clamp(value, 0, 100) / 100;
            }
            get => randomizer_deviation_min;
        }
        public float randomizer_deviation_min;

        [Property("Randomizer Deviation Max"), Unit("%"), DefaultPropertyValue(10f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Randomizer Deviation Max: The maximum percent change for the next generated area.")]
        public float Randomizer_dev_max_raw
        {
            set
            {
                randomizer_deviation_max = Math.Clamp(value, Randomizer_dev_min_raw + 0.0000001f, 100) / 100;
            }
            get => randomizer_deviation_max;
        }
        public float randomizer_deviation_max;

        [BooleanProperty("Enable Split Width/Height", ""), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Enable Split Width/Height: Allows for width and height to be generated separately. This will result in randomized area aspect ratios.")]
        public bool split_xy { set; get; }

        [Property("Minumum Area Multiplier Width"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Multiplier Width: The smallest percent of the current area's width that can be generated.\n" +
            "This option is only used when Enable Split Width/Height is enabled.")]
        public float minimum_area_multiplier_x_raw
        {
            set
            {
                minimum_area_multiplier_x = Math.Clamp(value, 0, 100) / 100;
            }
            get => minimum_area_multiplier_x;
        }
        public float minimum_area_multiplier_x;

        [Property("Minumum Area Multiplier Height"), Unit("%"), DefaultPropertyValue(50f), ToolTip
            ("Tablet Area Randomizer:\n\n" +
            "Minumum Area Multiplier Height: The smallest percent of the current area's height that can be generated.\n" +
            "This option is only used when Enable Split Width/Height is enabled.")]
        public float minimum_area_multiplier_y_raw
        {
            set
            {
                minimum_area_multiplier_y = Math.Clamp(value, 0, 100) / 100;
            }
            get => minimum_area_multiplier_y;
        }
        public float minimum_area_multiplier_y;
    }
}