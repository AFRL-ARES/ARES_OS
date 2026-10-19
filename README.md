# **ARES OS 2.0 is Available**

This repository is the codebase for ARES OS 1.0, our original ARES system. We have since migrated to ARES OS 2.0 as our new open source self-driving lab software. You can find that repository at https://github.com/AFRL-ARES/ARES or you can visit our documentation page at https://afrl-ares.github.io/ to learn more about our ARES OS 2.0 ecosystem. This repository is archived and no further changes will be made to this codebase. 

# ARES OS&trade;

ARES OS&trade; is research software to streamline the creation of systems for closed-loop, autonomous experimentation. 
The solution provided in this repository consists of 4 projects, ARESCore, AresSampleDevicePlugin, AresSampleAnalysisPlugin, and AresSamplePlanningPlugin. The Plugins are customized to the research resources while ARESCore provides the structured interface and closed loop testing capabilities.

The Devices plugin generally registers and controls devices and device commands. For example, a device may be a valve with 2 settings (on and off). The devices plugin would define the connection, control UI, and commands for this valve.

The Analysis plugin registers analyzers, which are the way in wich experiments are quantified. For example, a visual analysis technique may perform image processing to measure the quality of an experiment.

The planning plugin provides access to planners, which determine experiment parameters. 

## Software Requirements
PostgreSQL must be installed, with an administrator password of 'a'. You can install multiple instances of PostGreSQL, but note the port so you can select it the first time you start ARES OS&trade;.

## Developer Setup
If you are going to change/edit the software, we HIGHLY recommend using the IDE “Visual Studio”, versions 2019 or later. It can be done using the community edition. You can open the solution and be able to build and run from there. NuGet packages are used by this software, so we recommend being connected to a mechanism that can collect the packages (such as the internet). 

## License
This software is governed by the AFRL open source license, which requires registration. See the full terms on the license page.
