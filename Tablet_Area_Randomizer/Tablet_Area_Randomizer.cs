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
    public class Tablet_Area_Randomizer : IFilter
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

                    float timer_rand_raw = (float)multiplier.NextDouble();
                    timer_interval_rand = timer_rand_raw * (Time_interval_max - Time_interval_min) + Time_interval_min;

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

                    float timer_rand_raw = (float)multiplier.NextDouble();
                    timer_interval_rand = timer_rand_raw * (Time_interval_max - Time_interval_min) + Time_interval_min;

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

        public Vector2 Filter(Vector2 input) => FromCenter(Clamp(Randomizer(ToCenter(input))));

        public FilterStage FilterStage => FilterStage.PostTranspose;

        [Property("Timer Interval Min"), Unit("ms")]
        public float Time_interval_min { set; get; }

        [Property("Timer Interval Max"), Unit("ms")]
        public float Time_interval_max { set; get; }

        [SliderProperty("Minumum Area Multiplier", 0, 100), Unit("%")]
        public float Minimum_area_multiplier_raw { set; get; }

        [SliderProperty("Randomizer Deviation Min", 0, 100), Unit("%")]
        public float Randomizer_dev_min_raw { set; get; }

        [SliderProperty("Randomizer Deviation Max", 0, 100), Unit("%")]
        public float Randomizer_dev_max_raw { set; get; }

        [BooleanProperty("Enable Split X/Y Multipliers", "")]
        public bool Split_xy { set; get; }

        [SliderProperty("Minumum Area Multiplier X", 0, 100), Unit("%")]
        public float Minimum_area_multiplier_x_raw { set; get; }

        [SliderProperty("Minumum Area Multiplier Y", 0, 100), Unit("%")]
        public float Minimum_area_multiplier_y_raw { set; get; }

    }
}