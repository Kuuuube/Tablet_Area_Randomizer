# Tablet Area Randomizer Plugin for [OpenTabletDriver](https://github.com/OpenTabletDriver/OpenTabletDriver) [![](https://img.shields.io/github/downloads/Kuuuube/Tablet_Area_Randomizer/total.svg)](https://github.com/Kuuuube/Tablet_Area_Randomizer/releases/latest)

Randomizes tablet area within given parameters.

## Explanation of the Values:

<br>

**Timer Interval Min:** The minimum time before the next area change.

**Timer Interval Max:** The maximum time before the next area change.

**Maximum Area Size:** This is automatically set to your current area and cannot be changed within the plugin. It will always equal 100% to avoid mapping issues.

**Minumum Area Size:** The smallest percent of the current area that can be generated. This option is not used when **Enable Split Width/Height** is enabled.

**Randomizer Deviation Min:** The minimum percent change for the next generated area.

**Randomizer Deviation Max:** The maximum percent change for the next generated area.

**Enable Split Width/Height:** Allows for width and height to be generated separately. This will result in randomized area aspect ratios.

**Minumum Area Width:** The smallest percent of the current area's width that can be generated. This option is only used when **Enable Split Width/Height** is enabled.

**Minumum Area Height:** The smallest percent of the current area's height that can be generated. This option is only used when **Enable Split Width/Height** is enabled.

<br>

## Example Settings:
<p align="middle">
  <img src="https://raw.githubusercontent.com/Kuuuube/Tablet_Area_Randomizer/main/example_settings.png" align="middle"/>
</p>

These settings randomize at a rate between 100ms and 1000ms. The smallest area that can be generated with them is 50% of the current area size and the area size will differ between 5% and 10% every new generation.
