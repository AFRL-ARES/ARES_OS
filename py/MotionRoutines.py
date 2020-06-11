import time
import math
import serial
import AmaresConfig as config
import pymsgbox
from VisionUtils import *
from BasicMotion import motionMode, initMotors, doBeep, checkJog, translate, extrude, moveTo
from BasicMotion import runToolpath
from BasicComm import *
from ToolpathGenerator import *
import socket
from random import uniform
from Tools import alignmentCamera, depHead

def initAmares():
    # establishes a connection with the printer and clears the initial response data
    # also initializes system
    config.init()   # initializes dictionaries
    # config.configuration["useLog"] = False # uncomment to turn off logging
    createLogFile()
    #createDataFile()
    amaresConnect()
    initMotors()    # commutates motors
    initCoords()    # sets up coordinate systems (change?)
    initTCP(config.TCP["port"]) # opens TCP port for Terminal
    processLights() # turns on process camera lights
    homingCheck = pymsgbox.confirm("Perform homing routine (recommended)?","Initialize",["Yes","No"])
    if(homingCheck == "Yes"):  
        homingRoutine()

def quitAmares():
    # turns off lights, closes ports, etc.
    writeUsedVolume()
    processLights(0)
    alignmentLights(0)
    amaresDisconnect()
    config.TCP["client"].send(("Closing port:" + str(config.TCP["port"])).encode())
    sock.close()

def initTCP(port, host=''):
    try:
        if not config.TCP["isInUse"]:
            config.TCP["isInUse"] = True
            sock = socket.socket()
            sock.bind((host,port))
            addToLog("Bound port " + str(port) + " for Terminal")
            sock.listen(1)
            config.TCP["client"],config.TCP["addr"] = sock.accept()
            config.TCP["isInUse"] = False
        elif config.TCP["isInUse"]:
            addToLog("ERROR: Terminal already in use")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.initTCP")
        addToLog("\t" + str(ex))
    

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
        # First, load stored mesh bed leveling information:
        message = "Loading bed leveling mesh from EEPROM..."
        addToLog(message)
        sendTCP(message)
        clearBuffer()
        sendCommand('M501')
        getOutput(timeout = 1)

        # Then do homing routine:
        motionMode('relative') #G91
        config.status["homingDone"] = False
        config.tool_vars["currentTool"] = "depHead"
        time.sleep(0.1)
        message = "Running homing routine"
        addToLog(message)
        sendTCP(message)
        sendCommand("G28", wait=False) # performs the full homing routine (finishes at Z = 5)
        sendCommand("M400") # waits for homing to complete
        config.coords["current"] = getRawPos()
        sendCommand("G0 Z8.5 F800",wait=True) # Moves the tip up to clear thick secondary substrates
        #changeCoords([-59,253,10])
        sendCommand("G0 F6000") # set translate speed
        # Moves to front-left of build plate:
        gcode = ("G0 X" + str(config.coords["homingOffset"][0]) +
                 " Y" + str(config.coords["homingOffset"][1]))
        sendCommand(gcode, wait=True)
        sendCommand("G0 Z-8.5 F600", wait=True) # moves the tip back down
        getPos()
        # Redefines TAZ coordinate system with front-left as origin:
        changeCoords(config.coords["initHome"][0:3])
        # Resets software-tracked working coordinates at redefined origin:
        initCoords()
        config.status["homingDone"] = True
        motionMode('absolute') #G90
        getPos()

        # Move 5 mm toward build plate:
        time.sleep(0.1)
        translate('D', 5, 500)

        # Then activate bed leveling mesh:
        message = "Activating bed leveling mesh..."
        addToLog(message)
        sendCommand('M851 Z-4.2')
        sendCommand('M420 S1')
        getOutput(timeout = 10)
        sendCommand('G29 S0')
        time.sleep(2)
        getOutput(timeout = 10)
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.homingRoutine")
        addToLog("\t" + str(ex))

    
def bedLeveling():
    try:
        if(config.status["homingDone"] == True):
            pass
        elif(config.status["homingDone"] == False):
            addToLog("Homing Routine must be run before Auto Bed Leveling")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.bedLeveling")
        addToLog("\t" + str(ex))        

    
def changeTip():
    try:
        # moves tip beyond edge of build plate
        # switches to alignment camera to bypass tip stage limits
        if(config.status["homingDone"] == True):
            addToLog("Moving to tip change position")
            alignmentCamera()
            moveTo("10.5,0,")
        else:
            pymsgbox.alert("Homing Routine must be run before\n" +
                           "automated motion","Warning")
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.changeTip")
        addToLog("\t" + str(ex))

def cleanTip():
    try:
        if (config.tool_vars["currentTool"] == 'alignmentCamera'):
            sendTCP("Deposition head must be current tool to\n run tip cleaning")
            doBeep(2000,50,3)
        else:
            cleanXYZ = list(map(str, config.tipCleaning["cleanXYZ"]))
            #translate('U',1,800)
            returnDest = getPos()[:3]
            safeZ = str(returnDest[2] + 2)
            zStep = 0.2
            xyStep = 0.5
            lateralSpeed = 1000
            x = str(float(cleanXYZ[0]))
            y = str(float(cleanXYZ[1]) + uniform(-30,30))
            z = cleanXYZ[2]
            addToLog("Cleaning tip at [" + x + "," + y + "," + z + "]")
            moveTo(',,'+safeZ,800)
            moveTo(x+','+y+',',5000,useCheckMove=False)
            moveTo(',,'+z,800,useCheckMove=False)
            # set home position for toolpath:
            config.coords["dynHome"] = getPos()[:3]
            # generate toolpath (all coords relative to dynHome):
            initTP()
            config.tpGen["gcode"].append("# TIP CLEANING CODE:")    # flag to omit logging
            noDepToPoint('X',-xyStep)    # move away from center
            for j in range(1,5):
                for k in range(1,35):
                    # x = center_x + radius * cosine(angle)
                    circle_x = xyStep * math.cos(math.radians(10*k))
                    # y = center_y + radius * sine(angle)
                    circle_y = xyStep + math.sin(math.radians(10*k))
                    noDepToPoint('XYZ',circle_x,circle_y,-zStep*(j-1)+(-zStep/36)*k)
            for j in range(1,20):
                for k in range(1,35):
                    # x = center_x + radius * cosine(angle)
                    newXYStep = xyStep + ((j-1)*0.03) + (0.03/36)*k
                    circle_x = newXYStep * math.cos(math.radians(10*k))
                    # y = center_y + radius * sine(angle)
                    circle_y = newXYStep + math.sin(math.radians(10*k))
                    noDepToPoint('XYZ',circle_x,circle_y,zStep*(j-6)+(zStep/36)*k)
            runToolpath()
            moveTo(',,'+safeZ,800,useCheckMove=False)
            alignmentCamera(moveByOffset=False) # switches to alignment camera without moving
            moveTo(str(returnDest[0])+','+str(returnDest[1])+',',5000,useCheckMove=False)
            moveTo(',,'+str(returnDest[2]),800)
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.cleanTip")
        addToLog("\t" + str(ex))

def changeSample():
    # moves the build plate to the front to facilitate sample access
    try:
        if(config.status["homingDone"] == True):
            translate("U", 10, 500)
            pos = str(config.coords["lim_north"]-1)
            moveTo(',230,',useCheckMove=False)
            #sendCommand("M400")
            getRawPos()
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
    # intialize:   
    try:
        addToLog("Trying to generate toolpath... ")
        addToLog("Tip Diameter = " + str(config.dispenser["diameter"]))
        addToLog("Tip Length = " + str(config.dispenser["tiplength"]))
        addToLog("Extrude Mult. = " + str(config.dispenser["multiplier"]))
        addToLog("Tip Height = " + str(config.dispenser["work_dist"]))
        addToLog("Prime Amt. = " + str(config.dispenser["prime"]))
        addToLog("Retract Amt. = " + str(config.dispenser["retract"]))
        sendTCP("Toolpath generating...")
        config.tpGen["maxcoords"] = [0,0,0]
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.generateToolpath")
        addToLog("\t" + str(ex))
        
    # This actually generates the toolpath list:
    try:
        with open(str(tpFile),'r') as f:
            for line in f:
                try:
                    exec(line)
                except:
                    addToLog("Problem with toolpath line: " + str(line))
        addToLog("GCODE to run:")
        addToLog("******************************************")
        volume = (config.tpGen["finalPlungerDisplacement"] * math.pi *
                 config.dispenser["plungerRadius"]**2)/1000
        addToLog(str(volume) + " mL of material required\n")
        for line in config.tpGen["gcode"]:
            addToLog(str(line))
            if("TIP CLEANING" in line):
                addToLog('<OMITTED>')
                break
        addToLog("******************************************")
        f.close()
    except Exception as ex:
        addToLog("Toolpath File Error: tpFile = " + str(tpFile))
        addToLog("\t" + str(ex))
        doBeep()
    
    # create a temporary JSON file for current experiment parameters:
    # (for real-time plotter)
    try:
        if(createCampaignDataPath()):
            jsonfile = config.paths["campaigndatapath"] + "CurrentExp.json"
            newDataList = createNewDataList()
            PlannedParams = mapParams(config.data["PlannedParams"])
            addToLog("PlotterPlannedParams = " + str(PlannedParams))
            values = []
            for param in PlannedParams:
                addToLog("param: " + str(param))
                addToLog("config.data['ParamNames'].index(param) = " + str(config.data['ParamNames'].index(param)))
                values.append(newDataList[config.data['ParamNames'].index(param)])
                addToLog("Values are: " + str(values))
            data = {
                'ParamNames': PlannedParams,
                'Values': values
                }
            with open(jsonfile, 'w') as outfile:
                outfile.write(json.dumps(data, sort_keys=False))
    except Exception as ex:
        addToLog("Exception in MotionRoutines.generateToolpath -> expt curr. params.")
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



