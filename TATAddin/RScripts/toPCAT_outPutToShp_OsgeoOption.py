import sys, os, argparse, arcpy

osgeoCheck = 0
try:
    from osgeo import ogr, osr
except:
    osgeoCheck = 1


#Parse the input variables to a parsing object and provide basic descriptions for --help tag
parser = argparse.ArgumentParser()
parser.add_argument('--decimatedInputTxt',
                    help = 'The input point cloud to be used to create the point shapefile',
                    action = 'append',
                    default = [])

parser.add_argument('--outputShp',
                    help = 'The path to save the space delimted x y z file to',
                    action = 'append',
                    default = [])

parser.add_argument('--SpatialReference',
                    help = 'The .prj file or .shp file containing the spaital reference to be used for the TIN.',
                    type = str)


args = parser.parse_args()
sys.stdout.write('\n\n\nArguements parsed!!\n\n')

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

        def createFields(line, lyr):
            '''Helps in the creation of shapefiles from Decimated Point clouds because there are so many fields'''
            header = line.strip().split(',')
            for i in header[2:]:
                field = ogr.FieldDefn(str(i)[:8], ogr.OFTReal)
                lyr.CreateField(field)
            return header

        def decimatedShpDerives(outDataName, inData, spatialRef = None):
            '''Create shapefiles from the derivatives of the decimated point clouds'''
            dS = shpDriver(outDataName)
            lyr = dS.CreateLayer(str(os.path.splitext(outDataName)), spatialRef, ogr.wkbPoint)
            with open(inData, 'r') as dataFile:
                header = createFields(dataFile.readline(), lyr)
                lyrDef = lyr.GetLayerDefn()
                for line in dataFile:
                    lineValues = line.strip().split(',')
                    geom = ogr.Geometry(ogr.wkbPoint)
                    geom.AddPoint(float(lineValues[0]),float(lineValues[1]))
                    row = ogr.Feature(lyrDef)
                    row.SetGeometry(geom)
                    ct = 2
                    try:
                        for i in header[2:]:
                            row.SetField(str(i)[:8], float(lineValues[ct]))
                            ct += 1
                        lyr.CreateFeature(row)
                    except:
                        pass
            dS.Destroy()

        ct = 0
        for inputTxt in args.decimatedInputTxt:
            try:
                if os.path.exists(args.outputShp[ct]):
                    sys.stdout.write('\n\n{0} already exists.\n'.format(args.outputShp[ct]))
                    if ct == len(args.outputShp):
                        sys.exit()
                    else: 
                        continue

                if args.SpatialReference:
                    #Set spatial reference
                    spatialRef = osr.SpatialReference()
                    spatialRef.SetFromUserInput('ESRI::' + args.SpatialReference)
                    decimatedShpDerives(args.outputShp[ct], inputTxt , spatialRef)
                else:
                    decimatedShpDerives(args.outputShp[ct], inputTxt)

                sys.stdout.write('\n\nYour file {0} was successfully converted to {1}\n'.format(inputTxt, args.outputShp[ct]))
                ct += 1
            except:
                sys.stdout.write('{0}'.format(sys.exc_info()[1]))
                ct += 1
                continue

    #Do this is the user does not have osgeo
    elif osgeoCheck == 1:
        sys.stdout.write('\nUsing arcpy...')
        #FUNCTIONS TO DO WORK
        def createFields(line, lyr):
            '''Helps in the creation of shapefiles from Decimated Point clouds because there are so many fields'''
            header = line.strip().split(',')
            for i in header[2:]:
                field = str(i)[:10]#this may need to change as ESRI allows larger field names
                arcpy.AddField_management(lyr, field, 'DOUBLE')
            return header
        
        def decimatedShpDerives(outDataName, inData, spatialRef = None):
            '''Create shapefiles from the derivatives of the decimated point clouds'''
            arcpy.CreateFeatureclass_management(os.path.split(outDataName)[0],
                                                os.path.split(outDataName)[1],
                                                'POINT',
                                                '', '', '',
                                                spatialRef)
            with open(inData, 'r') as dataFile:
                header = createFields(dataFile.readline(), outDataName)
                header = [x.replace('-', '_')[:10] for x in header]#makes original header the same as field names
                arcpy.DeleteField_management(outDataName, 'Id')#gets rid of automatically made Id field
                fields = ['SHAPE@X', 'SHAPE@Y']
                for i in header[2:]:
                    fields.append(i)
                fields = tuple(fields)
                attributeInsert = arcpy.da.InsertCursor(outDataName,fields)
                for line in dataFile:
                    try:
                        lineValues = line.strip().split(',')
                        data = []
                        for i in lineValues:
                            data.append(float(i))
                        data = tuple(data)
                        attributeInsert.insertRow(data)
                    except:
                        pass
        
        ct = 0
        for inputTxt in args.decimatedInputTxt:
            try:
                if os.path.exists(args.outputShp[ct]):
                    sys.stdout.write('\n\n{0} already exists.\n'.format(args.outputShp[ct]))
                    if ct == len(args.outputShp):
                        sys.exit()
                    else: 
                        continue

                if args.SpatialReference:
                    decimatedShpDerives(args.outputShp[ct], inputTxt , args.SpatialReference)
                else:
                    decimatedShpDerives(args.outputShp[ct], inputTxt)

                sys.stdout.write('\n\nYour file {0} was successfully converted to {1}\n'.format(inputTxt, args.outputShp[ct]))
                ct += 1
            except:
                sys.stdout.write('{0}'.format(sys.exc_info()[1]))
                ct += 1
                continue
               
except:
    #Collect the error message, if the insert cursor was successfully created then delete it
    arcpy.AddMessage(sys.exc_info()[1])
    if 'attributeInsert' in locals():
        del attributeInsert
    
finally:
    if 'attributeInsert' in locals():
        del attributeInsert
                    
                    
