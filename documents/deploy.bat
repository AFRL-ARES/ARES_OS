@echo off
Rem %1 %2 are the source and destination directories respectively
robocopy %1\AresAdditiveAnalysisPlugin %2 /e
robocopy %1\AresAdditiveDevicesPlugin %2 /e
robocopy %1\AresAdditivePlanningPlugin %2 /e
robocopy %1\ARESCore %2 /e