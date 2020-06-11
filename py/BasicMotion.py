import AmaresConfig as config
import serial
import pymsgbox
import copy
from BasicComm import sendCommand, getOutput, getPos, addToLog, roundStr, sendTCP
from BasicComm import feedGCODE, doBeep
import math
import time

def defineHome():
    try:
        addToLog("Dynamic Home Definition")
        getPos()
        config.coords["dynHome"] = copy.deepcopy(config.coords["current"][:3])
        message = "Part home = " + str(config.coords["dynHome"])
        addToLog(message)
        sendTCP(message)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.defineHome")
        addToLog("\t" + str(ex))

def motionMode(mode):
    # changes and tracks motion mode
    try:
        if(mode=='absolute'):
            if(config.coords["motionMode"] != 'absolute'):
                config.coords["motionMode"] = 'absolute'
                sendCommand("G90") # Absolute motion mode
                addToLog("Motion mode = " + config.coords["motionMode"])
        elif(mode=='relative'):
            if(config.coords["motionMode"] != 'relative'):
                config.coords["motionMode"] = 'relative'
                sendCommand("G91") # Relative motion mode
                addToLog("Motion mode = " + config.coords["motionMode"])
    except Exception as ex:
        addToLog("ERROR in BasicMotion.motionMode")
        addToLog("\t" + str(ex))

def initMotors():
    # Just a simple little function to wake up the steppers
    try:
        addToLog("Commutating motors:")
        sendCommand("M17") # "Enable Steppers"
        doBeep()
    except Exception as ex:
        addToLog("ERROR in BasicMotion.initMotors")
        addToLog("\t" + str(ex))
        
def runToolpath():
    # runs the toolpath contained in "loadedTP"
    # sendCommand sets the printer to 'busy' (if a toolpath is loaded)
    # getOutput watches and returns the status of the printer (busy or ready)

    # this is where we can normalize the time elapsed between experiments:
    try:
        addToLog("Toolpath extents: " + str(config.tpGen["maxcoords"]))
        addToLog("Running toolpath:")
        sendTCP("Running toolpath...\n")
        feedGCODE(config.tpGen["gcode"])
        updateUsedVolume(config.tpGen["finalPlungerDisplacement"])
        getPos()
        config.expt['time_elapsed'] = time.time() - config.expt['finished']
        while(config.expt['time_elapsed'] < config.expt['wait_time']):
            config.expt['time_elapsed'] = time.time() - config.expt['finished']
            pass
        config.expt['finished'] = time.time()
    except Exception as ex:
        addToLog("ERROR in BasicMotion.runToolpath")
        addToLog("\t" + str(ex))

def abortToolpath():
    try:
        temp = copy.deepcopy(config.tpGen["gcode"])
        config.tpGen["gcode"] = []
        getOutput()
        getPos()
        addToLog("Toolpath aborted")
        addToLog("GCODE not sent:")
        for line in temp:
            addToLog(str(line))
        sendTCP("Toolpath aborted\n")
    except Exception as ex:
        addToLog("ERROR in BasicMotion.abortToolpath")
        addToLog("\t" + str(ex))

def plungerDistance(transDistance):
    # Calculates how much to move plunger to dispense a cylinder having
    # the same diameter as the dispensing tip and a length equal to the
    # provided translation distance
    try:
        nozzleDiam = config.dispenser["diameter"]
        nozzleRad = config.dispenser["radius"]
        plungerRad = config.dispenser["plungerRadius"]
        multiplier = config.dispenser["multiplier"]
        tipHeight = config.dispenser["work_dist"] #tip height
        if(tipHeight < nozzleDiam):
            multiplier *= (tipHeight/nozzleDiam)
        pDist = round(multiplier * (nozzleRad**2)/(plungerRad**2)*transDistance,8)
        return(pDist)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.plungerDistance")
        addToLog("\t" + str(ex))

def updateUsedVolume(plunger_distance):
    # volume in mL
    try:
        volume = plunger_distance * math.pi * config.dispenser["plungerRadius"]**2 # mm^3
        volume = volume / 1000 # convert to cm^3 or mL
        addToLog("Volume used = " + str(round(volume,6)) + " mL")
        config.tool_vars["volumeUsed"] += volume
    except Exception as ex:
        addToLog("ERROR in BasicMotion.updateUsedVolume")
        addToLog("\t" + str(ex))   

def prime(staticDelay = 0):
    # primes the tip using set prime values
    try:
        getPos()
        config.coords["current"][3] += config.dispenser["prime"]
        gcode = ("G1 F" + str(config.dispenser["prime_rate"]))
        sendCommand(gcode)
        gcode = ("G1 E" + str(config.coords["current"][3]))
        sendCommand(gcode)
        gcode = ("G4 P" + str(staticDelay + config.dispenser["prime_delay"]))
        updateUsedVolume(config.dispenser["prime"])
        sendCommand(gcode)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.prime")
        addToLog("\t" + str(ex))

def retract():
    # retracts using set retract values
    try:
        getPos()
        config.coords["current"][3] -= config.dispenser["retract"]
        gcode = ("G4 P" + str(config.dispenser["retract_delay"]))
        sendCommand(gcode)
        gcode = ("G1 F" + str(config.dispenser["retract_rate"]))
        sendCommand(gcode)
        gcode = ("G1 E" + str(config.coords["current"][3]))
        updateUsedVolume(-config.dispenser["retract"])
        sendCommand(gcode)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.retract")
        addToLog("\t" + str(ex))
    
def extrude(dist, rate):
    # moves the plunger in the positive or negative direction depending on the
    # extrusion increment and speed
    try:
        gcode = ("G1 F" + str(rate))
        sendCommand(gcode)
        if(config.status["homingDone"] == True) and (config.coords["motionMode"] == 'absolute'):
            getPos()
            config.coords["current"][3] -= dist
            gcode = ("G1 E" + str(config.coords["current"][3]))
            updateUsedVolume(-dist)
            sendCommand(gcode)
        elif(config.status["homingDone"] == False):
            if(config.coords["motionMode"] != 'relative'):
                motionMode("relative")
            gcode = ("G1 E" + str(-1*dist))
            updateUsedVolume(-dist)
            sendCommand(gcode)
        else:
            addToLog("ERROR in BasicMotion.extrude:")
            addToLog("homingDone = " + str(config.status["homingDone"]))
    except Exception as ex:
        addToLog("ERROR in BasicMotion.extrude")
        addToLog("\t" + str(ex))
    
def absDistance(coords):
    try:
        delta_x = coords[0] - config.tpGen["coords"][-1][0]
        delta_y = coords[1] - config.tpGen["coords"][-1][1]
        delta_z = coords[2] - config.tpGen["coords"][-1][2]
        distance = ((delta_x**2)+(delta_y**2)+(delta_z**2))**0.5
        return(distance)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.absDistance")
        addToLog("\t" + str(ex))

def checkOrthoJog(direct, dist):
    # checks to see if a N, W, S, or E jog would go beyond stage limits
    # returns "PASS" if the move stays within limits, "FAIL" otherwise
    try:
        if(direct == 'W'):
            if(config.coords["current"][0] - dist >= config.coords["lim_west"]):
                return("PASS")
            else:
                return("FAIL")
        elif(direct == 'E'):
            if(config.coords["current"][0] + dist <= config.coords["lim_east"]):
                return("PASS")
            else:
                return("FAIL")
        elif(direct == 'N'):
            if(config.coords["current"][1] + dist <= config.coords["lim_north"]):
                return("PASS")
            else:
                return("FAIL")
        elif(direct == 'S'):
            if(config.coords["current"][1] - dist >= config.coords["lim_south"]):
                return("PASS")
            else:
                return("FAIL")
        elif(direct == 'U') or (direct == 'D'):
            return("PASS")
        else:
            return("FAIL")
    except Exception as ex:
        addToLog("ERROR in BasicMotion.checkOrthoJog")
        addToLog("\t" + str(ex))


def checkJog(direct, dist):
    # check to see if the components of ANY jog go beyond stage limits
    # returns "PASS" if the move stays within limits, "FAIL" otherwise
    try:
        if(direct in ('N','E','S','W','U','D')):
            return(checkOrthoJog(direct,dist))
        elif(direct in ('NE','NW','SE','SW')):
            if(checkOrthoJog(direct[:1],dist) == 'PASS') and (checkOrthoJog(direct[1:],dist) == 'PASS'):
                return('PASS')
                addToLog("checkJog = PASS")
            else:
                return('FAIL')
                addToLog("checkJog = FAIL")
        else:
            return('FAIL')
            addToLog("checkJog = FAIL, direct not in N,E,S,W,U,D")
    except Exception as ex:
        addToLog("ERROR in BasicMotion.checkJog")
        addToLog("\t" + str(ex))


def checkMove(xyz):
    try:
        # ensures the new coordinates are within stage limits
        xyz = list(map(float,xyz))
        #addToLog("checkMove(" + str(xyz) + ")")
        limE = round(float(config.coords["lim_east"]),2)
        #addToLog("limE = " + str(limE))
        limW = round(float(config.coords["lim_west"]),2)
        #addToLog("limW = " + str(limW))
        limS = round(float(config.coords["lim_south"]),2)
        #addToLog("limS = " + str(limS))
        limN = round(float(config.coords["lim_north"]),2)
        #addToLog("limN = " + str(limN))
        if((limW <= xyz[0] <= limE) and (limS <= xyz[1] <= limN) and (2 <= xyz[2] <= 60)):
            addToLog("checkMove = PASS")
            return("PASS")
        else:
            addToLog("checkMove = FAIL; XYZ = " + str(xyz))
            addToLog("limE = " + str(limE) + ", limW = " + str(limW) +
                     ", limS = " + str(limS) + ", limN = " + str(limN))
            return("FAIL")
    except Exception as ex:
        addToLog("ERROR in BasicMotion.checkMove")
        addToLog("\t" + str(ex))

def doAlignOffset(coords):
    # offsets a set of coordinates for GetPos and moveTo when using alignment camera
    # takes a list of any, returns a list of string
    try:
        if (type(coords[0]) != str):
            coords = list(map(str,coords))
        if(coords[0] != ''):
            coords[0] = round(float(coords[0]) + config.tool_vars["off"][0],3)
        if(coords[1] != ''):
            coords[1] = round(float(coords[1]) + config.tool_vars["off"][1],3)
        if(coords[2] != ''):
            coords[2] = round(float(coords[2]) + config.tool_vars["off"][2],3)
        return (list(map(str,coords)))
    except Exception as ex:
        addToLog("ERROR in BasicMotion.doAlignOffset")
        addToLog("\t" + str(ex))


def moveTo(newCoords,speed=5000,useCheckMove=True): #newCoords is str, speed is int
    # newCoords is of the format "x,y,z"; commas required, values can be left blank
    # for uniaxial or biaxial motion e.g. ",2," to move y-axis only
    try:
        addToLog("moveTo " + newCoords + ", speed = " + str(speed))
        if(config.status["homingDone"] == True):
            if(newCoords.count(",")==2):
                newCoords = newCoords.replace(" ","")
                coords = newCoords.split(",")
                coords = roundStr(coords,3)
                addToLog("Align coords = " + str(coords))
                if (config.tool_vars["currentTool"] == 'alignmentCamera'):
                    coords = doAlignOffset(coords)
                if(any(coords)): # if there's at least one value
                    if (coords[0] == ''): coords[0] = str(config.coords["current"][0])
                    if (coords[1] == ''): coords[1] = str(config.coords["current"][1])
                    if (coords[2] == ''): coords[2] = str(config.coords["current"][2])
                    # append extruder position to tracked coordinates:
                    coords.append(str(config.coords["current"][3]))
                    # update tracked coordinates:
                    config.coords["current"] = list(map(float,coords))                    
                    addToLog("Tip coords = " + str(coords))
                    if(checkMove(coords)=="PASS") or (useCheckMove==False):
                        gcode1 = "G0 X" + coords[0] + " Y" + coords[1] + " F" + str(speed)
                        gcode2 = "G0 Z" + coords[2] + " F500"
                        sendCommand(gcode1)
                        sendCommand(gcode2, wait=True)
                    else:
                        addToLog("New coordinates out of range")
            else:
                addToLog("Must be of the form '  ,  ,  '\n" +
                               "(Blank values acceptable)")   
        elif(config.status["homingDone"] == False):
            addToLog("Homing Routine must be run prior to ""moveTo""")
    except Exception as ex:
        addToLog("ERROR in BasicMotion.moveTo")
        addToLog("\t" + str(ex))


def translate(direct, dist, speed, useCheckJog=True):
    # translates in a specified direction by a specified distance at a specified speed
    # The "translate" function itself is ALWAYS a relative motion function
    # If "motionMode" = "absolute" then the translate function will track coordinates
    # if "motionMode" = "relative" then coordinates are not being tracked
    # directions can be E, W, N, S, NE, SE, NW, SW, U, D
    try:
        speed = str(speed)
        dist = round(dist,2)
        if(config.status["homingDone"] == True) and (config.coords["motionMode"] == 'absolute'):
            
            if(checkJog(direct,dist) == "PASS") or (useCheckJog == False):
                
                if (direct in ('W','E')):
                    dist = max(dist, config.motion["minIncrement"])
                    if (direct == 'W'):
                        # apply appropriate displacement:
                        config.coords["current"][0] = round(config.coords["current"][0] - dist,3)
                    elif (direct == 'E'):
                        # apply appropriate displacement:
                        config.coords["current"][0] = round(config.coords["current"][0] + dist,3)
                    # generate GCODE:
                    gcode = "G1 X" + str(config.coords["current"][0]) + " F" + speed

                elif (direct in ('N','S')):
                    dist = max(dist, config.motion["minIncrement"])
                    if (direct == 'N'):
                        config.coords["current"][1] = round(config.coords["current"][1] + dist,3)
                    elif (direct == 'S'):
                        config.coords["current"][1] = round(config.coords["current"][1] - dist,3)
                    gcode = "G1 Y" + str(config.coords["current"][1]) + " F" + speed

                elif (direct in ('NE','NW','SE','SW')):
                    dist = max(dist, config.motion["minIncrement"])
                    if (direct == 'NE'):
                        # apply appropriate displacements:
                        config.coords["current"][1] = round(config.coords["current"][1] + dist,3)
                        config.coords["current"][0] = round(config.coords["current"][0] + dist,3)

                    elif (direct == 'NW'):
                        config.coords["current"][1] = round(config.coords["current"][1] + dist,3)
                        config.coords["current"][0] = round(config.coords["current"][0] - dist,3)

                    elif (direct == 'SE'):
                        config.coords["current"][0] = round(config.coords["current"][0] + dist,3)
                        config.coords["current"][1] = round(config.coords["current"][1] - dist,3)

                    elif (direct == 'SW'):
                        config.coords["current"][1] = round(config.coords["current"][1] - dist,3)
                        config.coords["current"][0] = round(config.coords["current"][0] - dist,3)
                    # generate GCODE:
                    gcode = "G1 X" + str(config.coords["current"][0]) +\
                            " Y" + str(config.coords["current"][1]) + " F" + speed

                elif (direct == 'U') or (direct == 'D'):
                    if (direct == 'U'): config.coords["current"][2] += dist
                    elif (direct == 'D'): config.coords["current"][2] -= dist
                    config.coords["current"][2] = round(config.coords["current"][2],3)
                    gcode = "G1 Z" + str(config.coords["current"][2]) + " F" + speed
            else:
                gcode = config.configuration["errorBeep"] # beep
            # send the generated GCODE:
            sendCommand(gcode, wait=False)
            
        elif(config.status["homingDone"] == False):
            if (config.coords["motionMode"] != 'relative'):
                motionMode('relative')
            # build GCODE as appropriate:
            gcode = "G0"
            if ('E' in direct):
                dist = max(dist, config.motion["minIncrement"])
                gcode = gcode + " X" + str(dist)
            if ('W' in direct):
                dist = max(dist, config.motion["minIncrement"])
                gcode = gcode + " X-" + str(dist)
            if ('N' in direct):
                dist = max(dist, config.motion["minIncrement"])
                gcode = gcode + " Y" + str(dist)
            if ('S' in direct):
                dist = max(dist, config.motion["minIncrement"])
                gcode = gcode + " Y-" + str(dist)
            if ('U' in direct):
                gcode = gcode + " Z" + str(dist)
            if ('D' in direct):
                gcode = gcode + " Z-" + str(dist)
            gcode = gcode + " F" + speed
            # send the generated GCODE:
            sendCommand(gcode)
    except Exception as ex:
        addToLog("ERROR in BasicMotion.translate")
        addToLog("\t" + str(ex))





