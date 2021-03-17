﻿using OpenTabletDriver.Plugin.Attributes;
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

                    float rand_new_x = (float)multiplier.NextDouble();
                    float rand_deviation_x = rand_new_x * (Randomizer_dev_max - Randomizer_dev_min) + Randomizer_dev_min;
                    float rand_limited_x = rand_old + rand_deviation_x;
                    float rand_clamp_x = Math.Clamp(MathF.Abs(rand_limited_x), Minimum_area_multiplier_x, 1);
                    rand_old_x = rand_clamp_x;

                    float rand_new_y = (float)multiplier.NextDouble();
                    float rand_deviation_y = rand_new_y * (Randomizer_dev_max - Randomizer_dev_min) + Randomizer_dev_min;
                    float rand_limited_y = rand_old + rand_deviation_y;
                    float rand_clamp_y = Math.Clamp(MathF.Abs(rand_limited_y), Minimum_area_multiplier_x, 1);
                    rand_old_y = rand_clamp_y;

                    float timer_rand = (float)multiplier.NextDouble();
                    timer = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

                    return new Vector2(
                        input.X *= rand_clamp_x,
                        input.Y *= rand_clamp_y
                    );
                }
                else
                {
                    start = DateTime.Now;

                    float rand_new = (float)multiplier.NextDouble();
                    float rand_deviation = rand_new * (Randomizer_dev_max - Randomizer_dev_min) + Randomizer_dev_min;
                    float rand_limited = rand_old + rand_deviation;
                    float rand_clamp = Math.Clamp(MathF.Abs(rand_limited), Minimum_area_multiplier, 1);
                    rand_old = rand_clamp;

                    float timer_rand = (float)multiplier.NextDouble();
                    timer = timer_rand * (Time_interval_max - Time_interval_min) + Time_interval_min;

                    return new Vector2(
                        input.X *= rand_clamp,
                        input.Y *= rand_clamp
                    );
                }
            }
            else
            {
                if (Split_xy)
                {
                    return new Vector2(
                    input.X *= rand_old_x,
                    input.Y *= rand_old_y
                    );
                }
                else
                {
                    return new Vector2(
                    input.X *= rand_old,
                    input.Y *= rand_old
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

        [Property("Timer Interval Min ms")]
        public float Time_interval_min { set; get; }

        [Property("Timer Interval Max ms")]
        public float Time_interval_max { set; get; }

        [Property("Minumum Area Multiplier")]
        public float Minimum_area_multiplier { set; get; }

        [Property("Randomizer Deviation Min")]
        public float Randomizer_dev_min { set; get; }

        [Property("Randomizer Deviation Max")]
        public float Randomizer_dev_max { set; get; }

        [BooleanProperty("Enable Split X/Y Multipliers", "")]
        public bool Split_xy { set; get; }

        [Property("Minumum Area Multiplier X")]
        public float Minimum_area_multiplier_x { set; get; }

        [Property("Minumum Area Multiplier Y")]
        public float Minimum_area_multiplier_y { set; get; }
    }
}