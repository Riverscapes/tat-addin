# GCD Simple Minimum LoD Change Distribution Plot R Script v.1

# Script Information:
# Last updated: 2/15/2013
# Created by: Sara Bangen (sara.bangen@gmail.com)
# This script was written for GCD users who have a basic working knowledge
# of R and desire greater user control over DoD distribution plots produced 
# from the GCD output. 
# Plots are produced using the ggplot2 package.
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
data = read.csv("C:\\YOURGCDFOLDER\\raw.csv", header=T)

# Shorten column header names
colnames(data) = c("LowerElevRange.m", "UpperElevRange.m", "TotalArea.m2", "TotalVol.m3", "CellCount")

# Create new column for Volume of Erosion using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data$VolumeEros.m3 = ifelse(data$LowerElevRange.m<0,data$TotalVol.m3,0)

# Create new column for Volume of Deposition using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data$VolumeDep.m3 = ifelse(data$LowerElevRange.m>=0,data$TotalVol.m3,0)

# Create new column for Area of Erosion using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data$AreaEros.m2 = ifelse(data$LowerElevRange.m<0,data$TotalArea.m2,0)

# Create new column for Area of Deposition using ifelse statement
# syntax: ifelse(test, value if true, value if false)
data$AreaDep.m2 = ifelse(data$LowerElevRange.m>=0,data$TotalArea.m2,0)

#View the data frame
View(data)

# Create new dataframe (data.dodvol)  
# copy only the subset of the data.dod dataframe is required for the barplot into the new dataframe
data.vol = data[, c("LowerElevRange.m", "VolumeEros.m3","VolumeDep.m3")]

#View the new volume data frame
View(data.vol)

# Create new dataframe (data.dodvol)  
# copy only the subset of the data.dod dataframe is required for the barplot into the new dataframe
data.area = data[, c("LowerElevRange.m", "AreaEros.m2","AreaDep.m2")]

#View the new area data frame
View(data.area)

#################################################
# Create AREAL Elevation Change Distribution Plot
################################################

# Note: y-axis limits set on 9th line: "coord_cartesian(ylim=c(0, INSERT UPPERLIMIT HERE))"

m.data.area = melt(data.area, id.vars=1)
View(m.data.area)
p1 = ggplot(m.data.area, aes(LowerElevRange.m, value, fill=variable))+
  geom_bar(stat="identity", size=.05) +
  scale_fill_manual(values=c("#C00000", "#1B3F8B")) +
  xlab("Elevation Change (m)") + ylab(expression(paste("Area"," ",(m^2))))+
  ggtitle("ENTER MAIN PLOT TITLE HERE") +
  theme_bw() + 
  coord_cartesian(ylim=c(0, 1400)) + 
  theme(axis.title.x = element_text(size=14), axis.text.x = element_text(colour = "#404040", size=12)) +
  theme(axis.title.y = element_text(size=14), axis.text.y = element_text(colour = "#404040",size=12)) +
  theme(legend.position="none") +
  theme(plot.margin = unit(c(.25,.25,.25,.25), "lines")) +
  theme(plot.title = element_text(size=20))
p1

#######################################################
# Create VOLUMETRIC Elevation Change Distribution Plot
#######################################################

# Note: y-axis limits set on 9th line: "coord_cartesian(ylim=c(0, INSERT UPPERLIMIT HERE))"

m.data.vol = melt(data.vol, id.vars=1)
View(m.data.vol)
p2 = ggplot(m.data.vol, aes(LowerElevRange.m, value, fill=variable))+
    geom_bar(stat="identity", size=.05) +
    scale_fill_manual(values=c("#C00000", "#1B3F8B")) +
    xlab("Elevation Change (m)") + ylab(expression(paste("Volume"," ",(m^3))))+
    ggtitle("ENTER MAIN PLOT TITLE HERE") +
    theme_bw() + 
    coord_cartesian(ylim=c(0, 35)) + 
    theme(axis.title.x = element_text(size=14), axis.text.x = element_text(colour = "#404040", size=12)) +
    theme(axis.title.y = element_text(size=14), axis.text.y = element_text(colour = "#404040",size=12)) +
    theme(legend.position="none") +
    theme(plot.margin = unit(c(.25,.25,.25,.25), "lines")) +
    theme(plot.title = element_text(size=20))
p2


#####################################
# Export Plots as *.pdf
#####################################

# Below is code for exporting individual plots as well as code
# for exporting both plots on a single page

# Note: PDF plot size (width=x, height=x) is in inches

# AREA PLOT #

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

# VOLUME #

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



