---
title: Uncertainty Analysis Submenu
weight: 1
---

There are three sub-submenus under Uncertainty Analysis: These include [FIS Development Tools]({{ site.baseurl }}/Help/Analysis/uncertainty-analysis-submenu/fis-development-assistant.html), [Point Cloud Based](), and  [Raster Based]().

![TAT_Toolbar_Analysis_Uncertainty]({{ site.baseurl }}\assets\images\TAT_Toolbar_Analysis_Uncertainty.png) 

These commands are intended to help you explore various ways of either developing FIS Error models tailored to your data, and/or to indepently estimate elevation uncertainty from point clouds or rasters. These tools were developed by James Hensleigh as part of the [MBES Tools project for Idaho Power](http://mbes.joewheaton.org), and have undergone some testing. They have not undergone extensive testing in the TAT environment and running from within ArcGIS, and are still missing some basic features the user might expect (e.g. add to map automatically). As such, they are currently made available in a BETA form. These tools have a variety of ArcGIS ArcPy, Spatial Analyst, 3D Analyst, and R depencies. 

#### Uncertainty Analysis Commands

- `FIS Development Tools`
  - [BETA `FIS Development Assistant`]({{ site.baseurl }}/Help/Analysis/uncertainty-analysis-submenu/fis-development-assistant.html)
- [`Point Cloud Based`]({{ site.baseurl }}/Help/Analysis/uncertainty-analysis-submenu/point-cloud-based/)
  - [`Coincident Points Tool`]({{ site.baseurl }}/gcd-command-reference/gcd-analysis-menu/a-uncertainty-analysis-submenu/b-point-cloud-based/i-coincident-points-tool)
- [`Raster Based`]({{ site.baseurl }}/Help/Analysis/uncertainty-analysis-submenu/raster-ba/)
  - [`Create Interpolation Error Surface`]({{ site.baseurl }}/Help/Analysis/uncertainty-analysis-submenu/raster-ba/create-interpolation-error-surface.html)

------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help"><i class="fa fa-chevron-circle-left"></i>  Back to TAT Help </a>  

	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl}}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>