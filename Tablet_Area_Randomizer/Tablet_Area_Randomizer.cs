using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin;
using System;
using System.Numerics;

namespace Tablet_Area_Randomizer
{
    [PluginName("Tablet Area Randomizer")]
    public class Tablet_Area_Randomizer : IFilter
    {
        DateTime start = DateTime.Now;

        private static readonly Random multiplier = new Random();
        float rand_old = 1;
        float rand_old_x = 1;
        float rand_old_y = 1;
        float timer;

        public Vector2 Randomizer(Vector2 input)
        {

            if (DateTime.Now.Subtract(start).TotalMilliseconds >= timer)
            {
                if (Split_xy)
                {
                    start = DateTime.Now;

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
                    timer = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

                    return new Vector2(
                        input.X *= rand_expanded_x,
                        input.Y *= rand_expanded_y
                    );
                }
                else
                {
                    start = DateTime.Now;

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
                    timer = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

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

        protected static Vector2 ToUnit(Vector2 input)
        {
            if (Info.Driver.OutputMode is AbsoluteOutputMode absoluteOutputMode)
            {
                var area = absoluteOutputMode.Input;
                var size = new Vector2(area.Width, area.Height);
                var half = size / 2;
                var display = (Info.Driver.OutputMode as AbsoluteOutputMode)?.Output;
                var offset = (Vector2)((Info.Driver.OutputMode as AbsoluteOutputMode)?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                var pxpermmw = display.Width / area.Width;
                var pxpermmh = display.Height / area.Height;
                return new Vector2(
                    ((input.X - shiftoffX) / pxpermmw - half.X) / half.X,
                    ((input.Y - shiftoffY) / pxpermmh - half.Y) / half.Y
                    );
            }
            else
            {
                return default;
            }
        }

        protected static Vector2 FromUnit(Vector2 input)
        {
            if (Info.Driver.OutputMode is AbsoluteOutputMode absoluteOutputMode)
            {
                var area = absoluteOutputMode.Input;
                var size = new Vector2(area.Width, area.Height);
                var half = size / 2;
                var display = (Info.Driver.OutputMode as AbsoluteOutputMode)?.Output;
                var offset = (Vector2)((Info.Driver.OutputMode as AbsoluteOutputMode)?.Output?.Position);
                var shiftoffX = offset.X - (display.Width / 2);
                var shiftoffY = offset.Y - (display.Height / 2);
                var pxpermmw = display.Width / area.Width;
                var pxpermmh = display.Height / area.Height;
                return new Vector2(
                    ((input.X * half.X) + half.X) * pxpermmw + shiftoffX,
                    ((input.Y * half.Y) + half.Y) * pxpermmh + shiftoffY
                );
            }
            else
            {
                return default;
            }
        }

        protected static Vector2 Clamp(Vector2 input)
        {
            return new Vector2(
            Math.Clamp(input.X, -1, 1),
            Math.Clamp(input.Y, -1, 1)
            );
        }

        public Vector2 Filter(Vector2 input) => FromUnit(Clamp(Randomizer(ToUnit(input))));

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