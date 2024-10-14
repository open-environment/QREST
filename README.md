# QREST

QREST (Quality Review and Exchange System for Tribes) is open-source software that provides tools to address the challenges of tribal air quality data access, validation, reporting, and submittal for multiple tribes throughout the US. This github site hosts the latest source code for QREST. For Tribal Agencies that wish to begin using QREST, please go to www.qrest.net. 

## Table of Contents

- [QREST Goals](#qrest-goals)
- [QREST Features](#qrest-features)
- [Change Log](#change-log)
- [Sending Feedback](#sending-feedback)



## QREST Goals

The overarching goals of QREST are to:
1. Provide tribal agencies with the ability to generate legally-defensible air quality data and build tribal capacity
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


## Change Log

- **2024-10-09**
  - **General Data Import Improvement:**
    - Fix bug when importing 5-minute records and specifying to calculate hourly data, but hourly valiation was not being run on the calculated hourly data
    - Fix bug when importing 5-minute records and QREST was not grabbing summary type from import config
- **2024-09-23**
  - **General Data Import Improvement:**
    - When bulk importing 5-minute or hourly data, adjustment factor is now applied if set in the Import Configuration 
  - **5-Minute Data Manual Import Improvements:** 
    - Now prevents starting a manual import if one or more summary calculations in the import template is missing, and the option to calculate hourly data during import is selected.
    - When deleting a past import of 5-minute data on the Manual Import History page, now also deletes any associated calculated hourly data
    - Various timeout settings changed to prevent system timeout on import of large 5-minute data files
  - **Site Polling Config Screen Improvements:**
    - In column mapping section, summary type field (e.g. Average, Total, Std Dev, etc) only displays when polling 5-minute data
    - Cannot save column mapping record if 5 minute and summary field is missing
    - If an adjustment factor is set for a column mapping, it is now viewable in the table
  - **Bug Fix:** fix unable to edit column mappings on Polling Config page if no summary type was specified for the column
- **2024-08-18**
  - **Online Help Performance:** Improved performance of loading the online help
- **2024-07-27**
  - **Polling Logic Change for ZENO & SUTRON loggers:** now retrieves data based on the latest date of data in QREST. Old logic was to grab 12 latest records from logger. Now it pulls up to 99 records starting with the last QREST stored data for that site. This allows these loggers to recover from QREST outages without resulting in data gaps.
  - **Sitewide documentation** can now be added at the Site Edit page
  - **Dashboard color change:** On the Admin dashboard, the color coding for "UTC Polling in the future" from red to yellow so that true logger downtime (still in red) stands out more prominently
  - **Increase timeout allowance** when connecting to SUTRON and ZENO loggers to reduce the number of timed-out polls
- **2024-06-23**
  - **Favorite monitor:** Can now set your favorite (default) monitor on the dashboard page
  -  **Manual Import Fill Gap Feature:** when importing hourly data, after the import is complete QREST will check if there are any data gaps for the monitors imported within the import date range. These will be displayed on the Import Status page
  -  **View Import Status for past manual imports:** when viewing the Import History page, the status column is now clickable which brings you to the Import Status page for that past import. So now if a past import has gaps, you can return later to the status page to fill the missing data.
  -  **Import Bug fix:** fix a bug when someone would try to import fewer columns than appear in the import template, QREST would display error page. (For example an import template expects Ozone in column 8 but import file only has 6 columns.) QREST will now just ignore missing columns
  -  **Import History page date display error:** fix display of import data date range (was always displaying start and end dates as same) 

Changes made prior to 2024 can be found here: [Change Log Archive](https://github.com/open-environment/QREST/blob/master/CHANGELOG.md)


## Sending Feedback
We are always open to [your feedback](https://github.com/open-environment/QREST/issues).

