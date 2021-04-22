using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin;
using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Timing;

namespace Tablet_Area_Randomizer
{
    [PluginName("Tablet Area Randomizer")]
    public class Tablet_Area_Randomizer : IPositionedPipelineElement<IDeviceReport>
    {
        protected HPETDeltaStopwatch randomizerStopwatch = new HPETDeltaStopwatch(true);
        private readonly Random multiplier = new Random();
        float rand_old = 1;
        float rand_old_x = 1;
        float rand_old_y = 1;
        float timer_interval_rand;

        public Vector2 Randomizer(Vector2 input)
        {
            if (randomizerStopwatch.Elapsed.TotalMilliseconds >= timer_interval_rand)
            {
                if (Split_xy)
                {
                    randomizerStopwatch.Restart();

                    float Randomizer_dev_min = Randomizer_dev_min_raw / 100;
                    float Randomizer_dev_max = Randomizer_dev_max_raw / 100;
                    float Minimum_area_multiplier_x = Minimum_area_multiplier_x_raw / 100;
                    float Minimum_area_multiplier_y = Minimum_area_multiplier_y_raw / 100;

                    float rand_new_x = (float)multiplier.NextDouble();
                    float rand_range_x = rand_new_x * (Randomizer_dev_max - (-Randomizer_dev_max)) + (-Randomizer_dev_max);
                    float rand_deviation_x = Math.Clamp(MathF.Abs(rand_range_x), Randomizer_dev_min, Randomizer_dev_max) * (MathF.Abs(rand_range_x) / rand_range_x);
                    float rand_limited_x = rand_old_y + rand_deviation_x;
                    float rand_clamp_x = Math.Clamp(MathF.Abs(rand_limited_x), Minimum_area_multiplier_x, 1);
                    float rand_expanded_x = 1 / rand_clamp_x;
                    rand_old_x = rand_clamp_x;

                    float rand_new_y = (float)multiplier.NextDouble();
                    float rand_range_y = rand_new_y * (Randomizer_dev_max - (-Randomizer_dev_max)) + (-Randomizer_dev_max);
                    float rand_deviation_y = Math.Clamp(MathF.Abs(rand_range_y), Randomizer_dev_min, Randomizer_dev_max) * (MathF.Abs(rand_range_y) / rand_range_y);
                    float rand_limited_y = rand_old_y + rand_deviation_y;
                    float rand_clamp_y = Math.Clamp(MathF.Abs(rand_limited_y), Minimum_area_multiplier_y, 1);
                    float rand_expanded_y = 1 / rand_clamp_y;
                    rand_old_y = rand_clamp_y;

                    float timer_rand = (float)multiplier.NextDouble();
                    timer_interval_rand = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

                    return new Vector2(
                        input.X *= rand_expanded_x,
                        input.Y *= rand_expanded_y
                    );
                }
                else
                {
                    randomizerStopwatch.Restart();

                    float Randomizer_dev_min = Randomizer_dev_min_raw / 100;
                    float Randomizer_dev_max = Randomizer_dev_max_raw / 100;
                    float Minimum_area_multiplier = Minimum_area_multiplier_raw / 100;

                    float rand_new = (float)multiplier.NextDouble();
                    float rand_range = rand_new * (Randomizer_dev_max - (-Randomizer_dev_max)) + (-Randomizer_dev_max);
                    float rand_deviation = Math.Clamp(MathF.Abs(rand_range), Randomizer_dev_min, Randomizer_dev_max) * (MathF.Abs(rand_range) / rand_range);
                    float rand_limited = rand_old + rand_deviation;
                    float rand_clamp = Math.Clamp(MathF.Abs(rand_limited), Minimum_area_multiplier, 1);
                    float rand_expanded = 1 / rand_clamp;
                    rand_old = rand_clamp;

                    float timer_rand = (float)multiplier.NextDouble();
                    timer_interval_rand = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

                    return new Vector2(
                        input.X *= rand_expanded,
                        input.Y *= rand_expanded
                    );
                }
            }
            else
            {
                if (Split_xy)
                {
                    return new Vector2(
                    input.X *= 1 / rand_old_x,
                    input.Y *= 1 / rand_old_y
                    );
                }
                else
                {
                    return new Vector2(
                    input.X *= 1 / rand_old,
                    input.Y *= 1 / rand_old
                    );
                }
            }
        }

        protected static Vector2 ToCenter(Vector2 input)
        {
            if (Info.Driver.OutputMode is AbsoluteOutputMode absoluteOutputMode)
            {
                var display = (Info.Driver.OutputMode as AbsoluteOutputMode)?.Output;
                var offset = (Vector2)((Info.Driver.OutputMode as AbsoluteOutputMode)?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                return new Vector2(
                    input.X - shiftoffX,
                    input.Y - shiftoffY
                    );
            }
            else
            {
                return default;
            }
        }

        protected static Vector2 FromCenter(Vector2 input)
        {
            if (Info.Driver.OutputMode is AbsoluteOutputMode absoluteOutputMode)
            {
                var display = (Info.Driver.OutputMode as AbsoluteOutputMode)?.Output;
                var offset = (Vector2)((Info.Driver.OutputMode as AbsoluteOutputMode)?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                return new Vector2(
                    input.X + shiftoffX,
                    input.Y + shiftoffY
                );
            }
            else
            {
                return default;
            }
        }

        protected static Vector2 Clamp(Vector2 input)
        {
            var display = (Info.Driver.OutputMode as AbsoluteOutputMode)?.Output;
            return new Vector2(
            Math.Clamp(input.X, -display.Width, display.Width),
            Math.Clamp(input.Y, -display.Height, display.Height)
            );
        }

        public event Action<IDeviceReport> Emit;

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                report.Position = Filter(report.Position);
                value = report;
            }

            Emit?.Invoke(value);
        }

        public Vector2 Filter(Vector2 input) => FromCenter(Clamp(Randomizer(ToCenter(input))));

        public PipelinePosition Position => PipelinePosition.PostTransform;

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