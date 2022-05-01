# QREST

QREST (Quality Review and Exchange System for Tribes) is open-source software that provides tools to address the challenges of tribal air quality data access, validation, reporting, and submittal for multiple tribes throughout the US. This github site hosts the latest source code for QREST. For Tribal Agencies that wish to begin using QREST, please go to www.qrest.net. 

## Table of Contents

- [QREST Goals](#qrest-goals)
- [QREST Features](#qrest-features)
- [Change Log](#Changelog)
- [Sending Feedback](#sending-feedback)



## QREST Goals

The overarching goals of QREST are to:
1. Provide small tribal agencies with the ability to generate legally-defensible air quality data and build tribal capacity
2. Support tribes with air quality data analysis and reporting by pushing out industry and EPA changes to all participating tribes at once 
3. Enable more valid data availability to EPA and partners

## QREST Features

QREST is a set of software tools that includes the following features:
-	**Data Logger Integration:** Integrate with tribal data loggers to automatically retrieve air monitoring data
-	**Automated Data Averaging:** As n-minute data are streamed into QREST, hourly summaries are immediately calculated and stored, using calculation logic defined for a tribe’s specific monitoring conditions. QREST is capable of calculating averages, mins, maxes, angular averages, totals, standard deviations, and angular (Yamartino) standard deviations. 
-	**Automated Data Validation & Alerting:** as data is streamed into QREST, validation checks are automatically performed, identifying data that exceeds expected ranges, identifies 'stuck' readings, or shows missing data (indicating data logger communication issues). Designated tribal operators are notified of exceptions, via email and/or text message.  
-	**Manual Data Upload:** Support for manual upload of data logger files in lieu of automated data integration
-	**Multi-Phase Data Review:**  Fulfill the independent quality assurance function required for regulatory and legally-defensible data, by providing a structure for the two levels of separation required between data gatherer and final data validation as required by CFR and all quality system requirements (ISO). 
-	**Quality Control:** Manage single-point QC, Annual Performance Evaluations, Flow Rate Verifications, Semi-Annual Flow Rate Audits (for particulate matter), and Zero/Span checks
-	**AQS Integration:** Push raw data (RD) and quality control (QA) data to EPA’s Air Quality Subsystem (AQS) via EPA’s Exchange Network. In addition, AQS reference data and other relevant codes and limits are pushed to QREST users when EPA makes changes 
-	**AirNow Integration:** tribes can opt to share data from QREST to EPA’s AirNow program
-	**Data Sharing with Public:** map-based website allowing tribes to share air data with the public, with options to download data reports
- **API Access:** 3rd party applications can integrate with QREST via API web services

The following diagram illustrates the various way that tribes can get data inTO QREST: 

![ezcv logo](https://raw.githubusercontent.com/open-environment/QREST/master/QREST/Content/Images/GettingDataIntoQREST.png)


## Changelog
Starting from 1/25/2021, changes to the released version of www.qrest.net are noted here:

### 2021-01-25
- AQS Submission module -> AQS/CDX account selection page -> improved ability to choose between local and global CDX account
### 2021-01-27
- AQS submission enhancements
- Data Export -> additional columns included in export


## Sending Feedback
We are always open to [your feedback](https://github.com/open-environment/QREST/issues).

