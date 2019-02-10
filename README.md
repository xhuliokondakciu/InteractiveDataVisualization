# KGTLearningMachineWeb

This is the Master Thesis project of Xhulio Kondakciu. This application creates a dynamic interactive interface for displaying different set of charts and graphs. The purpose is to display in an interactive way data of Deep Neural Networks for time series.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

To run this application Node.js is a prerequesite. Download and install the latest version from here https://nodejs.org/en/

After Node.js is installed highcharts export server need to be installed next. Run this command in the command promt and make sure it will get installed for all users

```
npm install highcharts-export-server -g
```

* Agree to the license terms
* Select highcharts version 7.0.2
* Select default value for all other options

If thumbnails are not being created after deployment check if the highcharts-export-server command is available to the user under which the web page is running in IIS 

## How to fix Highcharts trial expired

### On Web server

npm uninstall highcharts-export-server -g
npm install highcharts-export-server -g
iisreset

### On development machine

During development period the trial version of Highcharts is used. This will be changes later to a licensed version after the development is done. But during the meatime the highcharts .NET wrapper will have its trial expire after one month of usage. This issue can be detected if after uploading a file to be processed on the workspace the thumbnail of the chart is empty and the chart can't be opend. Opening the the developer tools of the browser and checking the errors will also confirm this. On the request response when opening the chart you should see the message that the trial has expired. To fix this issue this step should be followed:

* Uninstall the Highsoft.Highcharts nuget package from all the projects where it is used
    * KGTMachineLearningWeb
    * KGTMachineLearningWeb.Common
    * KGTMachineLearningWeb.config
    * KGTMachineLearningWeb.Domain
    * KGTMachineLearningWeb.Models
* Install latest version of Highsoft.Highcharts nuget package to all the projects again
    * KGTMachineLearningWeb
    * KGTMachineLearningWeb.Common
    * KGTMachineLearningWeb.config
    * KGTMachineLearningWeb.Domain
    * KGTMachineLearningWeb.Models
* Clean solution
* Rebuild solution
* Re-deploy application

### On build server

* locate local nuget.exe CLI cmd tool (e.g. C:\TeamCity\buildAgent\tools\NuGet.CommandLine.4.8.1\tools)
* run: nuget locals all -clear
* clear the checkout and build dirs for affected build configuration

## Deployment

The application is deployed as a normal ASP.NET web page.

## Authors

* **Xhulio Kondakciu**

