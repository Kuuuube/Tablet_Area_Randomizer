# Tablet Area Randomizer Plugin for [OpenTabletDriver](https://github.com/OpenTabletDriver/OpenTabletDriver)

## Explanation of the Values:

<br>

**Timer Interval Min:** The minimum time before the next area change.

**Timer Interval Max:** The maximum time before the next area change.

**Maximum Area Multiplier:** This is automatically set to your current area and cannot be changed within the plugin. It will always equal 100% to avoid mapping issues.

**Minumum Area Multiplier:** The smallest multiplier that will be used on the area. This option is not used when **Split X/Y Multipliers** is enabled.

**Randomizer Deviation Min:** The minimum amount of change in the next generated area.

**Randomizer Deviation Max:** The maximum amount of change in the next generated area.

**Enable Split X/Y Multipliers:** Allows for differing X and Y multipliers. This will result in randomized area aspect ratios.

**Minumum Area Multiplier X:** The smallest multiplier that will be used on the area's X coordinate. This option is only used when **Split X/Y Multipliers** is enabled.

**Minumum Area Multiplier Y:** The smallest multiplier that will be used on the area's Y coordinate. This option is only used when **Split X/Y Multipliers** is enabled.

<br>

## Example Settings:
<p align="middle">
  <img src="https://raw.githubusercontent.com/Kuuuube/Tablet_Area_Randomizer/main/example_settings.png" align="middle"/>
</p>

These settings randomize at a rate between 100ms and 1000ms. The smallest area that can be generated with them is 50% of the current area size and it will change the area size between 5% and 10% every generation.