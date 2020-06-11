import AmaresConfig as config
from BasicMotion import motionMode
from BasicComm import *
from VisionUtils import *

# NOTE: config.coords["current"] always tracks the deposition tip regardless
# of tool selected

def alignmentCamera(moveByOffset=True):
    try:
        #processLights(0, storeVal=False)
        # Moving between tools using offset values is relative motion:
        if(config.tool_vars["currentTool"] != "alignmentCamera"):
            alignmentLights(config.tool_vars["alignLightVal"])
            addToLog("Switching to alignment camera")
            if(moveByOffset==True):
            # By not using 'translate' we avoid stage soft limits but..
            # ...the working coordinates don't change...
                motionMode('relative')
                sendCommand("G0 Z" + str(round(config.tool_vars["off"][2],3)) + " F500", wait=False)
                sendCommand("G0 X" + str(round(config.tool_vars["off"][0],3)) + " Y" +
                            str(round(config.tool_vars["off"][1],3)) +" F3000", wait=False)
                if(config.status["homingDone"] == True): motionMode('absolute')
            # ...so we change the working coordinates manually here:
                config.coords["current"][0] += config.tool_vars["off"][0]
                config.coords["current"][1] += config.tool_vars["off"][1]
                config.coords["current"][2] += config.tool_vars["off"][2]
                config.coords["current"] = [round(num,3) for num in config.coords["current"]]
                 
            config.tool_vars["currentTool"] = "alignmentCamera"
            
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
        #alignmentLights(0, storeVal=False)
        # Moving between tools using offset values is relative motion:
        if(config.tool_vars["currentTool"] != "depHead"):
            processLights(config.tool_vars["procLightVal"])
            addToLog("Switching to tip camera")
            
            # By not using 'translate' we avoid stage soft limits but..
            # ...the working coordinates don't change...
            motionMode('relative')
            sendCommand("G0 X" + str(-1*round(config.tool_vars["off"][0],3)) + " Y" +
                        str(-1*round(config.tool_vars["off"][1],3)) + " F3000", wait=False)
            sendCommand("G0 Z" + str(-1* round(config.tool_vars["off"][2],3)) + " F500", wait=False)
            # ...so we change the working coordinates here:
            config.coords["current"][0] -= config.tool_vars["off"][0]
            config.coords["current"][1] -= config.tool_vars["off"][1]
            config.coords["current"][2] -= config.tool_vars["off"][2]
            config.coords["current"] = [round(num,3) for num in config.coords["current"]]
            if(config.status["homingDone"] == True): motionMode('absolute')
                
            config.tool_vars["currentTool"] = "depHead"
            
            # Also need to update stage limits based on tool:
            config.coords["lim_west"] -= config.tool_vars["off"][0]
            config.coords["lim_east"] -= config.tool_vars["off"][0]
            config.coords["lim_south"] -= config.tool_vars["off"][1]
            config.coords["lim_north"] -= config.tool_vars["off"][1]
    except Exception as ex:
        addToLog("ERROR in MotionRoutines.depHead")
        addToLog("\t" + str(ex))
