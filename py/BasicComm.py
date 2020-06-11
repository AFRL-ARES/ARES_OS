import AmaresConfig as config
import serial
import time
from datetime import datetime
import numpy
import cv2
from datetime import datetime
import os.path
from operator import add, sub
import csv, requests
import simplejson as json


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
            logtime = datetime.now().strftime("%H:%M:%S.%f")[:-3]
            config.status["prevLogEntry"] = data # used to filter repeating log data (see getOutput)
            text = logtime + "  " + data + "\n"
            config.status["logindex"] += (len(text) + 6) # the index itself is 6 chars
            config.status["logindex"] += 1 # newline character
            logindex = str(config.status["logindex"]).zfill(5) + " "
            #config.paths["log"].write(logindex + text)
            config.paths["log"].write(text)
            config.paths["log"].flush()
    else:
        pass

def readParameterLimits():
    try:
        # data mapping of selected parameter limits to indexed lists:
        f = open(config.paths["pypath"] + "ParameterLimits.json","r")
        jsondata = json.load(f)
        f.close()
        MinVals = [0]*len(config.data["ParamNames"])
        MaxVals = [0]*len(config.data["ParamNames"])
        for element in jsondata["Limits"]:
            if element["Name"] == "dispenser.work_dist":
                MinVals[config.data['ParamNames'].index("Working Distance")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Working Distance")] = element["Max"]
            elif element["Name"] == "dispenser.prime":
                MinVals[config.data['ParamNames'].index("Prime Distance")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Prime Distance")] = element["Max"]
            elif element["Name"] == "dispenser.multiplier":
                MinVals[config.data['ParamNames'].index("Extrusion Multiplier")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Extrusion Multiplier")] = element["Max"]
            elif element["Name"] == "dispenser.prime_rate":
                MinVals[config.data['ParamNames'].index("Prime Rate")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Prime Rate")] = element["Max"]
            elif element["Name"] == "dispenser.prime_delay":
                MinVals[config.data['ParamNames'].index("Prime Delay")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Prime Delay")] = element["Max"]
            elif element["Name"] == "dispenser.retract":
                MinVals[config.data['ParamNames'].index("Retract Distance")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Retract Distance")] = element["Max"]
            elif element["Name"] == "dispenser.retract_rate":
                MinVals[config.data['ParamNames'].index("Retract Rate")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Retract Rate")] = element["Max"]
            elif element["Name"] == "dispenser.speed":
                MinVals[config.data['ParamNames'].index("Print Speed")] = element["Min"]
                MaxVals[config.data['ParamNames'].index("Print Speed")] = element["Max"]
        MinVals[config.data['ParamNames'].index("Result Value")] = 0
        MaxVals[config.data['ParamNames'].index("Result Value")] = float(config.data["Target"])
        config.data["MinVals"] = MinVals
        config.data["MaxVals"] = MaxVals
    except Exception as ex:
        addToLog("Exception in BasicComm.readParameterLimits:")
        addToLog("\t" + str(ex))

def mapParams(Params):
    # maps C# parameter names to Python versions
    try:
        if not type(Params) == list:
            ParamsList = list(Params.split(","))
        else:
            ParamsList = Params
        addToLog("Planned Params = " + str(ParamsList))
        mappedList = []
        for Param in ParamsList:
            if Param == "dispenser.work_dist":
                mappedList.append("Working Distance")
            elif Param == "dispenser.prime":
                mappedList.append("Prime Distance")
            elif Param == "dispenser.multiplier":
                mappedList.append("Extrusion Multiplier")
            elif Param == "dispenser.prime_rate":
                mappedList.append("Prime Rate")
            elif Param == "dispenser.prime_delay":
                mappedList.append("Prime Delay")
            elif Param == "dispenser.retract":
                mappedList.append("Retract Distance")
            elif Param == "dispenser.retract_rate":
                mappedList.append("Retract Rate")
            elif Param == "dispenser.speed":
                mappedList.append("Print Speed")
            elif Param == "Prime Delay":
                mappedList.append("prime_delay")
            elif Param == "Working Distance":
                mappedList.append("work_dist")
            elif Param == "Prime Distance":
                mappedList.append("prime")
            elif Param == "Extrusion Multiplier":
                mappedList.append("multiplier")
            elif Param == "Prime Rate":
                mappedList.append("prime_rate")
            elif Param == "Retract Distance":
                mappedList.append("retract")
            elif Param == "Retract Rate":
                mappedList.append("retract_rate")
            elif Param == "Print Speed":
                mappedList.append("speed")
            #incomplete
        return(mappedList)
    except Exception as ex:
        addToLog("Exception in BasicComm.mapParams:")
        addToLog("\t" + str(ex))


def writeUsedVolume():
    # updates volume of ink used for current syringe
    try:
        f = open(config.paths["pypath"] + "tool_params.json","r+")
        data = json.load(f)
        data["volumeUsed"] = config.tool_vars["volumeUsed"]
        data["alignLightVal"] = config.tool_vars["alignLightVal"]
        data["procLightVal"] = config.tool_vars["procLightVal"]
        # clear old contents:
        f.seek(0)       
        f.truncate()
        # write new contents:
        f.write(json.dumps(data, indent=4, sort_keys=True))
        f.close()
    except Exception as ex:
        addToLog("Exception in BasicComm.writeUsedVolume:")
        addToLog("\t" + str(ex))

def createTransferDirectory():
    try:
        dataname = time.strftime("%Y-%m-%d-%H%M%S") + " BORAAS"
        config.paths["transferpath"] = config.paths["transferParent"] + dataname + "\\"
        if(not os.path.exists(config.paths["transferpath"])):
            os.makedirs(config.paths["transferpath"])
    except Exception as ex:
        addToLog("Exception in BasicComm.createTransferDirectory:")
        addToLog("\t" + str(ex))

def createCampaignDataPath():
    # checks for a campaign data directory, creates it if it doesn't exist
    if(config.data["Expt#"] == 1):
        config.paths["dataset"] = time.strftime("%Y-%m-%d-%H%M%S")
        config.paths["campaigndatapath"] = config.paths["datapath"] + config.paths["dataset"] + "\\"
        if(not os.path.isdir(config.paths["campaigndatapath"])):
            os.mkdir(config.paths["campaigndatapath"])
        config.data["History"] = [] # resets history list for new campaign
        config.data["VectorHistory"] = [] # resets vector history list for new campaign
        return(True)
    
def createDataFile():
    # creates datafiles and directories as appropriate; executed by analyzer
    # when Expt# = 1
    try:
        # check for file named with today's datetime, if no file, create it
        createCampaignDataPath()
        config.paths["datafile"] = config.paths["campaigndatapath"] + config.paths["dataset"] + ".csv"
        if(not os.path.isfile(config.paths["datafile"])):
            config.paths["data"] = open(config.paths["datafile"],mode='w',newline='')
            writer = csv.DictWriter(config.paths["data"], fieldnames=config.data["ParamNames"])
            writer.writeheader() #header values are from 'fieldnames'
        config.paths["data"].close()
        if(config.data["Planner"] == "BORAASWeb"):
            createTransferDirectory()
    except Exception as ex:
        addToLog("Exception in BasicComm.createDataFile:")
        addToLog("\t" + str(ex))

def createNewDataList():
    # create a new parameter list for experiment:
    # ** There is a more concise way to organize all of these parameter mappings...
    try:
        newDataList = [0]*len(config.data['ParamNames'])
        newDataList[config.data['ParamNames'].index("Expt#")] = config.data["Expt#"]
        newDataList[config.data['ParamNames'].index("Date")] = config.data["expt_date"]
        newDataList[config.data['ParamNames'].index("Time")] = config.data["expt_time"]
        newDataList[config.data['ParamNames'].index("Location")] = config.coords["dynHome"]
        newDataList[config.data['ParamNames'].index("Result Descriptor")] = config.data['Result Descriptor']
        newDataList[config.data['ParamNames'].index("Result Value")] = config.data["Result Value"]
        newDataList[config.data['ParamNames'].index("Result Unit")] = config.data["Result Unit"]
        newDataList[config.data['ParamNames'].index("Nozzle Diameter")] = config.dispenser["diameter"]
        newDataList[config.data['ParamNames'].index("Nozzle Length")] = config.dispenser["tiplength"]
        newDataList[config.data['ParamNames'].index("Extrusion Multiplier")] = config.dispenser["multiplier"]
        newDataList[config.data['ParamNames'].index("Working Distance")] = config.dispenser["work_dist"]
        newDataList[config.data['ParamNames'].index("Prime Distance")] = config.dispenser["prime"]
        newDataList[config.data['ParamNames'].index("Prime Rate")] = config.dispenser["prime_rate"]
        newDataList[config.data['ParamNames'].index("Prime Delay")] = config.dispenser["prime_delay"]
        newDataList[config.data['ParamNames'].index("Retract Distance")] = config.dispenser["retract"]
        newDataList[config.data['ParamNames'].index("Retract Rate")] = config.dispenser["retract_rate"]
        newDataList[config.data['ParamNames'].index("Print Speed")] = config.dispenser["speed"]
        newDataList[config.data['ParamNames'].index("Bed Temperature")] = config.data['bedTemp']
        newDataList[config.data['ParamNames'].index("Enclosure Temperature")] = config.data['enclosureTemp']
        newDataList[config.data['ParamNames'].index("Substrate")] = config.data['substrate']
        newDataList[config.data['ParamNames'].index("Analyzer")] = config.data['Analyzer']                                                                              
        newDataList[config.data['ParamNames'].index("Analyzer Parameters")] = config.data['Analyzer Parameters']
        newDataList[config.data['ParamNames'].index("Analysis Light")] = config.tool_vars['alignLightVal']
        newDataList[config.data['ParamNames'].index("Process Light")] = config.tool_vars['procLightVal']
        newDataList[config.data['ParamNames'].index("Material")] = config.dispenser['material']
        newDataList[config.data['ParamNames'].index("Initial Volume")] = config.tool_vars['initVolume']
        newDataList[config.data['ParamNames'].index("Volume Used")] = config.tool_vars['volumeUsed']
        newDataList[config.data['ParamNames'].index("Time Elapsed")] = config.expt['time_elapsed']
        newDataList[config.data['ParamNames'].index("Wait Time")] = config.expt['wait_time']
        newDataList[config.data['ParamNames'].index("Relative Humidity")] = config.data['Relative Humidity']
        newDataList[config.data['ParamNames'].index("Barometric Pressure")] = config.data['Barometric Pressure']
        newDataList[config.data['ParamNames'].index("Syringe Date")] = config.tool_vars['syringeDate']
        newDataList[config.data['ParamNames'].index("Syringe Time")] = config.tool_vars['syringeTime']
        newDataList[config.data['ParamNames'].index("Planner")] = config.data['Planner']
        return(newDataList)
    except Exception as ex:
        addToLog("Exception in BasicComm.createNewDataList:")
        addToLog("\t" + str(ex))

def createNewVector(parent_list, parent_header):
    try:
        # extracts select numerical vector elements from parent_list
        # "PlannedParams" comes from C# ARES code
        vector_params = mapParams(config.data["PlannedParams"]) + ["Result Value"]
        addToLog("vector_params = " + str(vector_params))
        newVector = [0]*len(vector_params)
        for indx, param in enumerate(vector_params):
            newVector[indx] = parent_list[parent_header.index(param)]
        return(newVector)
    except Exception as ex:
        addToLog("Exception in BasicComm.createNewVector:")
        addToLog("\t" + str(ex))

def postToBORAAS(data):
    try:
        startTime = time.time()
        addToLog("postToBORAAS.data = " + str(data))
        res = requests.post('https://phoneapp-planner.mybrainstudy.com/new_design',json=data)
        new_design = res.json()
        addToLog("Returned: " + str(new_design))
        addToLog("Execution time = " + str(round(time.time()-startTime,3)) + " s")
        return(round(time.time()-startTime,3), new_design)
    except Exception as ex:
        addToLog("Exception in BasicComm.postToBORAAS:")
        addToLog("\t" + str(ex))

def loadBORAASDesign(design):
    try:
        resultfile = config.paths["pypath"] + "BORAAS_Results.xfr" # for passing values back to C#
        with open(resultfile, 'w') as outfile:
            #outfile.write(str(config.dispenser["prime_delay"])) # static parameter for now
            outfile.write(str(design["Values"]))
        addToLog("New Design Values = " + str(design["Values"]))
    except Exception as ex:
        addToLog("Exception in BasicComm.loadBORAASDesign:")
        addToLog("\t" + str(ex))
    
        
def addData():
    # initialize
    try:
        startTime = time.time()
        addToLog("Recording data for expt #" + str(config.data["Expt#"]) + "...")
        config.data["expt_date"] = time.strftime("%Y-%m-%d")
        config.data["expt_time"] = datetime.now().strftime("%H:%M:%S.%f")
        readParameterLimits() # read in parameter limits
        # gather new data:
        newDataList = createNewDataList()       # for full "metadata" record
        newVector = createNewVector(newDataList,config.data["ParamNames"])   # for ML planners
        addToLog("newVector = " + str(newVector))
        config.data['bedTemp'], config.data['enclosureTemp'] = getBedTemp()
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> initialize")
        addToLog("\t" + str(ex))
        
    # add CSV data to CSV file
    try:
        # make dictionary for csv:
        csvDict = {}
        for name in config.data["ParamNames"]:
            csvDict[name] = newDataList[config.data['ParamNames'].index(name)]
        with open(config.paths["datafile"],'a',newline='') as csv_file:
            writer = csv.DictWriter(csv_file, fieldnames = config.data["ParamNames"])
            writer.writerow(csvDict)
            csv_file.close()
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> CSV")
        addToLog("\t" + str(ex))

    # update history:
    try:
        # add new data as a list element in 'History' list:
        config.data["History"].append(newDataList)
        config.data["VectorHistory"].append(newVector)
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> update history")
        addToLog("\t" + str(ex))

    # create a JSON file for experiment record:
    try:
        jsonfile = config.paths["campaigndatapath"] + str(int(config.data["Expt#"])) + ".json"
        data = {
            'ParamNames': config.data["ParamNames"],
            'History': config.data["History"],
            'SolveFor': ["Result Value"],
            'Target': config.data["Target"],
            'MinVals': config.data["MinVals"],
            'MaxVals': config.data["MaxVals"]
            }
        with open(jsonfile, 'w') as outfile:
            outfile.write(json.dumps(data, sort_keys=False))
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> expt JSON")
        addToLog("\t" + str(ex))

    # create a JSON file for experiment vectors:
    try:
        jsonfile = config.paths["campaigndatapath"] + "Vect_" + str(int(config.data["Expt#"])) + ".json"
        addToLog("config.data['PlannedParams'] = " + str(config.data['PlannedParams']))
        vector_params = mapParams(config.data["PlannedParams"]) + ["Result Value"]
        data = {
            'ParamNames': vector_params,
            'History': config.data["VectorHistory"],
            'SolveFor': ["Result Value"], # Parameter name list
            #'SolveFor': [vector_params.index(param) for param in mapParams(config.data["SolveFor"])], # Indices
            'MinVals': [config.data["MinVals"][config.data["ParamNames"].index(param)] for param in vector_params],
            'MaxVals': [config.data["MaxVals"][config.data["ParamNames"].index(param)] for param in vector_params]
            }
        with open(jsonfile, 'w') as outfile:
            outfile.write(json.dumps(data, sort_keys=False))
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> vect JSON")
        addToLog("\t" + str(ex))

    # send and receive data to BORAAS Web Planner:
    try:
        if(config.data["Planner"] == "BORAASWeb"):
            if(config.data["Expt#"] >= 3):  # BORAAS needs at least three data points
                data['transfer_time'], new_data = postToBORAAS(data)
                loadBORAASDesign(new_data)
                # make a record of file sent:
                jsonfile = config.paths["transferpath"] + "Vect_" + str(int(config.data["Expt#"])) + ".json"
                with open(jsonfile, 'w') as outfile:
                    outfile.write(json.dumps(data))
                # make a record of file received:
                responsefile = config.paths["transferpath"] + "newVect_" + str(int(config.data["Expt#"])) + ".json"
                with open(responsefile, 'w') as outfile:
                    outfile.write(json.dumps(new_data))
                # save received file in campaign directory:
                responsefile = config.paths["campaigndatapath"] + "newVect_" + str(int(config.data["Expt#"])) + ".json"
                with open(responsefile, 'w') as outfile:
                    outfile.write(json.dumps(new_data))
    except Exception as ex:
        addToLog("Exception in BasicComm.addData -> BORAAS")
        addToLog("\t" + str(ex))

    addToLog("Data recorded (" + str(round(time.time() - startTime,3)) + " s)")


def doBeep(frequency=2200, duration=200, repeat=1):
    # makes the printer make a beep sound
    try:
        for i in range(0,repeat):
            config.ser_comm["taz"].write(toSerBytes('M300 P' + str(duration) + ' S' + str(frequency)+ '\n'))
            config.ser_comm["taz"].write(toSerBytes('G4 P' + str(duration)))
    except Exception as ex:
        addToLog("ERROR in BasicComm.doBeep")
        addToLog("\t" + str(ex))


def clearBuffer(timeout=0.05):
    # like "getOutput" except doesn't wait for output and doesn't report anything
    start = time.time()
    while (time.time()-start) < float(timeout):
        pass
    while config.ser_comm["taz"].in_waiting: config.ser_comm["taz"].readline()
        
def getOutput(timeout=0.05, wait_for_string="none"):
    # gets (and clears) all available serial output bits
    try:
        noOutputMSG = "Timeout exceeded: No output available"
        fetchMSG = "Fetching output..."
        output = []
        start = time.time() # function timer timestamp
        
        if(config.status["prevLogEntry"] != noOutputMSG) and (config.status["prevLogEntry"] != fetchMSG):
            addToLog(fetchMSG)
            
        while(len(output)==0): # makes sure to wait until there is an output

            if(config.ser_comm["taz"].in_waiting > 0): # if there's an output waiting

                while(config.ser_comm["taz"].in_waiting > 0): # read all lines of output
                    output.append(config.ser_comm["taz"].readline().strip().decode())
                    addToLog(">>>  " + str(output[-1]))
                    
                if(config.status["printerStatus"] == 'busy'):
                    config.status["printerStatus"] = 'ready'
                    
            if((time.time()-start) > float(timeout)): # if no output in allotted time

                if(config.status["prevLogEntry"] != noOutputMSG) and (config.status["prevLogEntry"] != fetchMSG):
                    addToLog(noOutputMSG)
                break
            
        return(output)
    
    except Exception as ex:
        addToLog("ERROR in BasicComm.getOutput")
        addToLog("\t" + str(ex))

def toSerBytes(somestring):
    # Formatting - Converts a string to bytes and adds prefix and suffix to send to printer
    try:
        #somebytes = bytes("b'" + somestring +"\n'", 'utf-8')
        somebytes = bytes(somestring + "\n", 'utf-8')
        return(somebytes)
    except Exception as ex:
        addToLog("ERROR in BasicComm.toSerBytes")
        addToLog("\t" + str(ex))

def changeCoords(newXYZ):
    # newXYZ format needs to be [##,##,##]
    try:
        if(len(newXYZ) != 3): raise ValueError('Need three values')
        clearBuffer()
        gcode = "G92 X" + str(newXYZ[0]) + " Y" + str(newXYZ[1]) + " Z" + str(newXYZ[2])
        addToLog("sendCommand: " + gcode)
        config.ser_comm["taz"].write(toSerBytes(gcode))
        while True:
            if getRawPos()[0:3] == newXYZ:
                addToLog("Verified new coordinates")
                break
        config.coords["current"][0:3] = newXYZ
    except Exception as ex:
        addToLog("ERROR in BasicComm.changeCoords")
        addToLog("\t" + str(ex))
        


def sendCommand(somegcode, wait=False, log=True, listenTime=0.05):
    # Takes a string of gcode, converts it and sends it to printer
    # Since M400 doesn't work for G0 & G1, wait=True pauses execution
    # until all axes reach destination coordinates
    try:
        if((somegcode[:2] == "G0" or somegcode[:2] == "G1") and wait==True):
            rawDestCoords = somegcode.replace("G0 ","").replace("G1 ","")
            destCoords = rawDestCoords.split(" ")
            if(config.coords["motionMode"] == 'absolute'):
                destX = str(config.coords["current"][0])
                destY = str(config.coords["current"][1])
                destZ = str(config.coords["current"][2])
                destE = str(config.coords["current"][3])
                for coord in destCoords:
                    if coord[0] in "XYZE":
                        if coord[0] == "X": destX = coord[1:]
                        if coord[0] == "Y": destY = coord[1:]
                        if coord[0] == "Z": destZ = coord[1:]
                        if coord[0] == "E": destE = coord[1:]
            elif(config.coords["motionMode"] == 'relative'):
                destX = config.coords["current"][0]
                destY = config.coords["current"][1]
                destZ = config.coords["current"][2]
                destE = config.coords["current"][3]
                if(log==True):
                    addToLog("Relative mode confirmed")
                for coord in destCoords:
                    if coord[0] in "XYZE":
                        addToLog("Displacement = " + str(coord))
                        if coord[0] == "X": destX += round(float(coord[1:]),6)
                        if coord[0] == "Y": destY += round(float(coord[1:]),6)
                        if coord[0] == "Z": destZ += round(float(coord[1:]),6)
                        if coord[0] == "E": destE += round(float(coord[1:]),6)
            destPositions = (list(map(float, (destX, destY, destZ, destE))))
            if(log==True):
                addToLog("Dest. Pos. = " + str(destPositions))
                addToLog("sendCommand:  " + str(toSerBytes(somegcode)) + " wait=" + str(wait))
            config.ser_comm["taz"].write(toSerBytes(somegcode.replace(' ','')))
            while True:
                xDiff = abs(getRawPos()[0] - destPositions[0])
                yDiff = abs(getRawPos()[1] - destPositions[1])
                zDiff = abs(getRawPos()[2] - destPositions[2])
                if (max(xDiff,yDiff,zDiff) < 0.01):
                    addToLog("Destination reached")
                    config.coords["current"] = destPositions
                    break
        else:
            if(log==True):
                addToLog("sendCommand:  " + str(toSerBytes(somegcode)) + " wait=" + str(wait))
            config.ser_comm["taz"].write(toSerBytes(somegcode))
    except Exception as ex:
        addToLog("ERROR in BasicComm.sendCommand")
        addToLog("\t" + str(ex))

def feedGCODE(GCODE):
    try:
        tipCleaning = False
        config.status["printerStatus"] = 'ready'
        while(len(GCODE) > 0):
##            getOutput() # Checks (waits) for printer response and sets status accordingly
            if(len(GCODE) > 0) and (config.status["printerStatus"] == 'ready'):
                if("#" in GCODE[0]): #if it's a comment
                    if(GCODE[0] == "# TIP CLEANING CODE:"):
                        tipCleaning = True # flag to omit tip cleaning GCODE from log
                        addToLog("<TIP CLEANING CODE OMITTED>")
                    del GCODE[0]
                else:
                    config.status["printerStatus"] = 'busy'
                    if(tipCleaning == False):
                        sendCommand(GCODE[0])
                    elif(tipCleaning == True):
                        sendCommand(GCODE[0], log=False)
                    del GCODE[0]    # removes first line
                    waitForOK()     # waits for printer to return 'ok' before continuing
##        config.status["printerStatus"] = 'ready'
    except Exception as ex:
        addToLog("ERROR in BasicComm.feedGCODE")
        addToLog("\t" + str(ex))

def waitForOK():
    # for use with feedGCODE
    # waits for printer to return 'ok' before sending more GCODE to input buffer
    try:
        start = time.time()
        output = ''
        while True:
            if(config.ser_comm["taz"].in_waiting > 0): # if there's an output waiting
                while(config.ser_comm["taz"].in_waiting > 0): # read all lines of output
                    output = config.ser_comm["taz"].readline().strip().decode()
                    if('ok' in output):
                        if(config.status["printerStatus"] == 'busy'):
                            config.status["printerStatus"] = 'ready'
                        break
            elif(time.time() - start > 120):
                addToLog("waitForOK Timeout")
                break
            elif('ok' in output):
                break
    except Exception as ex:
        addToLog("ERROR in BasicComm.waitForOK")
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
    # queries the printer for build plate temperature and accessory temperature
    try:
        addToLog("Requesting temperatures:")
        start = time.time()
        getOutput() # gets (and clears) output bits
        sendCommand("M105") # "Report Temperatures" command
        time.sleep(0.1)
        while True:
            response = config.ser_comm["taz"].readline().decode() # ".decode()" converts to str
            if (len(response)>10):
                break
        try:
            bedTemp = response[(response.find("B:")+2):(response.find("B:")+2+5)]
            bedTemp = bedTemp.replace(" ","")
        except Exception as ex:
            bedTemp = 0
            addToLog("ERROR in BasicComm.getBedTemp -> bedTemp")
            addToLog("\t" + str(ex))
        try:
            accTemp = response[(response.find("T:")+2):(response.find("T:")+2+5)]
            accTemp = accTemp.replace(" ","")
        except Exception as ex:
            accTemp = 0
            addToLog("ERROR in BasicComm.getBedTemp -> accTemp")
            addToLog("\t" + str(ex))
        getOutput() # clears whatever might be left
        end = time.time()
        computationTime = round((end-start)*1000,3)
        addToLog("Bed = " + str(bedTemp) + ", Acc = " + str(accTemp) +
                 " (" + str(computationTime) + "ms)")
        return(bedTemp,accTemp)
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
    # requests current position; monitors output until position received
        try:
            count = 0
            noValidPositionData = True
            detail_marker = "V55" # to check if Marlin returned detailed positional data (which we didn't ask for!)
            while noValidPositionData:
                addToLog("Requesting position [" + str(count) + "]:")
                clearBuffer(0.15)
                sendCommand("M114") # "Get Current Position" command
                while True:
                    RawPosData = config.ser_comm["taz"].readline().decode() # ".decode()" converts to str
                    addToLog(">>>  " + RawPosData)
                    if detail_marker in RawPosData:
                        clearBuffer()  # as for positional data again
                        sendCommand("M114")
                        count += 1
                        addToLog("Requesting position [" + str(count) + "]:")
                    elif "X:" in RawPosData: # when the coordinates string is found
                        posData = RawPosData.replace("b'","").replace("\\n'","")
                        PosList = posData.split(" ")
                        xPos = PosList[0][2:]
                        yPos = PosList[1][2:]
                        zPos = PosList[2][2:]
                        ePos = PosList[3][2:]
                        break
                try:
                    positions = (list(map(float, (xPos, yPos, zPos, ePos))))
                    noValidPositionData = False
                except ValueError:
                    noValidPositionData = True
                    count += 1
            clearBuffer()
            addToLog("Current Pos. = " + str(positions))
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
        doBeep()
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

def startCampaign():
    pass

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
        
