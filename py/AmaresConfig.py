from win32api import GetSystemMetrics
import json
import serial
import math
import time
import os.path
from datetime import datetime

def init():
    global configuration
    configuration = {
        "ProcessCam_ID": 2,
        "AlignCam_ID": 1,
        "Delay": 45,
        "repeatDelay": 500,
        "verbose": 'on',
        "errorBeep": 'M300 S1900 P30: M300 S2100 P30: M300 S2400 P40',
        "sigfigs": 6,
        "useLog": True
        }

    # 0.813 mm, "M909" Brown, 1/2" from McMaster-Carr
    # 0.54 mm,  "M921" Green, 1/2" from McMaster-Carr
    # 0.43 mm, "M925" LightBlue, 1/2" from McMaster-Carr
    global dispenser
    dispenser = {
        "diameter": 0.54,
        "tiplength": 12, #mm
        "plungerRadius": 8,
        "speed": 100, #mm/min. print speed
        "multiplier": 0.75,
        "prime": 0.45,
        "retract": 0.45,
        "prime_rate": 200,  #mm/min?
        "retract_rate": 200,#mm/min?
        "prime_delay": 0.300, #s, delay after priming
        "retract_delay": 0, #s, delay before retracting
        "work_dist": 0.1,
        "coastFactor": 0.05,  #coasts for percentage of the move listed
        "material": "Alex Plus Acrylic Latex Caulk Plus Silicone - White"
        }
    dispenser["flexFactor"] = 0.003086 / pow(dispenser["diameter"],3)
    dispenser["radius"] = round(dispenser["diameter"]/2,6)
    dispenser["tip_volume"] = math.pi*(dispenser["radius"]**2)

    global status
    status = {
        "homingDone": False,
        "printerStatus": "ready",
        "prevLogEntry": "" # used to filter superfluous repeating log entries (TAX outputs)
        }

    global expt
    expt = {
        "finished": time.time(),
        "time_elapsed": 9999999,
        "wait_time": 15 # set this to the desired minimum time between experiments in seconds
        }

    global xhair_vals
    # crosshair parameters for offset definition
    xhair_vals = {
        "size": 10, #mm
        "final_offset": 1 # amount to raise z-axis when returning tip to center
        }

    global ser_comm
    # parameters for serial communication
    ser_comm = {
        "ser_out": "",
        "loadedTP": [], # "loaded toolpath"
        "lights": serial.Serial(),
        "taz": serial.Serial()
        }
    ser_comm["lights"].baudrate = 9600
    ser_comm["lights"].port = "COM4"
    ser_comm["taz"].baudrate = 250000
    ser_comm["taz"].port = "COM3"

    global TCP
    TCP = {
        "port": 5005,
        "isInUse": False,
        "client": None,
        "addr": None
        }

    global data
    # mm, ms, °C, <pressure units>
    data = {"ParamNames": [
                "Expt#",
                "Date",
                "Time",
                "Location",
                "Result Descriptor",
                "Result Value",
                "Result Unit",
                "Nozzle Diameter",
                "Nozzle Length",
                "Extrusion Multiplier",
                "Working Distance",
                "Prime Distance",
                "Prime Rate",
                "Prime Delay",
                "Retract Distance",
                "Retract Rate",
                "Print Speed",
                "Coast Factor",
                "Flex Factor",
                "Bed Temperature",
                "Enclosure Temperature",
                "Substrate",
                "Analyzer",
                "Analyzer Parameters",
                "Analysis Light",
                "Process Light",
                "Material",
                "Initial Volume",
                "Syringe Date",
                "Syringe Time",
                "Volume Used",
                "Time Elapsed",
                "Wait Time",
                "Relative Humidity",
                "Barometric Pressure",
                "Planner"
                ],
            'Expt#': 0,
            'Result Unit': "none",
            'Result Value': 0,
            'Result Descriptor': '',
            'bedTemp': 0,
            'enclosureTemp': 0,
            'substrate': "glass",
            'Analyzer': "none",
            'Analyzer Parameters': "",
            'History': [],
            'VectorHistory': [],
            'Relative Humidity': 0,
            'Barometric Pressure': 0,
            'PlannedParams': "",
            'Planner': "Simple",
            'Target': 1,
            'expt_date': 0,
            'expt_time':0
           }

    global limits
    limits = {}
    
    global paths
    paths = {
        "pypath": os.path.dirname(os.path.abspath(__file__)) + "\\",
        "transferParent": "D:\\AM ARES\\Transfers\\"
        } # returns directory of python file being run
    if(not os.path.exists(paths["pypath"] + "logs\\")):
        os.makedirs(paths["pypath"] + "logs\\")
    paths["logpath"] = paths["pypath"] + "logs\\"
    if(not os.path.exists(paths["pypath"] + "data\\")):
        os.makedirs(paths["pypath"] + "data\\")
    paths["datapath"] = paths["pypath"] + "data\\"
        
    global tool_vars
    f = open(paths["pypath"] + "tool_params.json","r")
    infile = json.load(f)
    tool_vars = {
        "currentTool": "depHead",
        "alignLightVal": infile.get("alignLightVal"),
        "procLightVal": infile.get("procLightVal"),
        #If the json file ends up only having one dictionary item
        #then it needs to be read as a list e.g. "off": infile . If the json
        #file ends up being structured as a proper dictionary, then use
        #"off": (infile.get("off")) to reference the appropriate dictionary
        #entry
        "off": (infile.get("off")),             # tool offset
        "initVolume": infile.get("initVolume"),
        "volumeUsed": infile.get("volumeUsed"),
        "syringeDate": infile.get("syringeDate"),
        "syringeTime": infile.get("syringeTime")
        }
    tool_vars["off"] = [round(val,3) for val in tool_vars["off"]]
        
    f.close()

##    try:
##        global vector_params
##        with open(paths["pypath"] + "vector_params.json","r") as infile:
##            vector_params = json.load(infile)
##    except:
##        pass

    global coords
    coords = {
        "homingOffset": [71.5, -191], # old vs. new home position offset using G92
        "initHome": [10,30,10], # Initial home position (used to be [10,0,6])
        "dynHome": [0,0,0], # Dynamic home position (e.g. for substrates)
        "gridHome": [0,0,0], # for printing multiple samples in a grid whose origin is relative to dynHome
        # soft stage limits based on tip and alignment camera coords:
        "lim_west": 10,  #10,
        "lim_east": 202,  #200,
        "lim_south": 30, #-24,
        "lim_north": 170, #230,
        "motionMode": "none"
        }

    global tipCleaning
    tipCleaning = {
        "cleanXYZ": [-70,24,0],
        "zStep": 0.2,
        "xyStep": 0.5,
        "lateralSpeed": 5000,
        "zOffsetWhenDone": 1,   # mm
        "numChords": 36,        # number of sides in polygon to approximate circle
        "radiusIncrement": 0.03 # mm
        }
    
    global motion
    motion = {
        "slowSpeed": 300, # mm/minute
        "medSpeed": 2500, # mm/minute
        "fastSpeed": 5000, # mm/minute
        "smallIncrement": 0.1, # mm
        "medIncrement": 1,
        "largeIncrement": 10.0,
        "increment": 1,
        "minIncrement": 0.06 # smallest lateral motion possible (stepper limitation)
        }
    motion["speed"] = motion["medSpeed"]
    
    global tpGen # toolpath generator
    tpGen = {
            "coords": [],
            "gcode": [],
            "maxcoords": [0,0,0]
            }

    global uservars
    uservars = {
        "var1": "NAME1",
        "var2": "NAME2",
        "var3": "NAME3",
        "var4": "NAME4",
        "var5": "NAME5",
        "var6": "NAME6"
        }
    uservars[uservars["var1"]] = 0 # i.e. uservars["speed"] = 0
    uservars[uservars["var2"]] = 0
    uservars[uservars["var3"]] = 0
    uservars[uservars["var4"]] = 0
    uservars[uservars["var5"]] = 0
    uservars[uservars["var6"]] = 0


def addToLog(data):
    if (configuration["useLog"] == True):
        if((data != ">>>  b'ok\\n'") or ((data == ">>>  b'ok\\n'") and (status["prevLogEntry"] != ">>>  b'ok\\n'"))):
            logtime = datetime.now().strftime("%H:%M:%S.%f")
            status["prevLogEntry"] = data # used to filter repeating log data (see getOutput)
            text = logtime + "  " + data + "\n"
            status["logindex"] += (len(text) + 6)
            logindex = str(status["logindex"]).zfill(5) + " "
            paths["log"].write(text)
            paths["log"].flush()
    else:
        pass

def setConfigValue(valname, value):
    vals = valname.split('.')
    globalname = vals[0]
    fieldname = vals[1]
    try:
        if (globals()[globalname][fieldname] != value):
            globals()[globalname][fieldname] = value
            if (str(globalname) != "uservars"):
                addToLog(str(fieldname) + " set: " + str(globals()[globalname][fieldname]))
            else:
                addToLog("NAME MISSING" + " set: " + str(globals()[globalname][fieldname]))
    except Exception as ex:
        addToLog("  ERROR in AmaresConfig.setConfigValue\n\t\t" + str(ex))

#  
def getConfigValue(valname, index=None):
    vals = valname.split('.')
    globalname = vals[0]
    fieldname = vals[1]
    if (index is not None):
        value = globals()[globalname][fieldname][index]
        addToLog("  Retrieving: " + str(globalname) + ": " + str(fieldname) +
                 ": " + str(globals()[globalname][fieldname][index]))
        return value
    value = globals()[globalname][fieldname]
    addToLog("  Retrieving: " + str(globalname) + ": " + str(fieldname) +
             ": " + str(globals()[globalname][fieldname]))
    return value
        

