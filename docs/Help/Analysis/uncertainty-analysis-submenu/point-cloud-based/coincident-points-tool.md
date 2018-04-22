---
title: Coincident Points Tool
weight: 1
---
### What it does
The `Coincident Points Tool` takes a raw survey x,y,z file as input and searches it for occurrences when the same x,y location is surveyed multiple times. The tool outputs a shapefile that contains the x,y locations, the two different z measurements  and the difference between the z values. The difference between the z values can be used as an estimation for uncertainty at theses x, y locations At these locations raster values, such as slope and surface roughness, can be extracted and used to model the relationship between these external parameters and point uncertainty. 

For conceptual background about how these coincident points can be used to model uncertainty [click here](http://mbes.joewheaton.org/background/conceptual-reference-pages/uncertainty-analysis) or for a tutorial on how to get started on uncertainty analysis with the output from this tool [click here](http://mbes.joewheaton.org/background/tutorial-how-to-pages/other-tricks-analyses-tutorials/coincident-points-analysis).

### Accessing the Tool
The `Coincident Points Tool` is located under the `Analysis` -> `Uncertainty Analysis` menu under the `Point Cloud Based` sub menu:

![AddInToolbar_CoincidentPointsTool]({{ site.baseurl }}/assets/images/TAT_Toolbar_Analysis_PointCloudBased.png)

### Using the Tool
When the `Coincident Points Tool` is run, the following dialog appears:

![CoincidentPointsToolForm]({{ site.baseurl }}/assets/images/TAT_CoincidentPointTools.png)

The video below describes how to use the `Coincident Points Tool`:

<iframe width="560" height="315" src="https://www.youtube.com/embed/HrAq5HHlZVY" frameborder="0" gesture="media" allow="encrypted-media" allowfullscreen></iframe>

#### INPUTS:

The inputs for this tool are:

- **Raw Point Cloud** 
  - An ascii text file formatted into columns of  x, y, z. Other columns can be present but will not be included in the output shapefile. (headers can be commented out with a `#`)
- **Set Precision**
  - By setting this check box the user can select a matching point precision to use other than an exact match of points.
  - If a precision other than `0` is set then a point does not have to be an exact match to be considered a coincident point. This is mainly used when using highly precise points. For example, if , a `1`, `2` or `3` was specified as the **precision**  an (x,y) pair of points of (1.0001, 1.0000) and (1.0002, 1.0001) would be considered *coincident*, but  if the precision was set to `4` they wold not be coicicdent. 
- **Separator**   (optional)
  - The symbol used to separate the x, y, z values. The default is comma
- **Spatial Reference** (optional)
  - Can be in the form of a `*.prj` file or you can load an existing shapefile that contains a spatial reference and that spatial reference will be imported.

**OUTPUTS:**

The outputs for the tool are:
- **Shapefile or Feature Class**
  - Contains fields of x, y coordinates for the location, and the two different z fields, and a field that contains the difference between the two z values. Note, output is repeated for every coincident pair found.


------
<div align="center">
​	
	<a class="hollow button" href="{{ site.baseurl }}/Help"><i class="fa fa-chevron-circle-left"></i>  Back to TAT Help </a>  

	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl}}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>