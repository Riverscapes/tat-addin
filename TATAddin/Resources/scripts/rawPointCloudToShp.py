#################################################################################################################
## MBES Tools - Raw Survey File To Point Shapefile
## James Hensleigh - Hensleigh.ride@gmail.com
##
## Converts a raw survey file (x,y,z, etc.) to a point Shapefile
## 
## INPUTS: Raw survey file, spatial reference
## OUTPUTS: Point shapefile
## 
#################################################################################################################
#!/usr/bin/env python
import sys, os, argparse, arcpy

osgeoCheck = 0
try:
    from osgeo import ogr, osr
except:
    osgeoCheck = 1



####DEFINE CONSTANT VARIABLES TO BE USED IN SCRIPT

separatorDict = {'Comma' : ',',
                 'Period' : '.',
                 'Semi-Colon' : ';',
                 'Colon' : ':',
                 'Space' : ' '}

#Parse the input variables to a parsing object and provide basic descriptions for --help tag
parser = argparse.ArgumentParser()
parser.add_argument('OutputShapefile',
                    help = 'This is the output location of the point shapefile file.',
                    type = str)

parser.add_argument('rawPointCloud',
                    help = 'The input point cloud to be used to create the point shapefile',
                    type = str)


parser.add_argument('--Separator',
                    help = 'The separator used in the file',
                    default = ',',
                    type = str)


parser.add_argument('--SpatialReference',
                    help = 'The .prj file or .shp file containing the spaital reference to be used for the TIN.',
                    type = str)

args = parser.parse_args()



try:
    #Do this if the user has osgeo installed
    if osgeoCheck == 0:
        sys.stdout.write('\nUsing osgeo...')
        #FUNCTIONS TO DO WORK
        def shpDriver(outShp, DriverByName = 'ESRI Shapefile'):
            '''Provides support for outShpOGRUncert - Registers ogr Driver, create and output datasource'''
            shpDriver = ogr.GetDriverByName('ESRI Shapefile')
            if os.path.exists(outShp):
                shpDriver.DeleteDataSource(outShp)
            dS = shpDriver.CreateDataSource(outShp)
            if dS is None:
                print 'Could not open', dS
                sys.exit(1)
            else:
                return dS

        def createFields(line, lyr, separator):
            '''Helps in the creation of shapefiles from Decimated Point clouds because there are so many fields'''
            header = line.strip().split(separator)
            field = ogr.FieldDefn("Z", ogr.OFTReal)
            lyr.CreateField(field)
            return header

        def decimatedShpDerives(outDataName, inData, separator, spatialRef = None):
            '''Create shapefiles from the derivatives of the decimated point clouds'''
            dS = shpDriver(outDataName)
            lyr = dS.CreateLayer(str(os.path.splitext(outDataName)), spatialRef, ogr.wkbPoint25D)
            sys.stdout.write('\nCreating point shapefile template.....\n')
            with open(inData, 'r') as dataFile:
                #edit to z-enable points
                #header = createFields(dataFile.readline(), lyr, separator)
                lyrDef = lyr.GetLayerDefn()
                sys.stdout.write('\nInsert cursor created. Now populating shapefile.....\n')
                for line in dataFile:
                    lineValues = line.strip().split(separator)
                    geom = ogr.Geometry(ogr.wkbPoint25D)
                    #geom.AddPoint(float(lineValues[0]),float(lineValues[1]))
                    #edit to z-enable points
                    geom.AddPoint(float(lineValues[0]), float(lineValues[1]), float(lineValues[2]))
                    
                    row = ogr.Feature(lyrDef)
                    row.SetGeometry(geom)
                    lyr.CreateFeature(row)
                    #edit to z-enable points                    
                    #try:
                    #    row.SetField("Z", float(lineValues[2]))
                    #    lyr.CreateFeature(row)
                    #except:
                    #    pass
            dS.Destroy()

        try:
            if os.path.exists(args.OutputShapefile):
                sys.stdout.write('\n\n{0} already exists.\n'.format(args.OutputShapefile))
                sys.exit()

            if args.SpatialReference:
                #Set spatial reference
                spatialRef = osr.SpatialReference()
                spatialRef.SetFromUserInput('ESRI::' + args.SpatialReference)
                decimatedShpDerives(args.OutputShapefile, args.rawPointCloud, separatorDict[args.Separator][0], spatialRef)
            else:
                decimatedShpDerives(args.OutputShapefile, args.rawPointCloud, separatorDict[args.Separator][0])

            sys.stdout.write('\n\nYour file {0} was successfully converted to {1}\n'.format(args.rawPointCloud, args.OutputShapefile))
            
        except:
            sys.stdout.write('{0}'.format(sys.exc_info()[1]))

    #Do this is the user does not have osgeo
    elif osgeoCheck == 1:
        sys.stdout.write('\nUsing arcpy...')
        
        if os.path.exists(args.OutputShapefile):
            arcpy.AddMessage('\n' + args.OutputShapefile + ' already exists.')
            sys.exit()
        
        #FUNCTIONS TO DO WORK
        def createFields(line, lyr, separator):
            '''Helps in the creation of shapefiles from Decimated Point clouds because there are so many fields'''
            header = line.strip().split(separator)
            field = str("Z")#this may need to change as ESRI allows larger field names
            arcpy.AddField_management(lyr, field, 'DOUBLE')
            return header
        
        def decimatedShpDerives(outDataName, inData, separator, spatialRef = None):
            '''Create shapefiles from the derivatives of the decimated point clouds'''
            arcpy.CreateFeatureclass_management(os.path.split(outDataName)[0],
                                                os.path.split(outDataName)[1],
                                                'POINT',
                                                '', '', 'ENABLED',
                                                spatialRef)
            arcpy.AddMessage('\nCreating point shapefile template.....\n')
            with open(inData, 'r') as dataFile:
                #edit to allow z-enabled
                #header = createFields(dataFile.readline(), outDataName, separator)
                #header = [x.replace('-', '_')[:10] for x in header]#makes original header the same as field names
                #arcpy.DeleteField_management(outDataName, 'Id')#gets rid of automatically made Id field
                #edit to allow z-enabled
                #fields = ['SHAPE@X', 'SHAPE@Y', "Z"]
                fields = ['SHAPE@X', 'SHAPE@Y', 'SHAPE@Z']
                fields = tuple(fields)
                attributeInsert = arcpy.da.InsertCursor(outDataName,fields)
                arcpy.AddMessage('Insert cursor created. Now populating shapefile.....\n\n')
                for line in dataFile:
                    try:
                        lineValues = line.strip().split(separator)
                        #edit to allow z-enabled
                        data = (float(lineValues[0]), float(lineValues[1]), float(lineValues[2]))
                        attributeInsert.insertRow(data)
                    except:
                        pass
        

        try:
            if args.SpatialReference:
                decimatedShpDerives(args.OutputShapefile, args.rawPointCloud, separatorDict[args.Separator][0], args.SpatialReference)
            else:
                decimatedShpDerives(args.OutputShapefile, args.rawPointCloud, separatorDict[args.Separator][0])

            sys.stdout.write('\n\nYour file {0} was successfully converted to {1}\n'.format(args.rawPointCloud, args.OutputShapefile))
        except:
            sys.stdout.write('{0}'.format(sys.exc_info()[1]))

               
except:
    #Collect the error message, if the insert cursor was successfully created then delete it
    arcpy.AddMessage(sys.exc_info()[1])
    if 'attributeInsert' in locals():
        del attributeInsert
    
finally:
    if 'attributeInsert' in locals():
        del attributeInsert
                    
