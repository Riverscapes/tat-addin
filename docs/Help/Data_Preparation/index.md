---
title: Data Preparation Menu
weight: 1
---

The Data Preparation menu consists of two sets of tools. The first focus on [Survey Preparation]({{ sites.baseurl }}/Help/Data_Preparation/survey-preparation-menu/) of topographic surveys:

![TAT_Toolbar_SurveyPrep]({{ sites.baseurl }}/assets/images/TAT_Toolbar_SurveyPrep.png)

This includes: 
- [ Create Point Feature Class <i class="fa fa-cog"></i>]({{ sites.baseurl }}/Help/Data_Preparation/survey-preparation-menu/create-point-feature-class.html)  Creates a shapefile from a point cloud file (e.g. `*.pts`, `*.xyz`, etc.) 
- [Create Survey Extent Polygon <i class="fa fa-cog"></i>]({{ sites.baseurl }}/Help/Data_Preparation/survey-preparation-menu/create-survey-extent-polygon.html) Creates a bounding polygon around a survey point feature class
- [Create DEM and/or TIN <i class="fa fa-cog"></i>]({{ sites.baseurl }}/Help/Data_Preparation/survey-preparation-menu/create-tin-and-or-dem.html) Creates a TIN and derives a DEM from that TIN

------
The second set of tools simply allow a user to run [ToPCAT](http://mbes.joewheaton.org/background/conceptual-reference-pages/topcat) to process large point clouds within a GIS environment and visualize the outputs in GIS.

![TAT_Toolbar_ToPCAT]({{ sites.baseurl }}/assets/images/TAT_Toolbar_ToPCAT.png)

This includes: 
- [ ToPCAT Preparation <i class="fa fa-cog"></i>]({{ sites.baseurl }}/Help/Data_Preparation/topcat-menu/topcat-preparation-tool.html)  Ensures that your input point cloud file is in a format that ToPCAT can read 
- [ToPCAT Point Cloud Decimation <i class="fa fa-cog"></i>]({{ sites.baseurl }}/Help/Data_Preparation/topcat-menu/topcat-point-cloud-decimation-tool.html) Runs ToPCAT on a point cloud


------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl }}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>