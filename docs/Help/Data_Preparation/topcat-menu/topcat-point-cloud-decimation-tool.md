---
title: ToPCAT Point Cloud Decimation Tool
weight: 2
---

The `ToPCAT point cloud decimation` tool takes in a space delimited x y z file (i.e. a `*.pts`), x and y sample window resolutions, and a minimum number of points to calculate a statistics. By gridding the points based on the user defined x and y resolutions it calculates valuable statistics for each sample window with at least the user input.minimum number of points.  These statistics are stored in a single point in the center of each sample window.

![GCD6_Form_ToPCAT_Decimation_Tab1]({{ site.baseurl }}/assets/images/GCD6_Form_ToPCAT_Decimation_Tab1.png)

![GCD6_Form_ToPCAT_Decimation_Tab2]({{ site.baseurl }}/assets/images/GCD6_Form_ToPCAT_Decimation_Tab2.png)

<!---
### Tutorial

For a demonstration of this tool (run in [MBES Tools](http://mbes.joewheaton.org/)) and an explanation of why or why not to create the two outputs see the video below (note: video from GCD 6, but operation is same in TAT):

<iframe width="560" height="315" src="https://www.youtube.com/embed/OyVlYZJmtIg" frameborder="0" gesture="media" allow="encrypted-media" allowfullscreen></iframe>
--->

------

## `ToPCAT Decimation`

#### Inputs:

- **ToPCAT ready raw point cloud**
  - raw point cloud formatted into space delimited columns of x y z.
- **X and Y sample window dimensions**
  - Dimensions in units of the raw survey file to define the size of the sample window used to calculate statistics from. (Note this typically is the same resolution that you wish to create a raster for, but not always)
- **Minimum number of points in sample window to calculate statistics**
  - To calculate many of its output statistics multiple points are needed within a sample window. The default minimum number is `4` points.

#### Outputs:

- **Z stats shapefile**
  - Point shapefile containing the statistics calculated for each sample window. The representative point is in the center of the sample window.
- **Z minimum shapfile**
  - Point shapefile containing the x,y location and z-value of the minimum value in the sample windows.
- **Z maximum shapefile**
  - Point Shapefile containing the x,y location and z-value of the maximum value in the sample windows.
- **Underpopulated Z stats shapefile**
  - Point shapefile with points at the cell centers of sample windows that did not have enough points to calculate all of the statistics calculated for its sample window. Still contains some statistics it was able to calculate.

------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help/Data_Preparation/topcat-menu/"><i class="fa fa-arrow-circle-up"></i> Back Up to ToPCAT Menu</a> 
	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl }}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>