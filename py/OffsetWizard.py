import pymsgbox
import serial
from MotionRoutines import *
import numpy as np
from glob import * # used for inter-module globals instead of creating separate globals file
import AmaresConfig as config
from BasicMotion import doBeep, absDistance, plungerDistance, runToolpath
from BasicComm import checkConnection, closeConnection, toSerBytes, sendCommand
from BasicComm import getOutput, addToLog
import simplejson as json
from operator import add
from ToolpathGenerator import initTP, startDep, depToPoint, endDep
from pprint import pprint
import copy
import socket
from Tools import alignmentCamera, depHead


def crosshairToolpath():
    config.dispenser["multiplier"] = 1
    # crosshair creation must end at the same X, Y, and Z coordinate that it started
    # with (easy) or other code that updates wcoords variables appropriately must exist (less easy)

    config.coords["dynHome"] = getPos()[:3]
    Size = config.xhair_vals["size"]    # nominal crosshair size in mm
    tipRadius = config.dispenser["radius"]
    
    # generate toolpath to print crosshair :
    # (stored in config.tpGen["gcode"])
    initTP()
    startDep('XY',0,0)
    depToPoint('X',Size)
    depToPoint('Y',Size)
    depToPoint('X',0)
    depToPoint('Y',0)
    endDep('XY',Size,Size)
    startDep('X',0)
    depToPoint('XYZ',Size/2,Size/2,tipRadius/2)
    endDep('XYZ',Size,0,0)

def runOffsetWizard(*args):
    client = config.TCP["client"]
    if not config.TCP["isInUse"]:
        config.TCP["isInUse"] = True
        client.send("Running offset wizard.\n".encode())
        if(config.tool_vars["currentTool"] == "depHead"):
            if(config.status["homingDone"] == True):
                client.send("Offset Definition: Print Crosshair?[Y/n]: ".encode())
                response = client.recv(1024).decode('utf-8')[0].upper()
                check = response != 'N'
                if(check):
                    # makes sure we have appropriate serial connections:
                    checkConnection(config.ser_comm["taz"])
                    checkConnection(config.ser_comm["lights"])
                    crosshairToolpath() # Loads crosshair toolpath
                    runToolpath()
                    # returns motion mode to absolute:
                    motionMode('absolute')
                    client.send("Offset Definition: Continue?[Y/n]:".encode())
                    response2 = client.recv(1024).decode('utf-8')[0].upper()
                    check2 = response2 != 'N'
                    if(check2):
                        alignmentCamera()
                        translate('NW',config.xhair_vals["size"]/2,config.motion["medSpeed"])
                        #restoreZPos()
                        # Record current coordinates:
                        oldCoords = copy.deepcopy(config.coords["current"])
                        addToLog("oldCoords = " + str(oldCoords) + ", Type: " + str(type(oldCoords)))
                        client.send("Offset Definition: Align crosshairs. Once aligned\nenter [Y] to save or [n] to cancel.[Y/n]:".encode())
                        response3 = client.recv(1024).decode('utf-8')[0].upper()
                        check3 = response3 != 'N'
                        if(check3):
                            # Adjust offset values accordingly:
                            config.tool_vars["off"][0] += round((config.coords["current"][0] - oldCoords[0]),3)
                            config.tool_vars["off"][1] += round((config.coords["current"][1] - oldCoords[1]),3)
                            config.tool_vars["off"][2] += round((config.coords["current"][2] - oldCoords[2]),3)
                            addToLog("Current coords = " + str(config.coords["current"]) +
                                     " oldcoords = " + str(oldCoords))
                            # Write new offset values to file:
                            data = json.dumps(config.tool_vars, indent=4, sort_keys=True) #["off"])
                            f = open(config.paths["pypath"] + "tool_params.json","w")
                            f.write(data)
                            f.close()
                            client.send("Switch back to dispenser camera[Y] or remain\non alignment camera[n]?".encode())
                            response4 = client.recv(1024).decode('utf-8')[0].upper()
                            check4 = response4 != 'N'
                            if(check4):
                                depHead()
                    else:
                        pass
                        #restoreZPos()
            else:
                client.send("Homing routine must be completed\nbefore running offset definition\n".encode())
                config.TCP["isInUse"] = False
        else:
            client.send("Extruder tool must be active\nbefore running offset wizard")
            config.TCP["isInUse"] = False
    elif config.TCP["isInUse"]:
        addToLog("ERROR: Terminal already in use")


def restoreZPos():
    # accessory to runOffsetWizard
    # moves tool back to accurate z-position after raising to preserve xhair features
    motionMode('relative')
    sendCommand("G0 Z-" + str(config.xhair_vals["final_offset"])) #return to proper z-position
    if(config.status["homingDone"] == True): motionMode('absolute')
            

    


        
