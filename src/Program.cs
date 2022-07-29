using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GeometryGym.Ifc;

var db = new DatabaseIfc(ReleaseVersion.IFC4X3_RC4);
var site = new IfcSite(db, "Z1");
site.Description = "Zone 1 - Kingsway";

var cdedata = new IfcDocumentInformation(site.Database, "HE514503-BAM-GEN-Z1-M3-IM-0001", "Federated Model");
cdedata.Revision = "P02";
cdedata.Purpose = "S3";
cdedata.Location = "HE514503-Z1";
cdedata.Status = IfcDocumentStatusEnum.FINAL;
cdedata.IntendedUse = "For Review and Comment";
cdedata.Description = "A38 Derby Junctions All Discipline Federated Model";

IfcDocumentReference data = new IfcDocumentReference(db);
data.ReferencedDocument = cdedata;
db.Contains(data);

var project = new IfcProject(site, "HE514503", IfcUnitAssignment.Length.Metre);
project.Description = "A38 Derby Junctions";

project.Database.Contains(cdedata);

Pset_SiteCommon pset_SiteCommon = new Pset_SiteCommon(site);
pset_SiteCommon.Name = "CDE Metadata";
pset_SiteCommon.Description = "Data from Projectwise";

List<IfcProperty> fileinfo = new List<IfcProperty>();
fileinfo.Add(new IfcPropertySingleValue(db, "Document ID", new IfcIdentifier(cdedata.Identification)));
fileinfo.Add(new IfcPropertySingleValue(db, "Name", new IfcIdentifier(cdedata.Name)));
fileinfo.Add(new IfcPropertySingleValue(db, "Description", new IfcIdentifier(cdedata.Description)));
fileinfo.Add(new IfcPropertySingleValue(db, "Revision", new IfcIdentifier(cdedata.Revision)));
fileinfo.Add(new IfcPropertySingleValue(db, "Purpose", new IfcIdentifier(cdedata.Purpose)));
fileinfo.Add(new IfcPropertySingleValue(db, "Location", new IfcIdentifier(cdedata.Location)));
fileinfo.Add(new IfcPropertySingleValue(db, "Intended Use", new IfcIdentifier(cdedata.IntendedUse)));

new IfcPropertySet(project, "File Properties", fileinfo);

IfcClassification uniclass2015 = new IfcClassification(db, "Uniclass2015");

var facility = new IfcFacility(db, "MyFacility")
{
    Description = "Just another facility",
};


// Existing Ground

var Existing = new IfcSpatialZone(site, "Existing Ground");


Tuple<int, int, int>[] coords =
    {
    Tuple.Create(10, 10, 4), 
    Tuple.Create(20, 20, 3), 
    Tuple.Create(10, 20, 5), 
    Tuple.Create(20, 10, 2)};

Tuple<double, double, double>[] coordsdouble =
    {
    Tuple.Create(10.0, 10.0, 4.0),
    Tuple.Create(20.0, 20.0, 3.0),
    Tuple.Create(10.0, 20.0, 5.0),
    Tuple.Create(20.0, 10.0, 2.0)};

var points = new IfcCartesianPointList3D(Existing.Database, coordsdouble);

var list = new List<int>();
list.Add(1);
list.Add(2);
list.Add(3);
list.Add(4);

var TIN = new IfcTriangulatedIrregularNetwork(points, coords , list);

IfcTriangulatedFaceSet face = new IfcTriangulatedFaceSet(points, coords);

//Existing.Representation = new IfcProductDefinitionShape(new IfcShapeRepresentation(face));

//var origin = new IfcAxis2Placement3D(new IfcCartesianPoint(db,0, 0, 0));

var ground = new IfcGeographicElement(Existing, Existing.ObjectPlacement, new IfcProductDefinitionShape(new IfcShapeRepresentation(TIN)));
ground.Name = "Existing Ground";
ground.Description = "Zone 1 - Kingsway Existimg Ground";



// IFC BRIDGE

var bridge = new IfcBridge(facility.Database)
{
    Name = "B01 - Kingsway Overbridge",
    Description = "New Overbridge for Kingsway Grade Separated Junction"
};

IfcClassificationReference BridgeEntities = new IfcClassificationReference(uniclass2015);
BridgeEntities.Identification = "En_80_94";
BridgeEntities.Name = "Bridges";

BridgeEntities.Associate(bridge);

var contained = new IfcRelContainedInSpatialStructure(site);
contained.RelatedElements.Add(bridge);



var foundations = new IfcBridgePart(bridge, null, null);
foundations.Name = "Foundations";

var name = "Pile Cap";
var length = 1.2;
var width = 1.2;
var depth = 2.4;

IfcMaterial concrete = new IfcMaterial(db, "Concrete");
{ 
};
  
IfcFootingType footingType = new IfcFootingType(db, name, IfcFootingTypeEnum.PAD_FOOTING);
{
    IfcMaterial MaterialSelect = concrete;
};

IfcRectangleProfileDef rect = new IfcRectangleProfileDef(db, "rect", length, width);
IfcExtrudedAreaSolid extrusion = new IfcExtrudedAreaSolid(rect, new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, 0)), new IfcDirection(db, 0, 0, 1), depth);

IfcProductDefinitionShape productRep = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrusion));
IfcShapeRepresentation shapeRep = new(extrusion);

footingType.RepresentationMaps.Add(
    new IfcRepresentationMap(
        db.Factory.XYPlanePlacement,
        shapeRep
    )
);

IfcFooting footing = new(
    foundations,
    foundations.ObjectPlacement,
    productRep
    )
{
    PredefinedType = IfcFootingTypeEnum.PAD_FOOTING,
    ObjectType = name
};

footing.Name = "Footing";

IfcLine line = new IfcLine(
    new IfcCartesianPoint(db, 0, 0, 0),
    new IfcVector(new IfcDirection(db, 1, 0), 10.0)
);

IfcSectionedSolidHorizontal sec = new IfcSectionedSolidHorizontal(
    line,
    new List<IfcProfileDef> { rect, rect, rect },
    new List<IfcAxis2PlacementLinear> {
        new IfcAxis2PlacementLinear(new IfcPointByDistanceExpression(0.0, line)),
        new IfcAxis2PlacementLinear(new IfcPointByDistanceExpression(5.0, line)),
        new IfcAxis2PlacementLinear(new IfcPointByDistanceExpression(10.0, line)) },
    true
);

IfcMaterial steel = new IfcMaterial(db, "steel");
{
};

IfcBeam beam = new(foundations, foundations.ObjectPlacement, new IfcProductDefinitionShape(new IfcShapeRepresentation(sec)))
{
    PredefinedType = IfcBeamTypeEnum.BEAM,
    Name = "Beam"
};

IfcBeamType beamtype = new IfcBeamType(db, "Beam", IfcBeamTypeEnum.BEAM);
{
    IfcMaterial MaterialSelect = steel;
};

IfcShapeRepresentation beamshapeRep = new(sec);

beamtype.RepresentationMaps.Add(
    new IfcRepresentationMap(
        db.Factory.XYPlanePlacement,
        beamshapeRep
    )
);

// IfcRoad

// Think about ADMM Structure for output - Location / GG184 Volumes / Asset Class / Objects
// For example : Z1 / HPV / PVPV / Surface Course
// or ZZ / HDG / DRCH / Chamber
// or Z3 / SBR / BRBR / Bridge Deck

var road = new IfcRoad(facility.Database);
road.Name = "Zone 1 - Kingsway A38 Mainline";
road.Description = "Mainline Corridor through Kingsway Junction";
road.ObjectType = "Road Complexes";


IfcClassificationReference RoadComplexes = new IfcClassificationReference(uniclass2015);
RoadComplexes.Identification = "Co_80_35";
RoadComplexes.Name = "Road Complexes";

RoadComplexes.Associate(road);

var rdcontained = new IfcRelContainedInSpatialStructure(site);
rdcontained.RelatedElements.Add(road);


var RoadCorridor = new IfcFacilityPart(road, "Roadway Corridor", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);

var Carriageway = new IfcFacilityPart(RoadCorridor, "Carriageway", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);
var RoadwaySide1 = new IfcFacilityPart(RoadCorridor, "Roadway Side A", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);
var RoadwaySide2 = new IfcFacilityPart(RoadCorridor, "Roadway Side B", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.ROADSEGMENT), IfcFacilityUsageEnum.LONGITUDINAL);

//var pavement = new IfcFacilityPart(
//    Carriageway,
//    "Pavement",
//    new IfcFacilityPartTypeSelect(IfcFacilityPartCommonTypeEnum.BELOWGROUND),
//    IfcFacilityUsageEnum.NOTDEFINED
//    );

IfcPavement pavement = new IfcPavement(Carriageway.Database)
 {
    Name = "Flexible Pavment",
    PredefinedType = IfcPavementTypeEnum.FLEXIBLE,
    ObjectType = "Pavement"
 };

var pavecontain = new IfcRelContainedInSpatialStructure(Carriageway);
pavecontain.RelatedElements.Add(pavement);

IfcMaterial asphalt = new IfcMaterial(db, "Asphalt");
{
};

IfcPavementType pavementType = new IfcPavementType(pavement.Database, "Pavement", IfcPavementTypeEnum.FLEXIBLE);
{
    IfcMaterial MaterialSelect = asphalt;
};

IfcRectangleProfileDef pave = new IfcRectangleProfileDef(pavement.Database, "pave", 200, 3.65);
IfcExtrudedAreaSolid mesh = new IfcExtrudedAreaSolid(pave, new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, 0)), new IfcDirection(db, 0, 0, 1), 0.4);

IfcProductDefinitionShape productPav = new IfcProductDefinitionShape(new IfcShapeRepresentation(mesh));
IfcShapeRepresentation shapePav = new(mesh);

pavementType.RepresentationMaps.Add(
    new IfcRepresentationMap(
        pavement.Database.Factory.XYPlanePlacement,
        shapePav
    )
);

IfcCourse surfacecourse = new(
    pavement,
    null,
    productPav
    )
{
    Name = "CH_EF3060_M3_ACM_VehicularPavement",
    Description = "Pavement, Surface Course",
    PredefinedType = IfcCourseTypeEnum.PAVEMENT,
    ObjectType = "Pavement"
};

IfcClassificationReference PavementClass = new IfcClassificationReference(uniclass2015);
PavementClass.Identification = "EN_30_60_95";
PavementClass.Name = "Vehicular Pavement";

PavementClass.Associate(surfacecourse);

var earthworks1 = new IfcEarthworksElement(RoadwaySide1.Database);
earthworks1.Name = "Earthworks A";
earthworks1.ObjectType = "Earthworks";

var earth1contain = new IfcRelContainedInSpatialStructure(RoadwaySide1);
earth1contain.RelatedElements.Add(earthworks1);

var fill1 = new IfcEarthworksFill(earthworks1.Database);
fill1.Description = "Fill";
fill1.PredefinedType = IfcEarthworksFillTypeEnum.EMBANKMENT;

//var fill1contain = new IfcRelContainedInSpatialStructure(earthworks1.Database);
//fill1contain.RelatedElements.Add(fill1);

var cut1 = new IfcEarthworksCut(RoadwaySide2.Database);
cut1.Description = "Cut";
cut1.PredefinedType = IfcEarthworksCutTypeEnum.CUT;

var cut1contain = new IfcRelContainedInSpatialStructure(RoadwaySide2);
cut1contain.RelatedElements.Add(cut1);

// var vrs1 = new IfcRailing(Carriageway);


//IfcPolyline polyline = new IfcPolyline();
//polyline.Points = new List<IfcCartesianPoint>;

//foreach (var point in Points)
//{
//    IfcPolyline.Points.Add(point.IfcCartesianPoint);
//    
//};


// DRAINAGE

var DrainageNetwork = new IfcFacility(site, "Zone 1 - Drainage");
DrainageNetwork.Description = "Zone 1 - Kingsway - Drainage Network Discharge Point 1";


var HDGvolume = new IfcFacilityPart(DrainageNetwork, "HDG", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.USERDEFINED), IfcFacilityUsageEnum.LONGITUDINAL);
HDGvolume.Description = "Highways - Drainage";
IfcClassificationReference SystemClass = new IfcClassificationReference(uniclass2015);
SystemClass.Identification = "Ss_50_35_08";
SystemClass.Name = "Below-ground gravity drainage systems";
SystemClass.Associate(HDGvolume);

var HDGAdmmClass = new IfcFacilityPart(HDGvolume, "DRMH", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.USERDEFINED), IfcFacilityUsageEnum.LONGITUDINAL);
HDGAdmmClass.Description = "ADMM Class - Drainage - Manholes";
IfcClassificationReference TypeClass = new IfcClassificationReference(uniclass2015);
TypeClass.Identification = "Ss_50_35_06";
TypeClass.Name = "Below-ground inspection systems";
TypeClass.Associate(HDGAdmmClass);

var DP01 = new IfcFacilityPart(HDGAdmmClass, "DP01", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.USERDEFINED), IfcFacilityUsageEnum.LONGITUDINAL);

var drainage = new IfcDistributionSystem(
    DP01,
    "Drainage",
    IfcDistributionSystemEnum.DRAINAGE
    );

var drcontained = new IfcRelReferencedInSpatialStructure(site);
drcontained.RelatedElements.Add(drainage);

foreach (int value in Enumerable.Range(1, 10))
{
    IfcCircleProfileDef circle = new IfcCircleProfileDef(drainage.Database, "Chamber Plan", 1.2);
    IfcExtrudedAreaSolid chamber = new IfcExtrudedAreaSolid(circle, new IfcAxis2Placement3D(new IfcCartesianPoint(db, (1*value*value), (1 * value * value), 1)), new IfcDirection(db, 0, 0, 1), 2.5);

    IfcProductDefinitionShape productChamber = new IfcProductDefinitionShape(new IfcShapeRepresentation(chamber));
    IfcShapeRepresentation shapeChamber = new(chamber);

    IfcDistributionChamberElement DrainageChamber = new IfcDistributionChamberElement(DP01, DP01.ObjectPlacement, productChamber, drainage);
    DrainageChamber.Name = (site.Name + "-" + HDGvolume.Name + "-" + HDGAdmmClass.Name + "-" + DP01.Name + "-MH0"+value);
    DrainageChamber.Description = "Type 3a Manhole";
    DrainageChamber.SetMaterial(concrete);
    {
        IfcMaterial MaterialSelect = concrete;
    };
    List<IfcProperty> ManholeClass = new List<IfcProperty>();
    ManholeClass.Add(new IfcPropertySingleValue(db, "Code", new IfcIdentifier("Ss_50_35_06_14")));
    ManholeClass.Add(new IfcPropertySingleValue(db, "Description", new IfcIdentifier("Concrete manhole systems")));

    new IfcPropertySet(DrainageChamber, "Classification", ManholeClass);

};

var HDGAdmmClassCatchpit = new IfcFacilityPart(HDGvolume, "DRCP", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.USERDEFINED), IfcFacilityUsageEnum.LONGITUDINAL);
HDGAdmmClassCatchpit.Description = "ADMM Class - Drainage - Catchpit";
TypeClass.Associate(HDGAdmmClassCatchpit);

var DP01CP = new IfcFacilityPart(HDGAdmmClassCatchpit, "DP01", new IfcFacilityPartTypeSelect(IfcRoadPartTypeEnum.USERDEFINED), IfcFacilityUsageEnum.LONGITUDINAL);

foreach (int value in Enumerable.Range(20, 30))
{
    IfcCircleProfileDef circle = new IfcCircleProfileDef(drainage.Database, "Chamber Plan", 1.05);
    IfcExtrudedAreaSolid chamber = new IfcExtrudedAreaSolid(circle, new IfcAxis2Placement3D(new IfcCartesianPoint(db, (1 * value * value), (1 * value * value), 1)), new IfcDirection(db, 0, 0, 1), 2.5);

    IfcProductDefinitionShape productChamber = new IfcProductDefinitionShape(new IfcShapeRepresentation(chamber));
    IfcShapeRepresentation shapeChamber = new(chamber);

    IfcDistributionChamberElement DrainageChamber = new IfcDistributionChamberElement(DP01CP, DP01CP.ObjectPlacement, productChamber, drainage);
    DrainageChamber.Name = (site.Name + "-" + HDGvolume.Name + "-" + HDGAdmmClassCatchpit.Name + "-" + DP01CP.Name + "-CP0" + value);
    DrainageChamber.Description = "Type 7 Catchpit";
    {
        IfcMaterial MaterialSelect = concrete;
    };
    List<IfcProperty> CatchpitClass = new List<IfcProperty>();
    CatchpitClass.Add(new IfcPropertySingleValue(db, "Code", new IfcIdentifier("Ss_50_35_06_14")));
    CatchpitClass.Add(new IfcPropertySingleValue(db, "Description", new IfcIdentifier("Concrete manhole systems")));

    new IfcPropertySet(DrainageChamber, "Classification", CatchpitClass);
};

// Complex Multi object built items (CAD Blocks and Cells and Families)




// SAVEAS

//System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
//saveFileDialog.Filter = "IFC BIM Data (*.ifc,*.ifcxml,*.ifcjson,*.ifczip)|*.ifc;*.ifcxml;*.ifcjson;*.ifczip"; ;
//if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//    db.WriteFile(saveFileDialog.FileName);

db.WriteFile("IFC4X3RC4_testBridge.ifc");
db.WriteFile("IFC4X3RC4_testBridge.ifcxml");
//db.WriteFile("IFC4X3RC4_testBridge.ifcjson"); //not a true ifc-json file


