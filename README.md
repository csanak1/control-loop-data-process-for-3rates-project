# Console app to query and process Control Loop data from Proficy Historian
It was developed for an internal project called 3Rates @ BorsodChem, which provides data to calculate CR KPI. 

## Background
Why is the weird name of this repo?
In BC's 3Rates project I developed applications to calculate KPI's for every plant from Alarms (ISA18.2 - AR), Control Loop (CR) and Interlock (IR) operation modes.
Some tags in Historian representing control loop operation modes, their data type is string. Full automatic operation is the aim. 
These KPI values are helping to create a measure.

I made this part of the project is made public in hope to help someone else to query data from Proficy Historian using Proficy.Historian.ClientAccess.API
