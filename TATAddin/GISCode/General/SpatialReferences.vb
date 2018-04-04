Module SpatialReferences


    Public Function GetProjection(Optional ByVal iFactoryCode As Integer = -1) As ESRI.ArcGIS.Geometry.IProjection

        'Setup the SpatialReferenceEnvironment - singleton object must use the Activator class
        Dim factoryType As Type = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment")
        Dim spatialRefFactory As ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3 = CType(Activator.CreateInstance(factoryType), ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3)

        Dim projectionSet As ESRI.ArcGIS.esriSystem.ISet = spatialRefFactory.CreatePredefinedProjections()
        projectionSet.Reset()
        Dim pProjection As ESRI.ArcGIS.Geometry.IProjection

        For i As Integer = 0 To projectionSet.Count - 1
            pProjection = projectionSet.Next()

            ' projectionSet = CType(projectionSet.Next(), ESRI.ArcGIS.Geometry.IProjection)
            Debug.Print(String.Format("VCS Name: {0} (Code: {1})", pProjection.Name, pProjection.FactoryCode))
            If pProjection.FactoryCode = iFactoryCode Then
                Debug.Print("Name: {0}. MATCH!", pProjection.Name)
                Return pProjection
            End If
        Next

        Return Nothing

    End Function

    Public Function GetProjectedCoordinateSystem(ByVal iFactoryCode As Integer) As ESRI.ArcGIS.Geometry.IProjection

        'Setup the SpatialReferenceEnvironment - singleton object must use the Activator class
        Dim factoryType As Type = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment")
        Dim spatialRefFactory As ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3 = CType(Activator.CreateInstance(factoryType), ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3)

        Dim pProjectedSystem As ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem = spatialRefFactory.CreateProjectedCoordinateSystem(iFactoryCode)
        Return pProjectedSystem

    End Function

    Public Function GetVerticalCoordinateSystem(Optional ByVal iFactoryCode As Integer = -1) As ESRI.ArcGIS.Geometry.IVerticalCoordinateSystem

        'Setup the SpatialReferenceEnvironment - singleton object must use the Activator class
        Dim factoryType As Type = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment")
        Dim spatialRefFactory As ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3 = CType(Activator.CreateInstance(factoryType), ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3)

        Dim vcsSet As ESRI.ArcGIS.esriSystem.ISet = spatialRefFactory.CreatePredefinedVerticalCoordinateSystems()
        vcsSet.Reset()
        Dim vcs As ESRI.ArcGIS.Geometry.IVerticalCoordinateSystem

        For i As Integer = 0 To vcsSet.Count - 1
            vcs = CType(vcsSet.Next(), ESRI.ArcGIS.Geometry.IVerticalCoordinateSystem)
            Debug.Print(String.Format("VCS Name: {0} (Code: {1})", vcs.Name, vcs.FactoryCode))
            If vcs.FactoryCode = iFactoryCode Then
                Debug.Print("Name: {0}. MATCH!", vcs.Name)
                Return vcs
            End If
        Next

        Return Nothing

    End Function

    Public Function GetLinearUnits(Optional ByVal iFactoryCode As Integer = -1) As ESRI.ArcGIS.Geometry.IVerticalCoordinateSystem

        'Setup the SpatialReferenceEnvironment - singleton object must use the Activator class
        Dim factoryType As Type = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment")
        Dim spatialRefFactory As ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3 = CType(Activator.CreateInstance(factoryType), ESRI.ArcGIS.Geometry.ISpatialReferenceFactory3)

        Dim linearUnitSet As ESRI.ArcGIS.esriSystem.ISet = spatialRefFactory.CreatePredefinedLinearUnits
        linearUnitSet.Reset()
        Dim pLinearUnit As ESRI.ArcGIS.Geometry.ILinearUnit

        For i As Integer = 0 To linearUnitSet.Count - 1
            pLinearUnit = CType(linearUnitSet.Next(), ESRI.ArcGIS.Geometry.ILinearUnit)
            Debug.Print(String.Format("VCS Name: {0} (Code: {1})", pLinearUnit.Name, pLinearUnit.FactoryCode))
            If pLinearUnit.FactoryCode = iFactoryCode Then
                Debug.Print("Name: {0}. MATCH!", pLinearUnit.Name)
                Return pLinearUnit
            End If
        Next

        Return Nothing

    End Function

End Module
