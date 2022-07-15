using GeometryGym.Ifc;

var db = new DatabaseIfc(ReleaseVersion.IFC4X3_RC4);
var site = new IfcSite(db, "MySite");

var project = new IfcProject(site, "MyProject", IfcUnitAssignment.Length.Metre);
{
};


var bridge = new IfcBridge(db)
{
    Name = "MyBridge",
    Description = "Just another bridge"
};


var facility = new IfcFacility(site, "MyFacility");




var foundations = new IfcFacilityPart(
    facility,
    "Foundations",
    new IfcFacilityPartTypeSelect(IfcFacilityPartCommonTypeEnum.BELOWGROUND),
    IfcFacilityUsageEnum.NOTDEFINED
    );

var name = "Pile Cap";
var length = 1.2;
var width = 1.2;
var depth = 2.4;

IfcFootingType footingType = new IfcFootingType(db, name, IfcFootingTypeEnum.PAD_FOOTING);

footingType.RepresentationMaps.Add(
    new IfcRepresentationMap(
        db.Factory.XYPlanePlacement,
        new IfcShapeRepresentation(
            new IfcExtrudedAreaSolid(
                new IfcRectangleHollowProfileDef(db, name, length, width, depth),
                new IfcAxis2Placement3D(
                    new IfcCartesianPoint(db, 0, 0, 0)
                    ),
                db.Factory.ZAxisNegative,
                depth
            )
        )
    )
);

// IfcRoad road1 = new IfcRoad(site, "my road", null);



var road = new IfcFacility(site, "My Road")
{
    CompositionType = IfcElementCompositionEnum.COMPLEX
};

var CarriagewayA = new IfcFacilityPart(road, "Carriageway A", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);
var CarriagewayB = new IfcFacilityPart(road, "Carriageway B", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);



var pavement = new IfcFacilityPart(
    CarriagewayA,
    "Carriageway",
    new IfcFacilityPartTypeSelect(IfcFacilityPartCommonTypeEnum.BELOWGROUND),
    IfcFacilityUsageEnum.NOTDEFINED
    );

IfcPavementType pavementType = new IfcPavementType(db, "Pavement", IfcPavementTypeEnum.FLEXIBLE);

pavementType.RepresentationMaps.Add(

    new IfcRepresentationMap(
        pavement.Database.Factory.XYPlanePlacement,
        new IfcShapeRepresentation(
            new IfcExtrudedAreaSolid(
                new IfcRectangleHollowProfileDef(pavement.Database, "Surface Course", 200, 3.65, 0.4),
                new IfcAxis2Placement3D(
                    new IfcCartesianPoint(db, 0, 0, 0)
                    ),
                db.Factory.ZAxisNegative,
                0.4
            )
            )
        )
    );

var drainage = new IfcDistributionSystem(
    road,
    "Drainage",
    IfcDistributionSystemEnum.DRAINAGE
    );


db.WriteFile("IFC4X3RC4_testBridge.ifc");


