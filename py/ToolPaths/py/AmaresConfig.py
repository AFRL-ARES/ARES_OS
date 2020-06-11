from win32api import GetSystemMetrics
import json
import serial
import math
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
        "errorBeep": 'M300 S4000 P30: M300 S3000 P30: M300 S2000 P40',
        "sigfigs": 6
        }

    # 0.813 mm, "M909" Brown, 1/2" from McMaster-Carr
    # 0.54 mm,  "M921" Green, 1/2" from McMaster-Carr
    # 0.43 mm, "M925" LightBlue, 1/2" from McMaster-Carr
    global dispenser
    dispenser = {
        "diameter": 0.54,
        "tiplength": 12, #mm
        "plungerRadius": 8,
        "speed": 300, #mm/min. print speed
        "multiplier": 0.75,
        "prime": 0.24,
        "retract": 0.26,
        "prime_rate": 200,
        "retract_rate": 200,
        "prime_delay": 200, #ms, delay after priming
        "retract_delay": 0, #ms, delay before retracting
        "work_dist": 0.25,
        "flexFactor": 0.196
        }
    dispenser["radius"] = round(dispenser["diameter"]/2,6)
    dispenser["tip_volume"] = math.pi*(dispenser["radius"]**2)

    global status
    status = {
        "homingDone": False,
        "printerStatus": "ready",
        "prevLogEntry": "" # used to filter superfluous repeating log entries (TAX outputs)
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
    
    global paths
    paths = {
        "pypath": os.path.dirname(os.path.abspath(__file__)) + "\\"
        }
    if(not os.path.exists(paths["pypath"] + "logs\\")):
        os.makedirs(paths["pypath"] + "logs\\")
    paths["logpath"] = paths["pypath"] + "logs\\"
    
    global tool_vars
    f = open(paths["pypath"] + "tool_params.json","r")
    infile = json.load(f)
    tool_vars = {
        "currentTool": "depHead",
        #If the json file ends up only having one dictionary item
        #then it needs to be read as a list e.g. "off": infile . If the json
        #file ends up being structured as a proper dictionary, then use
        #"off": (infile.get("off")) to reference the appropriate dictionary
        #entry
        "off": (infile.get("off"))
        }
    f.close()

    global coords
    coords = {
        "homingOffset": [71.5, -221], # old vs. new home position offset using G92
        "initHome": [10,0,6], # Initial home position
        "dynHome": [0,0,0], # Dynamic home position (e.g. for substrates)
        "gridHome": [0,0,0], # for printing multiple samples in a grid whose origin is relative to dynHome
        # soft stage limits based on tip and alignment camera coords:
        "lim_west": 10,
        "lim_east": 200,
        "lim_south": -24,
        "lim_north": 230,
        "motionMode": "none"
        }
    
    global motion
    motion = {
        "slowSpeed": 300, # mm/minute
        "medSpeed": 2500, # mm/minute
        "fastSpeed": 5000, # mm/minute
        "smallIncrement": 0.1, # mm
        "medIncrement": 1,
        "largeIncrement": 10.0,
        "increment": 1
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
    uservars[uservars["var1"]] = 0
    uservars[uservars["var2"]] = 0
    uservars[uservars["var3"]] = 0
    uservars[uservars["var4"]] = 0
    uservars[uservars["var5"]] = 0
    uservars[uservars["var6"]] = 0

def setConfigValue(valname, value):
    vals = valname.split('.')
    globalname = vals[0]
    fieldname = vals[1]
    logtime = datetime.now().strftime("%H:%M:%S.%f")
    try:
        if (globals()[globalname][fieldname] != value):
            globals()[globalname][fieldname] = value
            if (str(globalname) != "uservars"):
                paths["log"].write(logtime + "  " + str(fieldname) + " set: " + str(globals()[globalname][fieldname]) + "\n")
                paths["log"].flush()
            else:
                paths["log"].write(logtime + "  " + "NAME MISSING" + " set: " + str(globals()[globalname][fieldname]) + "\n")
                paths["log"].flush()
    except Exception as ex:
            paths["log"].write(logtime + "  ERROR in AmaresConfig.setConfigValue\n")
            paths["log"].write(logtime + "\t\t" + str(ex) + "\n")
            paths["log"].flush()
    
def getConfigValue(valname, index=None):
    vals = valname.split('.')
    globalname = vals[0]
    fieldname = vals[1]
    if (index is not None):
        value = globals()[globalname][fieldname][index]
        logtime = datetime.now().strftime("%H:%M:%S.%f")
        paths["log"].write(logtime + "  Retrieving: " + str(globals()[globalname][fieldname][index]) + ".\n")
        paths["log"].flush()
        return value
    value = globals()[globalname][fieldname]
    logtime = datetime.now().strftime("%H:%M:%S.%f")
    paths["log"].write(logtime + "  Retrieving: " + str(globals()[globalname][fieldname]) + ".\n")
    paths["log"].flush()
    return value
        

