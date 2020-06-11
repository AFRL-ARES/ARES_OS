import AmaresConfig as config
from BasicMotion import plungerDistance, absDistance
from BasicComm import getPos
from operator import add, sub

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

config.init()   # initializes dictionaries (temporary for testing)

sigfigs = config.configuration["sigfigs"]

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
        #config.coords["dynHome"] = getPos()[:3] # Will want to take this out later when "define part home" exists
        config.tpGen["coords"].append(offsetCoords([0,0,0])) # get starting X,Y,Z (w/ offsets)
        config.tpGen["coords"][0].append(getPos()[3]) # add starting E to first index
        # Initialize gcode:
        config.tpGen["gcode"] = []
        # the amount of tip displacement from pressure being applied to the syringe:
        config.dispenser["flexDist"] = round(config.dispenser["flexFactor"] * config.dispenser["prime"],6)
        config.tpGen["gcode"].append("M204 S500") #set accel to 500 mm/s^2
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.initTP")
        addToLog("\t" + str(ex))
    
def startDep(axes, *coords):
    # Moves to point without extruding and primes tip
    try:
        # Load in values from library:
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.motion["fastSpeed"]
        prime = config.dispenser["prime"]
        prRate = config.dispenser["prime_rate"]
        prDelay = config.dispenser["prime_delay"]

        # format/update coordinate entry appropriately:
        coords = formatCmd(axes, coords)

        # Simplify variables for GCODE assembly:
        x = round(coords[0],sigfigs)
        y = round(coords[1],sigfigs)
        z = round(coords[2] + tipHeight + flexDist,sigfigs)
        e = round(config.tpGen["coords"][-1][3],sigfigs) # fetches the most recent 'e' value

        # increments axis values appropriately:
        e = round(e + prime,8)
        if(len(config.tpGen["gcode"])==1): #if it's the beginning of toolpath
            config.tpGen["gcode"].append("G90")  # absolute motion
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
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.dispenser["speed"]

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
        e = round(e + plungerDistance(absDistance([x,y,z])),sigfigs)

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
        tipHeight = config.dispenser["work_dist"] #tip height
        flexDist = config.dispenser["flexDist"]
        speed = config.dispenser["speed"]
        retract = config.dispenser["retract"]
        retRate = config.dispenser["retract_rate"]
        retDelay = config.dispenser["retract_delay"]

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
        e = round(e + plungerDistance(absDistance([x,y,z])),sigfigs)

        config.tpGen["gcode"].append("G1 X" + str(x) + " Y" + str(y) + " Z" +
                                     str(z) + " E" + str(e) + " F" + str(speed))

        e = round(e - retract,sigfigs)
        config.tpGen["gcode"].append("G1 E" + str(e) + " F" + str(retRate))
        config.tpGen["gcode"].append("G4 P" + str(retDelay))

        z -= (tipHeight + flexDist)
        config.tpGen["coords"].append([x,y,z,e])
    except Exception as ex:
        addToLog("ERROR in ToolpathGenerator.endDep")
        addToLog("\t" + str(ex))
    
