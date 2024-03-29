---
title: Create Point Feature Class
weight: 1
---

The `Create Point Feature Class` tool takes a raw xyz point cloud in a flat ascii file (e.g. `*.csv`, `*.txt`, `*.xyz`, `*.pts`) and converts it into a point shapefile for direct use in deriving a TIN and DEM. It also allows you to specify the spatial reference (coordinate system) of the point shapefile you will create, which is required to use the shapefile in a GCD project analyses (e.g. point density estimation). The tool is a precursor to `Create DEM and/or TIN` tool.

![CreatePointFeatureClassToolUI]({{ site.baseurl }}/assets/images/TAT_RawPointCloud2Shapefile.png)

The  video gives a succinct tutorial on how to use and the details of the Create Point Feature Class Tool (note the video shows accessing the tool from GCD 6, but functionality is same in TAT):

<!---
<iframe width="560" height="315" src="https://www.youtube.com/embed/5e6e9j4v5Fc" frameborder="0" gesture="media" allow="encrypted-media" allowfullscreen></iframe> --->

## Raw Point Cloud to Shapefile 

#### INPUTS:

- **Raw Point Cloud**
  - An ascii text file formatted into columns of x, y, z. Other columns can be present but will not be included in the output shapefile. (headers can be commented out with a `#`)
- **Spatial Reference** (optional)
  - can be in the form of a .prj file or you can load an existing shapefile that contains a spatial reference and that spatial reference will be imported.

#### OUTPUTS:

The outputs for the tool are:

- **Shapefile or Feature Class**
  - contains x, y, z fields from the original surveyed points.


------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help/Data_Preparation/survey-preparation-menu/"><i class="fa fa-arrow-circle-up"></i> Back Up to Data Preparation Menu</a> 
	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl }}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>