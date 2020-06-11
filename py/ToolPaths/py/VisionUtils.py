import serial
import time
import AmaresConfig as config #need to import AmaresConfig in all modules
from BasicComm import addToLog

def openLightPort():
    try:
        config.ser_comm["lights"].open()
        addToLog("Opening lights on " + str(config.ser_comm["lights"]))
        while (config.ser_comm["lights"].in_waiting < 7):
            pass
        addToLog("Lights port opened")
    except Exception as ex:
        addToLog("ERROR in VisionUtils.openLightPort")
        addToLog("\t" + str(ex))

def closeLightPort():
    try:
        config.ser_comm["lights"].close()
        time.sleep(0.2)
        addToLog("Lights port closed")
    except Exception as ex:
        addToLog("ERROR in VisionUtils.closeLightPort")
        addToLog("\t" + str(ex))
    
def setLight(index, value):
    try:
        if (config.ser_comm["lights"].isOpen() == False):
            openLightPort()
        if (index==1) and (value!=0):
            print("Accessing process lights...")
        if (index==2) and (value!=0):
            print("Accessing alignment lights...")
       
        value = (index*1000) + value
        config.ser_comm["lights"].write(b'%d' % value)
        time.sleep(0.05)
    except Exception as ex:
        addToLog("ERROR in VisionUtils.setLight")
        addToLog("\t" + str(ex))

def processLights(value=255):
    try:
        setLight(1,value)
        addToLog("Process lights set to " + str(value))
    except Exception as ex:
        addToLog("ERROR in VisionUtils.processLights")
        addToLog("\t" + str(ex))
        
def alignmentLights(value=255):
    try:
        setLight(2,value)
        addToLog("Alignment lights set to " + str(value))
    except Exception as ex:
        addToLog("ERROR in VisionUtils.alignmentLights")
        addToLog("\t" + str(ex))





    
