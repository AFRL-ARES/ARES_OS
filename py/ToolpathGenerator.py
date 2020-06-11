import AmaresConfig as config
from BasicMotion import plungerDistance, absDistance
from BasicComm import getPos, addToLog
from operator import add, sub
from random import uniform
import math
from Tools import alignmentCamera, depHead

"""
Format for creating a toolpath:

For a square:
    initTP()
    startDep('X',10)
    depToPoint('Y',10)
    depToPoint('X',0)
    endDep('Y',0)

For a right angle triangle:
    initTP()
    startDep('XY',10,10)
    depToPoint('Y',0)
    endDep('X',0)
"""

#config.init()   # initializes dictionaries (temporary for testing)


def offsetCoords(coords):
    # Add necessary offsets to XYZ coordinates
    try:
        coords = list(map(add,coords,config.coords["dynHome"]))
        coords = list(map(add,coords,config.coords["gridHome"]))
        coords = list(map(float,coords))
        return(coords)
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.offsetCoords")
        addToLog("\t" + str(ex))

def checkAxes(axes):
    # makes sure that user only enters X, Y, or Z
    try:
        check = 'Pass'
        for axis in axes:
            if(axis not in 'XYZ'): check = 'Fail'
        return(check)
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.checkAxes")
        addToLog("\t" + str(ex))

def formatCmd(axes, coords):
    try:
        axes = axes.upper()
        if(len(axes)!=len(coords)):
            raise ValueError('number of axes doesn''t match number of coords',axes,coords)
        elif(len(axes)>3 or len(coords)>3):
            raise ValueError('number of values exceeds maximum of 3',axes,coords)
        elif(checkAxes(axes)=='Fail'):
            raise ValueError('axes may only contain X, Y, and/or Z',axes)
        else:
            # fetch previous coordinate values:
            x = config.tpGen["coords"][-1][0]
            y = config.tpGen["coords"][-1][1]
            z = config.tpGen["coords"][-1][2]
            # update coordinates as required:
            for index,axis in enumerate(axes):
                # only offsets new entries:
                if(axis=='X'):
                    config.tpGen["maxcoords"][0] = max(config.tpGen["maxcoords"][0],coords[index])
                    x = offsetCoords([coords[index],0,0])[0]
                elif(axis=='Y'):
                    config.tpGen["maxcoords"][1] = max(config.tpGen["maxcoords"][1],coords[index])
                    y = offsetCoords([0,coords[index],0])[1]
                elif(axis=='Z'):
                    config.tpGen["maxcoords"][2] = max(config.tpGen["maxcoords"][2],coords[index])
                    z = offsetCoords([0,0,coords[index]])[2]
            return(x,y,z)
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.formatCmd")
        addToLog("\t" + str(ex))

def initTP():
    try:
        # Initialize coordinate logging and clears previous toolpath:
        config.tpGen["coords"] = []
        # Initialize tracker for volume of material needed by toolpath:
        config.tpGen["finalPlungerDisplacement"] = 0
        #config.coords["dynHome"] = getPos()[:3] # Will want to take this out later when "define part home" exists
        config.tpGen["coords"].append(offsetCoords([0,0,0])) # get starting X,Y,Z (w/ offsets)
        config.tpGen["coords"][0].append(getPos()[3]) # add starting E to first index
        # Initialize gcode:
        config.tpGen["gcode"] = []
        # the amount of tip displacement from pressure being applied to the syringe:
        config.dispenser["flexDist"] = round(config.dispenser["flexFactor"] * config.dispenser["prime"],6)
        addToLog("Flex Dist. = " + str(config.dispenser["flexDist"]))
        config.tpGen["gcode"].append("M204 S500")   #set accel to 500 mm/s^2
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.initTP")
        addToLog("\t" + str(ex))
    
def startDep(axes, *coords):
    # Moves to point without extruding and primes tip
    try:
        # Load in values from library:
        sigfigs = config.configuration["sigfigs"]
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.motion["fastSpeed"]
        prime = config.dispenser["prime"]
        prRate = config.dispenser["prime_rate"]
        prDelay = config.dispenser["prime_delay"] * 1000 # ms
        config.tpGen["finalPlungerDisplacement"] += prime

        # format/update coordinate entry appropriately:
        coords = formatCmd(axes, coords)

        # Simplify variables for GCODE assembly:
        x = round(coords[0],sigfigs)
        y = round(coords[1],sigfigs)
        z = round(coords[2] + tipHeight + flexDist,sigfigs)
        e = round(config.tpGen["coords"][-1][3],sigfigs) # fetches the most recent 'e' value

        # increments axis values appropriately:
        e = round(e + prime,8)
        if(len(config.tpGen["gcode"])==1):      #if it's the beginning of toolpath
            config.tpGen["gcode"].append("G90") # absolute motion
            config.tpGen["gcode"].append("G0 Z" + str(z) + " F500")

        config.tpGen["gcode"].append("G0 X" + str(x) + " Y" + str(y) + " Z" +
                                     str(z) + " F" + str(speed))
        config.tpGen["gcode"].append("G1 E" + str(e) + " F" + str(prRate))
        config.tpGen["gcode"].append("G4 P" + str(prDelay))
        
        z -= (tipHeight + flexDist)
        config.tpGen["coords"].append([x,y,z,e])
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.startDep")
        addToLog("\t" + str(ex))

def depToPoint(axes, *coords):
    # Moves to point while extruding appropriate amount
    try:
        # Load in values from library:
        sigfigs = config.configuration["sigfigs"]
        tipHeight = config.dispenser["work_dist"] # tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.dispenser["speed"] * 60 # convert from mm/s to mm/min

        # format/update coordinate entry appropriately:
        coords = formatCmd(axes, coords)

        # Simplify variables for GCODE assembly:
        x = round(coords[0],sigfigs)
        y = round(coords[1],sigfigs)
        z = round(coords[2] + tipHeight + flexDist,sigfigs)
        e = round(config.tpGen["coords"][-1][3],sigfigs) # fetches the most recent 'e' value
        
        # increments extruder appropriately:
        # first subtract previous coordinates from new coordinates to get vector:
        # vector = list(map(sub,[x,y,z],config.tpGen["coords"][-1][:3]))
        # then calculate the amount needed to dispense for that vector:
        pDist = plungerDistance(absDistance([x,y,z]))
        e = round(e + pDist,sigfigs)
        config.tpGen["finalPlungerDisplacement"] += pDist

        config.tpGen["gcode"].append("G1 X" + str(x) + " Y" + str(y) + " Z" +
                                     str(z) + " E" + str(e) + " F" + str(speed))

        z -= (tipHeight + flexDist)
        config.tpGen["coords"].append([x,y,z,e])
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.depToPoint")
        addToLog("\t" + str(ex))
    

def noDepToPoint(axes, *coords):
    # Moves to a point without any extrusion action
    try:
        # Load in values from library:
        sigfigs = config.configuration["sigfigs"]
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.motion["fastSpeed"]

        # format/update coordinate entry appropriately:
        coords = formatCmd(axes, coords)

        # Simplify variables for GCODE assembly:
        x = round(coords[0],sigfigs)
        y = round(coords[1],sigfigs)
        z = round(coords[2] + tipHeight + flexDist,sigfigs)
        e = round(config.tpGen["coords"][-1][3],sigfigs) # fetches the most recent 'e' value

        config.tpGen["gcode"].append("G1 X" + str(x) + " Y" + str(y) + " Z" +
                                     str(z) + " F" + str(speed))

        z -= (tipHeight + flexDist)
        config.tpGen["coords"].append([x,y,z,e])
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.noDepToPoint")
        addToLog("\t" + str(ex))

def endDep(axes, *coords):
    # Moves to point while extruding and retracts material
    # might want to change this later to begin retraction before finishing move (coasting)
    try:
        # Load in values from library:
        sigfigs = config.configuration["sigfigs"]
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = int(config.dispenser["speed"] * 60) # convert from mm/s to mm/min
        retract = config.dispenser["prime"]
        retRate = config.dispenser["retract_rate"]
        retDelay = config.dispenser["retract_delay"] * 1000 #ms

        # format/update coordinate entry appropriately:
        coords = formatCmd(axes, coords)

        # Simplify variables for GCODE assembly:
        x = round(coords[0],sigfigs)
        y = round(coords[1],sigfigs)
        z = round(coords[2] + tipHeight + flexDist,sigfigs)
        e = round(config.tpGen["coords"][-1][3],sigfigs) # fetches the most recent 'e' value

        # increments extruder appropriately:
        # first subtract previous coordinates from new coordinates to get vector:
        # vector = list(map(sub,[x,y,z],config.tpGen["coords"][-1][:3]))
        # then calculate the amount needed to dispense for that vector:
        pDist = plungerDistance(absDistance([x,y,z]))
        e = round(e + pDist,sigfigs)
        config.tpGen["finalPlungerDisplacement"] += pDist

        config.tpGen["gcode"].append("G1 X" + str(x) + " Y" + str(y) + " Z" +
                                     str(z) + " E" + str(e) + " F" + str(speed))

        e = round(e - retract,sigfigs)
        config.tpGen["finalPlungerDisplacement"] -= retract
        config.tpGen["gcode"].append("G1 E" + str(e) + " F" + str(retRate))
        config.tpGen["gcode"].append("G4 P" + str(retDelay))

        z -= (tipHeight + flexDist)
        config.tpGen["coords"].append([x,y,z,e])
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.endDep")
        addToLog("\t" + str(ex))

def callCleanTip():
    try:
        num_chords = config.tipCleaning["numChords"] # number of chords to approximate circle
        chord_angle = math.radians(360 / num_chords)
        
        x = config.tipCleaning["cleanXYZ"][0]
        y = config.tipCleaning["cleanXYZ"][1] + uniform(-30,30)
        z = config.tipCleaning["cleanXYZ"][2]
        zStep = config.tipCleaning["zStep"]
        radius = config.tipCleaning["xyStep"]
        radius_incr = config.tipCleaning["radiusIncrement"]
        speed = config.tipCleaning["lateralSpeed"]
        
        returnDest = getPos()[:3]
        safeZ = returnDest[2] + config.tpGen["maxcoords"][2] + 2
        returnDest[2] += config.tipCleaning['zOffsetWhenDone']
        config.tpGen["gcode"].append("# TIP CLEANING CODE:")
        config.tpGen["gcode"].append("G90")
        config.tpGen["gcode"].append("G0 Z"+str(safeZ)+" F800")
        config.tpGen["gcode"].append("G0 X"+str(x)+" Y"+str(y)+" F5000")
        config.tpGen["gcode"].append("G0 Z"+str(z)+" F800")
        #config.tpGen["gcode"].append("G91")
        config.tpGen["gcode"].append("G0 F"+str(speed))
        config.tpGen["gcode"].append("G0 X"+str(x-radius))
        # constant radius rotation while lowering tip:
        for j in range(1,5):
            for k in range(1,num_chords):
                # x = center_x + radius * cosine(angle)
                circle_x = round(x + radius * math.cos(chord_angle*k),6)
                # y = center_y + radius * sine(angle)
                circle_y = round(y + radius + math.sin(chord_angle*k),6)
                newZ = round(z-zStep*(j-1)+(-zStep/num_chords)*k,6)
                config.tpGen["gcode"].append("G0 X"+str(circle_x)+
                                             " Y"+str(circle_y)+
                                             " Z"+str(newZ))
        # increasing radius rotation while raising tip:
        for j in range(1,20):
            for k in range(1,num_chords):
                # x = center_x + radius * cosine(angle)
                newRadius = radius + ((j-1)*radius_incr) + (radius_incr/num_chords)*k
                circle_x = round(x + newRadius * math.cos(chord_angle*k),6)
                # y = center_y + radius * sine(angle)
                circle_y = round(y + newRadius + math.sin(chord_angle*k),6)
                newZ = round(z+zStep*(j-6)+(zStep/num_chords)*k,6)
                config.tpGen["gcode"].append("G0 X"+str(circle_x)+
                                             " Y"+str(circle_y)+
                                             " Z"+str(newZ))
        config.tpGen["gcode"].append("G0 Z"+str(safeZ)+" F800")
        config.tpGen["gcode"].append("G0 X"+str(returnDest[0])+" Y"+
                                     str(returnDest[1])+" F5000")
        config.tpGen["gcode"].append("G0 Z"+str(returnDest[2])+" F800")
        
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.callCleanTip")
        addToLog("\t" + str(ex))

def pauseToolpath(seconds):
    try:
        config.tpGen["gcode"].append("G4 S" + str(seconds))
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.pauseToolpath")
        addToLog("\t" + str(ex))

def setMinDuration(min_duration=0):
    try:
        config.expt['wait_time'] = min_duration
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.setMinDuration")
        addToLog("\t" + str(ex))

    
