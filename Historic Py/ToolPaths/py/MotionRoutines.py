import time
import math
import serial
import AmaresConfig as config
import pymsgbox
from VisionUtils import *
from BasicMotion import motionMode, initMotors, doBeep, checkJog, translate, extrude, moveTo
from BasicMotion import doBeep
from BasicComm import sendCommand, getOutput, toSerBytes, getPos, getBedTemp
from BasicComm import setBedTemp, amaresConnect, amaresDisconnect, createLogFile, addToLog
from ToolpathGenerator import *

def initAmares():
    # establishes a connection with the printer and clears the initial response data
    # also initializes system
    config.init()   # initializes dictionaries
    createLogFile()
    amaresConnect()
    initMotors()    # commutates motors
    initCoords()    # sets up coordinate systems (change?)
    processLights() # turns on process camera lights
    alignmentLights() # turns on alignment camera lights
    homingCheck = pymsgbox.confirm("Perform homing routine (recommended)?","Initialize",["Yes","No"])
    if(homingCheck == "Yes"):  
        homingRoutine()

def quitAmares():
    # turns off lights, closes ports, etc.
    processLights(0)
    alignmentLights(0)
    amaresDisconnect()

def initCoords():
    try:
        # Dynamic home position (e.g. for substrates)
        config.coords["dynHome"] = config.coords["initHome"]
        addToLog("Dynamic home = " + str(config.coords["dynHome"]))
        # "Grid" home position (for printing multiple samples in a grid whose
        # origin is relative to dynHome
        config.coords["gridHome"] = [0,0,0]
        addToLog("Grid home = " + str(config.coords["gridHome"]))
        addToLog("Current offsets = " + str(config.tool_vars["off"]))
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.initCoords")
        addToLog("\t" + str(ex))    
	
def gotoHome():
    # moves the x and y axes to their home positions (if homing routine done)
    try:
        if config.status["homingDone"] == True:
            (x,y,z) = list(map(str,config.coords["dynHome"]))
            moveTo(x + "," + y + ",",config.motion["fastSpeed"])
            moveTo(",," + z)
            addToLog("Moving to home position " + str([x,y,z]))
        else:
            addToLog("Unable to move to home position")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.gotoHome")
        addToLog("\t" + str(ex))
        
def homingRoutine():
    try:
        motionMode('relative')
        config.status["homingDone"] = False
        time.sleep(0.1)
        addToLog("Running homing routine")
        sendCommand("G28") # performs the full homing routine
        # Finishes at Z = 5; we're going to TELL it it's at Z = 6 to account for an
        # unlevel build plate
        sendCommand("M400")
        sendCommand("G92 X-59 Y253 Z5")
        sendCommand("G0 F6000") # set translate speed
        # Moves to front-left of build plate:
        gcode = ("G0 X" + str(config.coords["homingOffset"][0]) +
                 " Y" + str(config.coords["homingOffset"][1]))
        sendCommand(gcode)
        sendCommand("M400")
        # Redefines TAZ coordinate system with front-left as origin:
        sendCommand("G92 X" + str(config.coords["initHome"][0]) +
                    " Y" + str(config.coords["initHome"][1]) +
                    " Z" + str(config.coords["initHome"][2]))
        # Resets software-tracked working coordinates at redefined origin:
        initCoords()
        config.status["homingDone"] = True
        motionMode('absolute')
        getPos()
        # Move 3 mm toward build plate:
        time.sleep(0.1)
        translate('D', 3, 500)
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.homingRoutine")
        addToLog("\t" + str(ex))

    
def bedLeveling():
    try:
        if(config.status["homingDone"] == True):
            motionMode('absolute')
            #addToLog("Homing Routine b/f leveling")
            #sendCommand('G28') # First need to do homing routine
            addToLog("Running auto bed leveling")
            sendCommand('G29') # Auto bed leveling
            sendCommand('M400')
            getOutput()
            sendCommand('G0 Z3.7 F500') # Raise nozzle by 3.7 mm
            # 3.7 mm is the z-offset from build plate after homing
            # and raising head by 5 mm
            # Return to home position
            sendCommand("G0 X" + str(config.coords["initHome"][0]) +
                        " Y" + str(config.coords["initHome"][1]) +
                        " F6000")
            # reassign home coordinates
            sendCommand("G92 X" + str(config.coords["initHome"][0]) +
                        " Y" + str(config.coords["initHome"][1]) +
                        " Z" + str(config.coords["initHome"][2]))
            getOutput()
            initCoords() # reinitialize home coords
            getPos() # needs to know where it is after moving around w/o tracking
            translate('D', config.coords["initHome"][2]-3, 500)
        elif(config.status["homingDone"] == False):
            addToLog("Homing Routine must be run before Auto Bed Leveling")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.bedLeveling")
        addToLog("\t" + str(ex))        

    
def changeTip():
    try:
        # moves y-axis back and z-axis up (if homing routine done)
        if(config.status["homingDone"] == True):
            addToLog("Moving to tip change position")
            # here we figure out how far we can move without hitting y-limit switch
            # From native home position, we could move 271 mm in the y without
            # hitting switch
            gcode = ("G0 Y" + str(config.coords["initHome"][1] - (271 -
                                 abs(config.coords["homingOffset"][1]))) +
                     " Z50 F5000")
            sendCommand(gcode)
        else:
            pymsgbox.alert("Homing Routine must be run before\n" +
                           "automated motion","Warning")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.changeTip")
        addToLog("\t" + str(ex))

def changeSample():
    # moves the build plate to the front to facilitate sample access
    try:
        if(config.status["homingDone"] == True):
            addToLog("Moving to change sample position")
            moveTo(",,10")
            moveTo(",230,")
        else:
           addToLog("Homing Routine must be run before automated motion")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.changeSample")
        addToLog("\t" + str(ex))
        
        
def eStop():
    # Used for emergency stopping, M112 shuts down the machine, 
    # turns off all the steppers and heaters, and if possible, 
    # turns off the power supply. 
    # A reset is required to return to operational mode.
    config.tpGen["gcode"] = [] # Clears toolpath loaded in memory
    config.status["homingDone"] = False
    sendCommand("M112") # "KILLS" motion

    addToLog("Emergency stop -- Please wait...")
    time.sleep(5)
    while getOutput()[:9] == "No output":
        pass
    addToLog("Reconnecting...")
    try:
        # The following code might work (I forget) but it takes some time (15-30 seconds) making it pointless
        config.ser_comm["lights"].setDTR(False) # Resets system; step 1
        config.ser_comm["lights"].setDTR(True) # Resets system; step 2
        config.ser_comm["taz"].setDTR(False) # Resets system; step 1
        config.ser_comm["taz"].setDTR(True) # Resets system; step 2
        sendCommand("M17") # re-enables steppers
        config.ser_comm["taz"].write(b'M502\n')
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.eStop")
        addToLog("\t" + str(ex))

def generateToolpath(tpFile):
    try:
        addToLog("Trying to generate toolpath... ")
        addToLog("Tip Diameter = " + str(config.dispenser["diameter"]))
        addToLog("Tip Length = " + str(config.dispenser["tiplength"]))
        addToLog("Extrude Mult. = " + str(config.dispenser["multiplier"]))
        addToLog("Tip Height = " + str(config.dispenser["work_dist"]))
        addToLog("Prime Amt. = " + str(config.dispenser["prime"]))
        addToLog("Retract Amt. = " + str(config.dispenser["retract"]))
        config.tpGen["maxcoords"] = [0,0,0]
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.generateToolpath")
        addToLog("\t" + str(ex))
        
    # This actually generates the toolpath list:
    try:
        with open(str(tpFile),'r') as f:
            for line in f:
                exec(line)
        addToLog("GCODE to run:")
        for line in config.tpGen["gcode"]:
            addToLog(str(line))
    except Exception as ex:
        addToLog("Toolpath File Error: tpFile = " + str(tpFile))
        addToLog("\t" + str(ex))
        doBeep()
    addToLog("Toolpath extents: " + str(config.tpGen["maxcoords"]))


def alignmentCamera():
    try:
        # Moving between tools using offset values is relative motion:
        motionMode('relative')
        addToLog("Switching to alignment camera")
        # By not using 'translate' we avoid stage soft limits but..
        # ...the working coordinates don't change...
        sendCommand("G0 Z" + str(config.tool_vars["off"][2]) + " F500")
        sendCommand("G0 X" + str(config.tool_vars["off"][0]) + " Y" +
                    str(config.tool_vars["off"][1]) +" F3000")
        if(config.status["homingDone"] == True):
            motionMode('absolute')
        config.tool_vars["currentTool"] = "alignmentCamera"
        # ...so we change the working coordinates here: 
        config.coords["current"][0] += config.tool_vars["off"][0]
        config.coords["current"][1] += config.tool_vars["off"][1]
        config.coords["current"][2] += config.tool_vars["off"][2]
        # Also need to update stage limits based on tool:
        config.coords["lim_west"] += config.tool_vars["off"][0]
        config.coords["lim_east"] += config.tool_vars["off"][0]
        config.coords["lim_south"] += config.tool_vars["off"][1]
        config.coords["lim_north"] += config.tool_vars["off"][1]
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.alignmentCamera")
        addToLog("\t" + str(ex))
    
  
def depHead():
    try:
        # Moving between tools using offset values is relative motion:
        motionMode('relative')
        addToLog("Switching to tip camera")
        # By not using 'translate' we avoid stage soft limits but..
        # ...the working coordinates don't change...
        sendCommand("G0 X" + str(-1*config.tool_vars["off"][0]) + " Y" +
                    str(-1*config.tool_vars["off"][1]) + " F3000")
        sendCommand("G0 Z" + str(-1* config.tool_vars["off"][2]) + " F500")
        if(config.status["homingDone"] == True):
            motionMode('absolute')
        config.tool_vars["currentTool"] = "depHead"
        # ...so we change the working coordinates here:
        config.coords["current"][0] -= config.tool_vars["off"][0]
        config.coords["current"][1] -= config.tool_vars["off"][1]
        config.coords["current"][2] -= config.tool_vars["off"][2]
        # Also need to update stage limits based on tool:
        config.coords["lim_west"] -= config.tool_vars["off"][0]
        config.coords["lim_east"] -= config.tool_vars["off"][0]
        config.coords["lim_south"] -= config.tool_vars["off"][1]
        config.coords["lim_north"] -= config.tool_vars["off"][1]
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.depHead")
        addToLog("\t" + str(ex))

def eastTranslate():
    # for GUI button, speed = params[0], step = params[1]
    translate('E',config.motion["increment"],config.motion["medSpeed"])
    
def westTranslate():
    # for GUI button
    translate('W',config.motion["increment"],config.motion["medSpeed"])
    
def northTranslate():
    # for GUI button
    translate('N',config.motion["increment"],config.motion["medSpeed"])
    
def southTranslate():
    # for GUI button
    translate('S',config.motion["increment"],config.motion["medSpeed"])

def northEastTranslate():
    # for GUI button
    translate('NE',config.motion["increment"],config.motion["medSpeed"])

def northWestTranslate():
    # for GUI button
    translate('NW',config.motion["increment"],config.motion["medSpeed"])

def southEastTranslate():
    # for GUI button
    translate('SE',config.motion["increment"],config.motion["medSpeed"])

def southWestTranslate():
    # for GUI button
    translate('SW',config.motion["increment"],config.motion["medSpeed"])
    
def upTranslate():
    # for GUI button
    translate('U', round(0.1*abs(config.motion["increment"]),2),config.motion["medSpeed"])
    
def downTranslate():
    # for GUI button
    translate('D', round(0.1*abs(config.motion["increment"]),2),config.motion["medSpeed"])
	
def TranslateIncrement(increment):
    addToLog("Changed jog increment to " + str(increment))
    config.motion["increment"] = increment
    
def setxyspeed(speed):
    addToLog("Changed translate speed to " + str(speed))
    config.motion["medSpeed"] = round(speed * 60,3)
    
##def setzspeed(speed):
##    addToLog("Changed Z speed to " + str(speed))
##    slowSpeed = speed



