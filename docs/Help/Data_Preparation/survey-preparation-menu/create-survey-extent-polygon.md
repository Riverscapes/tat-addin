---
title: Create Survey Extent Polygon
weight: 2
---

The` Create Survey Extent Polygon` tool (or `Survey Mask Creator`) takes a raw xyz point cloud in a flat ascii file (e.g. `*.csv`, `*txt`,`*.xyz`, `*.pts`) and converts it into a polygon shapefile for direct use in deriving a TIN and DEM. The polygon shapefile will be used in constricting the area that is interpolated across when creating a TIN and/or DEM through the [`Create DEM and/or TIN`]({{ site.baseurl }}/Help/Data_Preparation/survey-preparation-menu/create-tin-and-or-dem.html). It also allows you to specify the spatial reference (coordinate system) of the polygon shapefile. The tool is a precursor to [`Create DEM and/or TIN`]({{ site.baseurl }}/Help/Data_Preparation/survey-preparation-menu/create-tin-and-or-dem.html) tool.

![CreateSurveyExtentPolygonToolUI]({{ site.baseurl }}/assets/images/CreateSurveyExtentPolygonToolUI.png)

The  video gives a succinct tutorial on how to use and the details of the `Survey Mask Creator` (note video is from GCD 6, but functionality is same in TAT):

<iframe width="560" height="315" src="https://www.youtube.com/embed/O6MR16S2mBc" frameborder="0" gesture="media" allow="encrypted-media" allowfullscreen></iframe>


## `Survey Mask Creator`

#### INPUTS:


- **Raw Point Cloud**
  - An ascii text file formatted into columns of x, y, z. Other columns can be present but will not be included in the output shapefile. (headers can be commented out with a `#`)
  - *File must be space delimited*. If it is not space delimited use the [TopCat Prep](http://mbes.joewheaton.org/background/mbes-tools-command-reference/data-preparation/topcat-prep) tool to make the file space delimited. 

- **Spatial Reference**(optional)
  - can be in the form of a`* .prj` file or you can load an existing shapefile that contains a spatial reference and that spatial reference will be imported.

- **Sample Window Size**
  - defines the window size (cell resoluton) that will be used to create the survey grid. Every sample window that contains one point within it will be included in the final survey extent polygon.
  - *The sample window size should match the grid size that will be used in the subsequent GCD analysis*.

#### OUTPUTS:

The outputs for the Create Survey Extent Polygon tool are:

- **Polygon Shapefile or Feature Class**
  - *represents the area where a point was surveyed for each individual sample window based on the grid created by the input sample size window*


# KNOWN LIMITATIONS
Warning: This command only works with older versions of ArcGIS (pre 10.2) given its ArCPy Depencies. See [this post](https://github.com/Riverscapes/tat-addin/issues/9).


------

<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help/Data_Preparation/survey-preparation-menu/"><i class="fa fa-arrow-circle-up"></i> Back Up to Data Preparation Menu</a> 
	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl }}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>
