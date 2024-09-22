# Changelog (since June 2024 only)

All notable changes to this project will be documented in this file.

## 2024-08-18

### Bugs Fixed
- **Online Help Performance:** Improved performance of loading the online help


## 2024-07-27

### Added

- **Sitewide documentation** can now be added at the Site Edit page
- **Dashboard color change:** On the Admin dashboard, the color coding for "UTC Polling in the future" from red to yellow so that true logger downtime (still in red) stands out more prominently

### Bugs Fixed

### Changed

- **Polling Logic Change for ZENO & SUTRON loggers:** now retrieves data based on the latest date of data in QREST. Old logic was to grab 12 latest records from logger. Now it pulls up to 99 records starting with the last QREST stored data for that site. This allows these loggers to recover from QREST outages without resulting in data gaps. 
- **Increase timeout allowance** when connecting to SUTRON and ZENO loggers to reduce the number of timed-out polls

## 2024-06-23

### Added

- **Favorite monitor:** Can now set your favorite (default) monitor on the dashboard page
- **Manual Import Fill Gap Feature:** when importing hourly data, after the import is complete QREST will check if there are any data gaps for the monitors imported within the import date range. These will be displayed on the Import Status page
- **View Import Status for past imports:** when viewing the Import History page, the status column is now clickable which brings you to the Import Status page for that past import. So now if a past import has gaps, you can return later to the status page to fill the missing data.

### Bugs Fixed

- **Import Bug fix:** fix a bug when someone would try to import fewer columns than appear in the import template, QREST would display error page. (For example an import template expects Ozone in column 8 but import file only has 6 columns.) QREST will now just ignore missing columns
- **Import History page date display error:** fix display of import data date range (was always displaying start and end dates as same) 

