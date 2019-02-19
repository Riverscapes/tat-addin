#################################################################################################################
## MBES Tools - Point Cloud Shapefile to TIN and/or DEM
## James Hensleigh - Hensleigh.ride@gmail.com
##
## Converts a point cloud shape file (x,y,z, etc.) to a TIN and/or DEM
## 
## INPUTS: Point Cloud Shapefile, Extent Polygon, Spatial Reference
## OUTPUTS: Triangular Irregular Network (TIN) and/or a Digital Elevation Model (DEM)
## 
#################################################################################################################
#!/usr/bin/env python
import arcpy, sys, os, shutil, argparse

#Check to if 3D analyst extension is available
if arcpy.CheckExtension('3D') == 'Available':
    arcpy.CheckOutExtension('3D')
    arcpy.AddMessage('\n3D Analyst extension checked out.')
else:
    arcpy.AddMessage('\n3D Analyst extension not available.')
    sys.exit()

#Parse the input variables to a parsing object and provide basic descriptions for --help tag
parser = argparse.ArgumentParser()
parser.add_argument('OutputTIN',
                    help = 'This is the output location of the TIN file.',
                    type = str)

parser.add_argument('SpatialReference',
                    help = 'The .prj file or .shp file containing the spaital reference to be used for the TIN.',
                    type = str)

parser.add_argument('InputPointCloud',
                    help = 'The input point cloud to be used to create the TIN',
                    type = str)

parser.add_argument('InputExtentPolygon',
                    help = 'The input extent polygon to be used to set the boundaries of the TIN',
                    type = str)

parser.add_argument('InputZField',
                    help = 'The field to create the TIN and DEM from.',
                    type = str)

parser.add_argument('--OutputDEM',
                    help = 'The path with extention to create a DEM (optional).',
                    type = str)

parser.add_argument('--CellSize',
                    help = 'The cell size to use when creating the DEM.',
                    type = str)

parser.add_argument('--DeleteTIN',
                    help = 'This is an option to delete TIN or not after creating DEM.',
                    type = bool)

args = parser.parse_args()

try:

    #Check to see if the folder containing the TIN already exists, if so exit the script.
    if os.path.exists(os.path.split(args.OutputTIN)[0] + '\\' + os.path.split(args.OutputTIN)[1].lower()):
        arcpy.AddMessage(str(args.OutputTIN) + ' already exists.')
        arcpy.CheckInExtension('3D')
        sys.exit()    
        
    else:
        arcpy.AddMessage('Creating TIN....')

        '''Create TIN with user inputs. However user is not given option for following parameters:
        Interpolation method: defaults to NATURAL NEIGHBORS
        Triangulation: defaults to DELAUNAY
        **This is for simplicity reason for IPC techs but can be changed upon request
        '''
        arcpy.CreateTin_3d(args.OutputTIN,
                           args.SpatialReference,
                           [[args.InputPointCloud, args.InputZField, 'Mass_Points', '<None>'], [args.InputExtentPolygon, '<None>', 'Hard_Clip', '<None>']])
        

        arcpy.AddMessage('Your TIN ' + str(args.OutputTIN) + ' has been successfully created.')

        #If the user checked the create DEM then create DEM, if user said to delete the TIN then delete TIN
        if args.OutputDEM:
            
            if os.path.exists(args.OutputDEM):
                arcpy.AddMessage(args.OutputDEM + ' already exists.')
                arcpy.CheckInExtension('3D')
                sys.exit()
            else:    
                arcpy.AddMessage('Creating DEM...')
                outDEM = args.OutputDEM
                cellSize = args.CellSize
                arcpy.TinRaster_3d(args.OutputTIN, args.OutputDEM, 'FLOAT', 'NATURAL_NEIGHBORS', 'CELLSIZE ' + args.CellSize)
                arcpy.AddMessage('Your DEM ' + args.OutputDEM + ' was successfully created.\n')
                if args.DeleteTIN:
                    #The folder to contain the TIN automatically changes the users inputs to all lowercase letters, the below makes
                    #the input argument the same and removes the folder
                    shutil.rmtree(os.path.split(args.OutputTIN)[0] + '\\' + os.path.split(args.OutputTIN)[1].lower())
                    arcpy.AddMessage('The input tin ' + args.OutputTIN + ' was removed.')
       
except:
    arcpy.CheckInExtension('3D')
    arcpy.AddMessage(sys.exc_info()[1])
    
finally:
    arcpy.CheckInExtension('3D')
    
