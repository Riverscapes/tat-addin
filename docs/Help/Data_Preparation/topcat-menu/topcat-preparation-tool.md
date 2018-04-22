---
title: ToPCAT Preparation Tool
weight: 1
---

Currently in order to use ToPCAT to decimate a point cloud the input point cloud file needs to be in a space separated `*.xyz` file. To address this need, `ToPCAT Prep` takes in a raw point cloud file and replaces its separator with spaces so that the file can be used in ToPCAT.

In addition to space separated files, files greater than 300 MB can occasionally fail to be processed by ToPCAT due to their large size. If you have a file larger than this or have run ToPCAT and it does not work you may want to consider subsetting your raw point cloud file with the *Subset Raw Point Cloud Tool*.

When the `ToPCAT Prep` - Convert Point Cloud to Space Delimited button is clicked this form is raised::

![GCD6_Form_ToPCAT_Prep]({{ site.baseurl }}/assets/images/GCD6_Form_ToPCAT_Prep.png)

------

## `ToPCAT Prep`

#### Inputs:

- **Raw point cloud**
  - **formatted into columns of x, y, z.**
- **Point Cloud separator** (optional):
  - the symbol used to separate the x, y, z values. The default is comma `,`.

#### Outputs:

- **ToPCAT ready raw point cloud**
  - raw point cloud formatted into space delimited columns of x y z.

------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help/Data_Preparation/topcat-menu/"><i class="fa fa-arrow-circle-up"></i> Back Up to ToPCAT Menu</a> 
	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl }}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>