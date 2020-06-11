import AmaresConfig as config
import serial
import time
import numpy
import cv2
from datetime import datetime
import os.path
from operator import add, sub


def checkConnection(port):
    if(port.isOpen() == False):
        try:
            port.open()
            time.sleep(2) # Wait for connection
            addToLog('Trying to open ' + str(port))
        except Exception as ex:
            addToLog('Failed to open ' + str(port))
            addToLog("\t" + str(ex))
            return("Failed to open port")
        
def closeConnection(port):
    if(port.isOpen() == True):
        port.close()

def createLogFile():
    # check for file named with today's date
    # if no file, create it
    logname = time.strftime("%Y-%m-%d-%H%M%S")
    config.paths["logfile"] = config.paths["logpath"] + logname + ".log"
    config.status["logindex"] = 0
    if(not os.path.isfile(config.paths["logfile"])):
        config.paths["log"] = open(config.paths["logfile"],"w")

def addToLog(data):
    if (config.configuration["useLog"] == True):
        if((data != ">>>  b'ok\\n'") or ((data == ">>>  b'ok\\n'") and (config.status["prevLogEntry"] != ">>>  b'ok\\n'"))):
            logtime = datetime.now().strftime("%H:%M:%S.%f")
            config.status["prevLogEntry"] = data # used to filter repeating log data (see getOutput)
            text = logtime + "  " + data + "\n"
            config.status["logindex"] += (len(text) + 6) # the index itself is 6 chars
            config.status["logindex"] += 1 # newline character
            logindex = str(config.status["logindex"]).zfill(5) + " "
            config.paths["log"].write(logindex + text)
            config.paths["log"].flush()
    else:
        pass

def doBeep(frequency=2500, duration=45):
    # makes the printer make a beep sound
    try:
        config.ser_comm["taz"].write(toSerBytes('M300 P' + str(duration) + ' S' + str(frequency)+ '\n'))
    except Exception as ex:
        addToLog("ERROR in BasicComm.doBeep")
        addToLog("\t" + str(ex))
        
def getOutput(timeout=0.05):
    # gets (and clears) all available serial output bits
    try:
        output = []
        start = time.time() # function timer timestamp
        while(len(output)==0): # makes sure to wait until there is an output
            if(config.ser_comm["taz"].in_waiting > 0): # if there's an output waiting
                while(config.ser_comm["taz"].in_waiting > 0): # read all lines of output
                    output.append(config.ser_comm["taz"].readline())
                    addToLog(">>>  " + str(output[-1]))
                if(config.status["printerStatus"] == 'busy'):
                    config.status["printerStatus"] = 'ready'
            if((time.time()-start) > float(timeout)): # if no output in allotted time
                if(config.status["prevLogEntry"] != "No output available at this time"):
                    addToLog("No output available at this time")
                break
        return(output)
    except Exception as ex:
        addToLog("ERROR in BasicComm.getOutput")
        addToLog("\t" + str(ex))

def waitForOK(log=False):
    # for use with feedGCODE
    # waits for printer to return any output (used to be "b'ok\n'")
    try:
        output = []
        while True:
            #while(len(output)==0): # makes sure to wait until there is an output
            if(config.ser_comm["taz"].in_waiting > 0): # if there's an output waiting
                while(config.ser_comm["taz"].in_waiting > 0): # read all lines of output
                    output.append(config.ser_comm["taz"].readline())
                if(config.status["printerStatus"] == 'busy'):
                    config.status["printerStatus"] = 'ready'
                if log:
                    addToLog(">>>  " + str(output[-1]))
                    addToLog("Wait complete")
                break
    except Exception as ex:
        addToLog("ERROR in BasicComm.waitForOK")
        addToLog("\t" + str(ex))


def toSerBytes(somestring):
    # Formatting - Converts a string to bytes and adds prefix and suffix to send to printer
    try:
        somebytes = bytes("b'" + somestring +"\n'", 'utf-8')
        return(somebytes)
    except Exception as ex:
        addToLog("ERROR in BasicComm.toSerBytes")
        addToLog("\t" + str(ex))


def sendCommand(somegcode, wait=False, log=False, listenTime=0.05):
    # Takes a string of gcode, converts it and sends it to printer
    # If wait=True, function runs until motion is complete and printer returns
    # 'b\ok\n'
    try:
        addToLog("sendCommand:  " + somegcode + " wait=" + str(wait))
        config.ser_comm["taz"].write(toSerBytes(somegcode))
        if wait:
            getOutput(timeout=listenTime)
            config.ser_comm["taz"].write(toSerBytes("M400"))
            config.ser_comm["taz"].write(toSerBytes("G0"))
            waitForOK(log)
    except Exception as ex:
        addToLog("ERROR in BasicComm.sendCommand")
        addToLog("\t" + str(ex))

def feedGCODE(GCODE):
    try:
        while(len(GCODE) > 0):
            getOutput() # Checks (waits) for printer response and sets status accordingly
            if(len(GCODE) > 0) and (config.status["printerStatus"] == 'ready'):
                sendCommand(GCODE[0])
                config.status["printerStatus"] = 'busy'
                del GCODE[0] # removes first line
    except Exception as ex:
        addToLog("ERROR in BasicMotion.feedGCODE")
        addToLog("\t" + str(ex))

def sendTCP(string):
    try:
        string += "\n"
        if not config.TCP["isInUse"]:
            config.TCP["isInUse"] = True
            config.TCP["client"].send(string.encode())
            config.TCP["isInUse"] = False
    except Exception as ex:
        addToLog("ERROR in BasicComm.sendTCP")
        addToLog("\t" + str(ex))
    

def getTCP():
    response = config.TCP["client"].recv(1024).decode('utf-8')
    return(response)

def getBedTemp():
    # queries the printer for build plate temperature
    try:
        start = time.time()
        getOutput() # gets (and clears) output bits
        sendCommand("M105") # "Report Temperatures" command
        response = config.ser_comm["taz"].readline().decode() # ".decode()" converts to str
        temperature = response[(response.find("B")+2):(response.find("B")+2+5)]
        temperature = temperature.replace(" ","")
        end = time.time()
        computationTime = round((end-start)*1000,3)
        return(temperature)
    except Exception as ex:
        addToLog("ERROR in BasicComm.getBedTemp")
        addToLog("\t" + str(ex))


def setBedTemp(temperature):
    # sets the build plate temperature M140 S80
    try:
        sendCommand("M140 S" + str(temperature))
        time.sleep(0.1)
    except Exception as ex:
        addToLog("ERROR in BasicComm.setBedTemp")
        addToLog("\t" + str(ex))


def roundStr(string,num=3):
    # takes a list of strings of numbers and rounds them to num digits
    try:
        newstring = []
        for val in string:
            if val: # if there is a string
                newval = str(round(float(val),num))
                newstring.append(newval)
            elif not val: # if the string is empty
                newstring.append(val)
        return (newstring)
        addToLog("newstring = " + str(newstring))
    except Exception as ex:
        addToLog("ERROR in BasicComm.roundStr")
        addToLog("\t" + str(ex))

def getRawPos():
        try:
            addToLog("Requesting position:")
            getOutput()
            sendCommand("M114") # "Get Current Position" command
            while True:
                RawPosData = config.ser_comm["taz"].readline().decode() # ".decode()" converts to str
                if RawPosData[:2] == "X:": # when the coordinates string is found
                    PosList = RawPosData.split(" ")
                    xPos = PosList[0][2:]
                    yPos = PosList[1][2:]
                    zPos = PosList[2][2:]
                    ePos = PosList[3][2:]
                    break
            getOutput() # clear the rest of the output buffer
            positions = (list(map(float, (xPos, yPos, zPos, ePos))))
            return [round(val,3) for val in positions]
        except Exception as ex:
            addToLog("ERROR in BasicComm.getRawPos")
            addToLog("\t" + str(ex))

def getPos():
    # queries the printer for all axes positions and assigns returned
    # values to config.coords["current_?"]
    try:
        config.coords["current"] = getRawPos()
        config.coords["current"] = [round(val,3) for val in config.coords["current"]]
        addToLog("Current coords: " + str(config.coords["current"]))
        return (config.coords["current"])
    except Exception as ex:
        addToLog("ERROR in BasicComm.getPos")
        addToLog("\t" + str(ex))

def amaresConnect():
    try:
        config.ser_comm["taz"].open()
        time.sleep(2)   # Wait for connection
        config.ser_comm["taz"].write(b'M502\n') # Applies correct setting for syringe extruder based on values stored in firmware
        getOutput(timeout=5)     # reads and clears printer's output buffer
    except Exception as ex:
        addToLog("ERROR in BasicComm.amaresConnect")
        addToLog("\t" + str(ex))

def amaresDisconnect():
    try:
        config.ser_comm["taz"].close()
        config.ser_comm["lights"].close()
        addToLog("System disconnected")
        config.paths["log"].close()
    except Exception as ex:
        addToLog("ERROR in BasicComm.amaresDisconnect")
        addToLog("\t" + str(ex))

def GetXPos():
    try:
        try:
            config.coords["current"][0]
        except:
            return(0)
        else:
            if (config.tool_vars["currentTool"] == "depHead"):
                return(config.coords["current"][0])
            elif (config.tool_vars["currentTool"] == "alignmentCamera"):
                return(round(config.coords["current"][0] - config.tool_vars["off"][0],3))
        
    except Exception as ex:
        addToLog("ERROR in BasicComm.GetXPos")
        addToLog("\t" + str(ex))
	
def GetYPos():
    try:
        try:
            config.coords["current"][1]
        except:
            return(0)
        else:
            if (config.tool_vars["currentTool"] == "depHead"):
                return(config.coords["current"][1])
            elif (config.tool_vars["currentTool"] == "alignmentCamera"):
                return(round(config.coords["current"][1] - config.tool_vars["off"][1],3))

    except Exception as ex:
        addToLog("ERROR in BasicComm.GetYPos")
        addToLog("\t" + str(ex))

def GetZPos():
    try:
        try:
            config.coords["current"][2]
        except Exception as ex:
            return(0)
        else:
            if (config.tool_vars["currentTool"] == "depHead"):
                return (round(config.coords["current"][2],3))
            elif (config.tool_vars["currentTool"] == "alignmentCamera"):
                return (round(config.coords["current"][2] - config.tool_vars["off"][2],3))
            
    except Exception as ex:
        addToLog("ERROR in BasicComm.GetZPos")
        addToLog("\t" + str(ex))
	
def GetEPos():
    try:
        try:
            config.coords["current"]
        except:
            return(0)
        else:
            return(round(config.coords["current"][3],6))
    except Exception as ex:
        addToLog("ERROR in BasicComm.GetEPos")
        addToLog("\t" + str(ex))
        
