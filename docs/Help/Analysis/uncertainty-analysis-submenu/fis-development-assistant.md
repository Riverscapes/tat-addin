---
title: FIS Development Assistant
weight: 2
---
## Accessing Tool

The BETA `FIS Development Tools` sub menu is located under the `Analysis` --> `Uncertainty Analysis` sub menu: 

![GCD6_Menu_Analysis_Uncertainty_FIS_FISAssist (1)]({{ site.baseurl }}/assets/images/TAT_Toolbar_Analysis_FIS_Assit.png) 

If you attempt to run the tool without R installed, you will not be able to and get the following message:
![TAT_FIS_No_R_Warning]({{ site.baseurl }}/assets/images/TAT_FIS_No_R_Warning.png)

## Description
The `FIS Development Tools`, currently only includes the BETA `FIS Development Assistant` and *may* (subject to funding) eventually include an `FIS Development Wizard`. The `FIS Development Assistant` is an R-script (only runs if R installed), which uses an algorithm developed by Hensleigh (2014) to derive a fuzzy inference system empricially. It requires the user to define the inputs they wish to use for the FIS, provide an independent estimate of elevation uncertainty that corresponds to those inputs, and then it:
- Derives *suggested* input membership functions from the distribution of input values provided
- Derives *suggested* output membership functions based on distribution of independently estiamted output values (e.g. elevation uncertainty)
- Looks for occcurances of all possible combinations of inputs, and creates a preliminary rule table (i.e. inference system), based on what output category those combinations of input predominantly fall in. 
- Outputs the above as a draft `*.fis` file, which is compatible with Matlab's fuzzy logic toolbox and GCD.

It should be noted that the above is useful for developing an intitiall FIS, but it should always be carefully inspected and calibrated to make sure it works as intended. Common problems with this approach include:
- Arbitrarily precise boundaries for membership functions (makes sense to round them to reasonable values)
- Range of empirial values in inputs and outputs don't account for full possible range of values (similarily number of categories may need to be increased)
- Plausible and possible combinations of inputs do not occur in input training datasets. 

These are straight-forward to adjust for manually. 

### References 

- Hensleigh J. (2014). [Geomorphic Change Detection Using Multi-Beam Sonar](https://digitalcommons.usu.edu/gradreports/376/). Masters Thesis. Utah State University, Logan, Utah. 
- 
------
<div align="center">
	<a class="hollow button" href="{{ site.baseurl }}/Help"><i class="fa fa-chevron-circle-left"></i>  Back to TAT Help </a>  

	<a class="hollow button" href="{{ site.baseurl }}/"><img src="{{ site.baseurl}}/assets/images/Tatty.png">  Back to TAT Home </a>  
</div>