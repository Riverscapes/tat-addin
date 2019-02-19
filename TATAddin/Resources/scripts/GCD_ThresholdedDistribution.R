# GCD Thresholded Change Distribution Plot v.1

# Script Information:
# Last updated: 2/15/2013
# Created by: Sara Bangen (sara.bangen@gmail.com)
# This script was written for GCD users who have a basic working knowledge
# of R and desire greater user control over DoD distribution plots produced 
# from the GCD output. 
# Note: Plots are produced using the ggplot2 package.
# 
# Input: raw.csv 
#
# Output: Area & Volumetric Elevation Change Distribution Plots
#
# User MUST define:  - Import Data filepath 
#                    - Plot y-axis limits (e.g. ylim=c(0, 500))
#                    - Plot output filepath 

# Call on packages needed for this script
library(Hmisc)
library(plotrix)
library(reshape2)
library(ggplot2)
library(grid)

# Import Data
data.raw = read.csv("C:\\YOURGCDFOLDER\\raw.csv", header=T)
data.thresh = read.csv("C:\\YOURGCDFOLDER\\thresholded.csv", header=T)

# Shorten data.thresh data frame column header names
colnames(data.thresh) = c("LowerElevRange.m", "UpperElevRange.m", "ThreshTotalArea.m2", "ThreshTotalVol.m3", "CellCount")

# Add raw total area and raw total volume to data.dodhtresh data frame 
data.thresh$RawTotalArea.m2 = data.raw[, c("Total.Area..m2.")]
data.thresh$RawTotalVol.m3 = data.raw[, c("Total.Volume..m3.")]

# Calculate area and volume of changes due to DEM noise
data.thresh$NoiseArea.m2 = data.thresh$RawTotalArea.m2 - data.thresh$ThreshTotalArea.m2
data.thresh$NoiseVol.m3 = data.thresh$RawTotalVol.m3 - data.thresh$ThreshTotalVol.m3


# Create new column for Volume of Erosion using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data.thresh$ThreshVolumeEros.m3 = ifelse(data.thresh$LowerElevRange.m<0,data.thresh$ThreshTotalVol.m3,0)

# Create new column for Volume of Deposition using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data.thresh$ThreshVolumeDep.m3 = ifelse(data.thresh$LowerElevRange.m>=0,data.thresh$ThreshTotalVol.m3,0)

# Create new column for Area of Erosion using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data.thresh$ThreshAreaEros.m2 = ifelse(data.thresh$LowerElevRange.m<0,data.thresh$ThreshTotalArea.m2,0)

# Create new column for Area of Deposition using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data.thresh$ThreshAreaDep.m2 = ifelse(data.thresh$LowerElevRange.m>=0,data.thresh$ThreshTotalArea.m2,0)


#View the data frame
View(data.thresh)
names(data.thresh)

# Create new dataframe (data.threshvol)  
# copy only the subset of the data.dod dataframe is required for the plot into the new dataframe
data.threshvol = data.thresh[, c("LowerElevRange.m", "ThreshVolumeEros.m3","ThreshVolumeDep.m3","NoiseVol.m3")]

#View the new volume data frame
View(data.threshvol)

# Create new dataframe (data.dodvol)  
# copy only the subset of the data.dod dataframe is required for the plot into the new dataframe
data.thresharea = data.thresh[, c("LowerElevRange.m", "ThreshAreaEros.m2","ThreshAreaDep.m2","NoiseArea.m2")]

#View the new area data frame
View(data.thresharea)

#################################################
# Create AREAL Elevation Change Distribution Plot
################################################

# Note: y-axis limits set on 9th line: "coord_cartesian(ylim=c(0, INSERT UPPERLIMIT HERE))"

m.data.thresharea = melt(data.thresharea, id.vars=1)
View(m.data.thresharea)
p1 = ggplot(m.data.thresharea, aes(LowerElevRange.m, value, fill=variable))+
  geom_bar(stat="identity", size=.05) +
  scale_fill_manual(values=c("#C00000", "#1B3F8B", "grey")) +
  xlab("Elevation Change (m)") + ylab(expression(paste("Area"," ",(m^2))))+
  ggtitle("ENTER MAIN PLOT TITLE HERE") +
  theme_bw() + 
  coord_cartesian(ylim=c(0, 1400)) + 
  theme(axis.title.x = element_text(size=14), axis.text.x = element_text(colour = "#404040", size=12)) +
  theme(axis.title.y = element_text(size=14), axis.text.y = element_text(colour = "#404040",size=12)) +
  theme(legend.position="none") +
  theme(plot.margin = unit(c(.25,.25,.25,.25), "lines")) +
  theme(plot.title = element_text(size=20))
# View the plot
p1

#######################################################
# Create VOLUMETRIC Elevation Change Distribution Plot
#######################################################

# Note: y-axis limits set on 9th line: "coord_cartesian(ylim=c(0, INSERT UPPERLIMIT HERE))"

m.data.threshvol = melt(data.threshvol, id.vars=1)
View(m.data.threshvol)
p2 = ggplot(m.data.threshvol, aes(LowerElevRange.m, value, fill=variable))+
    geom_bar(stat="identity", size=.05) +
    scale_fill_manual(values=c("#C00000", "#1B3F8B", "grey")) +
    xlab("Elevation Change (m)") + ylab(expression(paste("Volume"," ",(m^3))))+
    ggtitle("ENTER MAIN PLOT TITLE HERE") +
    theme_bw() + 
    coord_cartesian(ylim=c(0, 35)) + 
    theme(axis.title.x = element_text(size=14), axis.text.x = element_text(colour = "#404040", size=12)) +
    theme(axis.title.y = element_text(size=14), axis.text.y = element_text(colour = "#404040",size=12)) +
    theme(legend.position="none") +
    theme(plot.margin = unit(c(.25,.25,.25,.25), "lines")) +
    theme(plot.title = element_text(size=20))
# View the plot
p2

#####################################
# Export Plots as *.pdf
#####################################

# Below is code for exporting individual plots as well as code
# for exporting both plots on a single page

# Note: PDF plot size (width=x, height=x) is in inches

# AREA PLOT ON A SINGLE PAGE #

pdf( "C:\\YOURGCDFOLDER\\AreaPlot.pdf", paper="special", width=8.5, height=5.5, useDingbats=FALSE)
vp.setup <- function(x,y){
  # create a new layout with grid
  grid.newpage()
  # define viewports and assign it to grid layout
  pushViewport(viewport(layout = grid.layout(x,y)))}
  vp.layout <- function(x,y){
  viewport(layout.pos.row=x, layout.pos.col=y)}
  vp.setup(1,1)
  print(p1, vp=vp.layout(1, 1))
dev.off()

# VOLUME PLOT ON A SINGLE PAGE #

pdf( "C:\\YOURGCDFOLDER\\VolumePlot.pdf", paper="special", width=8.5, height=5.5, useDingbats=FALSE)
vp.setup <- function(x,y){
  # create a new layout with grid
  grid.newpage()
  # define viewports and assign it to grid layout
  pushViewport(viewport(layout = grid.layout(x,y)))}
  vp.layout <- function(x,y){
  viewport(layout.pos.row=x, layout.pos.col=y)}
  vp.setup(1,1)
  print(p2, vp=vp.layout(1, 1))
dev.off()

# BOTH PLOTS ON A SINGLE PAGE #

pdf( "C:\\YOURGCDFOLDER\\AreaVolumePlots.pdf", paper="special", width=8.5, height=11, useDingbats=FALSE)
vp.setup <- function(x,y){
  # create a new layout with grid
  grid.newpage()
  # define viewports and assign it to grid layout
  pushViewport(viewport(layout = grid.layout(x,y)))}
  vp.layout <- function(x,y){
  viewport(layout.pos.row=x, layout.pos.col=y)}
  vp.setup(2,1)
  print(p1, vp=vp.layout(1, 1))
  print(p2, vp=vp.layout(2, 1))
dev.off()



