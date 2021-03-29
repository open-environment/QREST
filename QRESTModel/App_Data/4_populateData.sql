USE [QREST];
GO

--THIS SCRIPT POPULATES THE DATABASE WITH INITIAL DATA

--****************GENERAL APP SETTINGS  *****************************************************************************************
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_FROM','donotreply@qrest.net','The email address in the FROM line when sending emails from this application.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_SERVER','smtp.sendgrid.net','The SMTP email server used to allow this application to send emails.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_PORT','25','The port used to access the SMTP email server.',0,GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('EMAIL_SECURE_USER','smtp@change.me','If the SMTP server requires authentication, this is the SMTP server username.','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('EMAIL_SECURE_PWD','change.me','If the SMTP server requires authentication, this is the SMTP server password or API KEY.','',GetDate(),1);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('SMS_SID','change.me','Unique identification for a Twilio account, needed for QREST to send text messages.','',GetDate(),1);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('SMS_AUTH_TOKEN','change.me','Authentication token for a Twilio account, needed for QREST to send text messages.','',GetDate(),1);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('SMS_PHONE_NUM','change.me','Text messages sent from QREST will originate from this number. This must be a number purchased through Twilio.','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('NUM_POLL_RECS','12','Default number of records to poll from data loggers when performing automated polling.','',GetDate(),0);

INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('AIRNOW_ACTIVE_IND','1','Set to 1 to globally allow AirNow service','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('AIRNOW_FTP_USER','change.me','Global username used for FTPing files to EPA AirNow.','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('AIRNOW_FTP_PWD','change.me','Global password used for FTPing files to EPA AirNow.','',GetDate(),1);

INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('CDX_URL','change.me','Website URL for the Exchange Network Node (CDX).','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('CDX_GLOBAL_USER','change.me','Exchange Network username, if none supplied by tribe.','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('CDX_GLOBAL_PWD','change.me','Exchange Network password, if none supplied by tribe.','',GetDate(),1);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('ENVIRONMENT','PROD','Sets this instance of QREST as either TEST or PROD','',GetDate(),0);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('TEST_URL','https://qrest-test.azurewebsites.net/','Defines the URL of the testing site','',GetDate(),0);



INSERT INTO T_QREST_APP_SETTINGS_CUSTOM ([TERMS_AND_CONDITIONS],[ANNOUNCEMENTS]) values ('<p>The access and use of the QREST requires the creation of a user ID and password that you must maintain and keep confidential.</p>	
<p>By proceeding, you acknowledge that you fully understand and consent to all of the following:</p>	
<ul><li>Any communications or information used, transmitted, or stored on QREST may be used or disclosed for any lawful government purpose, including but not limited to, administrative purposes, penetration testing, communication security monitoring, personnel misconduct measures, law enforcement, and counterintelligence inquiries</li>	
<li>At any time, parties may for any lawful government purpose, without notice, monitor, intercept, search, and seize any authorized or unauthorized communication or information used or stored on QREST</li>	
</ul><p>&nbsp;</p>	
<p><strong>Privacy Statement</strong><br> Personal identifying information you provide will be used for the expressed purpose of registration to this site and for updating and correcting agency information as necessary. This information will not be made available for other purposes unless required by law. Your information will not be sold or otherwise transferred to an outside third party.</p>','');


--****************TASKS ************************************************************************************
INSERT INTO [dbo].[T_QREST_APP_TASKS]([TASK_NAME],[TASK_DESC],[FREQ_TYPE],[FREQ_NUM],[LAST_RUN_DT],[NEXT_RUN_DT],[STATUS],[MODIFY_USER_IDX],[MODIFY_DT])
     VALUES ('LoggerPolling','Polls data loggers to retrieve data','M',5,GetDate(),GetDate(),'Stopped',null,GetDate());
INSERT INTO [dbo].[T_QREST_APP_TASKS]([TASK_NAME],[TASK_DESC],[FREQ_TYPE],[FREQ_NUM],[LAST_RUN_DT],[NEXT_RUN_DT],[STATUS],[MODIFY_USER_IDX],[MODIFY_DT])
     VALUES ('HourlyValidation','Performs Level 0 validation on hourly data','M',60,GetDate(),GetDate(),'Stopped',null,GetDate());
INSERT INTO [dbo].[T_QREST_APP_TASKS]([TASK_NAME],[TASK_DESC],[FREQ_TYPE],[FREQ_NUM],[LAST_RUN_DT],[NEXT_RUN_DT],[STATUS],[MODIFY_USER_IDX],[MODIFY_DT])
     VALUES ('AirNow','Sends data to AirNow ftp folder','M',60,GetDate(),GetDate(),'Stopped',null,GetDate());
INSERT INTO [dbo].[T_QREST_APP_TASKS]([TASK_NAME],[TASK_DESC],[FREQ_TYPE],[FREQ_NUM],[LAST_RUN_DT],[NEXT_RUN_DT],[STATUS],[MODIFY_USER_IDX],[MODIFY_DT])
     VALUES ('Import','Background task that processes any large import files','M',240,GetDate(),'12/12/2050','Stopped',null,GetDate());


--****************ONLINE HELP*****************************************************************************************
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('Users Guide',1,'<p>The QREST User''s Guide can be found here: <a href="#">User''s Guide</a></p>');
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('FAQ',2,'<p>May be provided later.</p>');
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('Tutorial Videos',3,'<p>May be provided later.</p>');



--****************T_PRT_REF_EMAIL_TEMPLATE ************************************************************************************
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('EMAIL_CONFIRM','Sent to users to allow them to confirm their email address as part of activating their account.','Verify Your Email','Please verify your QREST account by clicking this link: <a href=''{callbackUrl}''>Verify Account</a>');
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('RESET_PASSWORD','Reset a user''s password','QREST - Reset Password','Please reset your QREST password by clicking here: <a href=''{callbackUrl}''>Reset Password</a>');
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('ACCESS_REQUEST','QREST Access Request','QREST - Access Request','Someone has requested access to your organization in QREST. The user {UserName} has requested to join your organization {orgName}. Please go to the dashboard in QREST to approve or deny their request. Click this link to manage: {siteUrl}');
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('POLLING_ALERT','QREST Polling Alerts','QREST Polling Alert','The following QREST alert(s) for hourly polled data have been triggered: {notifyMsg}');




--****************EPA REGION *****************************************************************************************
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (1,'EPA Region 1');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (2,'EPA Region 2');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (3,'EPA Region 3');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (4,'EPA Region 4');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (5,'EPA Region 5');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (6,'EPA Region 6');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (7,'EPA Region 7');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (8,'EPA Region 8');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (9,'EPA Region 9');
INSERT INTO [T_QREST_REF_REGION] ([EPA_REGION],[EPA_REGION_NAME]) VALUES (10,'EPA Region 10');


GO


--****************EPA REGION *****************************************************************************************
INSERT INTO [T_QREST_REF_TIMEZONE] ([TZ_CODE],[TZ_NAME]) VALUES ('-8','Pacific Standard Time');
INSERT INTO [T_QREST_REF_TIMEZONE] ([TZ_CODE],[TZ_NAME]) VALUES ('-7','Mountain Standard Time');
INSERT INTO [T_QREST_REF_TIMEZONE] ([TZ_CODE],[TZ_NAME]) VALUES ('-6','Central Standard Time');
INSERT INTO [T_QREST_REF_TIMEZONE] ([TZ_CODE],[TZ_NAME]) VALUES ('-5','Eastern Standard Time');


GO


--****************REF_ASSESS_TYPE *****************************************************************************************
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('1-Point QC','40 CFR Part 58 Appendix A §3.2.1 regulations require the primary quality assurance organization (PQAO) to perform a quality control check at least once every 2 weeks at required ranges (0.01 – 0.1ppm for SO2, NO2 and O3; and 1 – 10 ppm for CO) on every automated gaseous monitor. The “check standard” is a gas concentration generated from standard reference materials. In the case of O3, the “check standard” concentration is generated using an ozone generator.
The monitors for which this assessment applies usually sample continuously and report hourly average values as raw data to AQS. When it is time to run the 1-Point QC check, the check standard is introduced in the monitor, and readings from the monitor recorded. The monitor concentration and the check standard concentration are reported to AQS for the monitor and date. A null data code is included on the raw data transaction for that hour if a routine observation cannot be made for the hour due to performing the 1-Point QC check, and will indicate that the 1-Point QC check was performed.
For 1-Point QC Checks, the known value of the check gas concentration is referred to as the Assessment Concentration on the transaction. The value indicated by the monitor as a result of sampling the standard reference gas is referred to as the Monitor Concentration on the transaction. This should help alleviate some of the confusion that arose from the terms “actual” and “indicated” on the old RP and RA transactions.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('Annual PE','40 CFR Part 58 Appendix A §3.2.2 regulations require the PQAO, each calendar quarter (during which monitors are operated), evaluate at least 25 percent of the SLAMS monitors for SO2, NO2, O3, or CO such that each monitor is evaluated at least once per year. The evaluation is made by challenging the monitor with audit gas standards of known concentration from at least three of ten audit levels2. The audit levels selected should represent or bracket 80 percent of ambient concentrations measured by the monitor being evaluated.
Each audit level has a minimum and maximum concentration range, with the lowest range defined as audit level 1. For the criteria gases, 10 audit level ranges are currently defined.
AQS evaluates each reported assessment concentration on the transaction with its reported level number. If the concentration is not within the level’s concentration range, then AQS will produce a warning message when processing the transaction.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('Flow Rate Verification','40 CFR Part 58 Appendix A §3.2.3, 3.3.2 and 3.3.4.1 describe requirements for a one-point flow rate verification check on automated and manual monitors used to measure PM10, PM10-2.5, PM2.5, and Pb (Pb-TSP and Pb-PM10). To perform the flow rate verification check, the monitor’s normal flow rate is checked using a certified flow rate transfer standard. This check is performed at different frequencies depending on the type of monitor being used. For manual method hi-vol samplers, flow rates must be verified on at least a quarterly basis. For manual method lo-vol samplers, flow rates must be verified on at least a monthly basis. For automated methods, flow rates must also be verified monthly.
Particulate monitors using automated methods usually sample continuously and report hourly average values as raw data to AQS. When this check is performed, if the hourly average raw data value is not available for reporting, then a null value and null data code is reported for that hour. The known flow rate of the transfer standard, and the measured (or indicated) value from the monitor, are recorded by the operator for entry into AQS for that monitor on that date. For manual monitors, the check is performed prior to or after sampling so there is no data loss.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('Semi-Annual Flow Rate Audit','40 CFR Part 58 Appendix A §3.2.4, 3.3.3, 3.3.4 and 3.3.4.1 describe requirements for flow rate audits to be performed on each monitor used to measure PM10, PM10-2.5, PM2.5 and Pb. The Semi-Annual Flow Rate Audits should be performed at least every 6 months. To perform the audit, the monitor’s normal flow rate is checked using a certified flow rate transfer standard which is different from the one used for calibrating the monitor. The auditing agency conducting the Semi-Annual Flow Rate Audit may be the PQAO, or may be an independent agency. In any event, the assessment should be conducted by other than the routine site operator.
Particulate monitors using automated methods usually sample continuously and report hourly average values as raw data to AQS. When this audit is performed, the hourly average raw data value may not be available for reporting, in which case a null value and null data code are reported for that hour. The flow standard value (known flow rate of the transfer standard), and the monitor value (response value indicated by monitor) are recorded by the operator for entry into AQS for the monitor being assessed, for that date. For manual monitors the assessment can be performed prior to or after sampling so there is no data loss.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('PMc Flow Rate V','40 CFR Part 58 Appendix A §3.2.3 describes requirements for performing a one-point flow rate verification check on automated and manual monitors used to measure PMc (PMc stands for PM-coarse, another way to say PM10-2.5). Because PMc data is often generated by calculating the difference in values between a PM10 sampler and a PM2.5 sampler, this transaction includes additional fields to report both flows.
To perform the flow rate verification check, each monitor’s normal flow rate is checked using a certified flow rate transfer standard. This check is performed on at least a monthly basis. The Flow Rate Verification for PMc transaction includes additional fields to allow the reporting of both the PM10 and PM2.5 samplers’ indicated flow rate, as well as the true flow rate of the standard flow meter. The transaction also includes fields to report the method code for both samplers.
Particulate monitors using automated methods usually sample continuously and report hourly average values as raw data to AQS. If automated monitors are in use, and if hourly average values are not available for reporting as raw data due to a flow rate verification, then a null value and null data code is reported as the raw data record for that hour. The known flow rate of the transfer standard, and the measured (or indicated) value from the monitor are reported using the flow rate verification for PMc transaction.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('PMc Semi Annual Flow Rate Audit','40 CFR Part 58 Appendix A §3.2.4 describes requirements for semi-annual flow rate audits on automated and manual monitors used to measure PMc (PMc stands for PM-coarse, another way to say PM10-2.5). Because PMc data is often generated by calculating the difference in values between a PM10 sampler and a PM2.5 sampler, this transaction includes additional fields to report both flows.
To perform the semi-annual flow rate audit, each monitor’s normal flow rate is checked using a certified flow rate transfer standard. This check is performed on at least a semi-annual basis. The Semi-Annual Flow Rate Audit for PMc transaction includes additional fields to allow the reporting of both the PM10 and PM2.5 samplers’ indicated flow rate, as well as the true flow rate of the standard flow meter. The transaction also includes fields to report the method code for both samplers.
Particulate monitors using automated methods usually sample continuously and report hourly average values as raw data to AQS. If automated monitors are in use, and if hourly average values are not available for reporting as raw data due to a flow rate verification, then a null value and null data code is reported as the raw data record for that hour. The known flow rate of the transfer standard, and the measured (or indicated) value from the monitor are reported using the flow rate verification for PMc transaction.');
INSERT INTO [T_QREST_REF_ASSESS_TYPE] ([ASSESSMENT_TYPE],[ASSESSMENT_TYPE_DESC]) VALUES ('Zero Span','Zero and Span are two different measurements that are quality control checks. The zero check is a challenge of the instruments zero calibration conducted by introducing zero air into the analyzer and measuring the instrument response. The span check is a challenge of the upper limit of the analyzers calibration conducted by introducing a calibration gas (approximately 80% of the operating range) into the analyzer.');






--****************ROLES *****************************************************************************************
INSERT INTO [T_QREST_ROLES] ([ROLE_IDX], [Name]) VALUES (NEWID(), 'GLOBAL ADMIN'); --'Global administration role for QREST, spanning all organizations.'
--INSERT INTO [T_QREST_ROLES] ([ROLE_IDX], [Name]) VALUES (NEWID(), 'CERTIFIED QA REVIEWER'); 

GO


--****************REF_PAR_UNITS *****************************************************************************************
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42401');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42401');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42402');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42402');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42600');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42600');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42601');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42601');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42602');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42602');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','42603');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','42603');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('007','44201');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('008','44201');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('011','61101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('012','61101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('013','61101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('060','61101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('014','61102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('011','61103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('012','61103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('013','61103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('060','61103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('014','61104');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('014','61106');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('015','62101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('017','62101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('037','62101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('015','62103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('017','62103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('037','62103');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('015','62107');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('017','62107');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('037','62107');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('019','62201');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('018','63301');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('025','63301');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('079','63301');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('016','64101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('022','64101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('059','64101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('021','65102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('029','65102');

INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('073','68101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('083','68101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('118','68101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('119','68101');

INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','81102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','81102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','81102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','81102');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','85101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','85101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','85101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','85101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','88101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','88101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','88101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','88101');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','88500');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','88500');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','88500');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','88500');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','88501');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','88501');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','88501');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','88501');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('001','88502');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('005','88502');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('105','88502');
INSERT INTO [T_QREST_REF_PAR_UNITS] ([UNIT_CODE],[PAR_CODE]) VALUES ('109','88502');

GO


--****************REF_ACCESS_LEVEL *****************************************************************************************
INSERT INTO [T_QREST_REF_ACCESS_LEVEL] ([ACCESS_LEVEL],[ACCESS_LEVEL_DESC]) VALUES ('A','Admin');
INSERT INTO [T_QREST_REF_ACCESS_LEVEL] ([ACCESS_LEVEL],[ACCESS_LEVEL_DESC]) VALUES ('R','Read-Only');
INSERT INTO [T_QREST_REF_ACCESS_LEVEL] ([ACCESS_LEVEL],[ACCESS_LEVEL_DESC]) VALUES ('U','Operator');
INSERT INTO [T_QREST_REF_ACCESS_LEVEL] ([ACCESS_LEVEL],[ACCESS_LEVEL_DESC]) VALUES ('Q','QA Reviewer');


--****************T_QREST_REF_USER_STATUS *****************************************************************************************
INSERT INTO [T_QREST_REF_USER_STATUS] ([STATUS_IND],[STATUS_IND_DESC]) VALUES ('A','Active');
INSERT INTO [T_QREST_REF_USER_STATUS] ([STATUS_IND],[STATUS_IND_DESC]) VALUES ('P','Pending');
INSERT INTO [T_QREST_REF_USER_STATUS] ([STATUS_IND],[STATUS_IND_DESC]) VALUES ('R','Rejected');


--****************ADDITIONAL DURATION NOT COMING FROM EPA *****************************************************************************************
insert into [T_QREST_REF_DURATION](DURATION_CODE, DURATION_DESC, ACT_IND, CREATE_DT) values ('G','1 MINUTE',1,GetDate());


--****************ADDITIONAL QUALIFIER NOT COMING FROM EPA *****************************************************************************************
insert into T_QREST_REF_QUALIFIER(QUAL_CODE,QUAL_DESC,QUAL_TYPE,CREATE_DT) values ('-1','<<REMOVE ANY>>','NULL',GetDate());




--****************QC STUFF *****************************************************************************************
    
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',1,0.004,0.0059);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',2,0.006,0.019);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',3,0.020,0.039);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',4,0.040,0.069);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',5,0.070,0.089);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',6,0.090,0.119);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',7,0.120,0.139);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',8,0.140,0.169);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',9,0.170,0.189);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('44201',10,0.190,0.259);

insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',1,0.0003,0.0029);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',2,0.0030,0.0049);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',3,0.0050,0.0079);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',4,0.0080,0.0199);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',5,0.0200,0.0499);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',6,0.0500,0.0999);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',7,0.1000,0.1499);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',8,0.1500,0.2599);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',9,0.2600,0.7999);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42401',10,0.8000,1.0000);

insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',1,0.0003,0.0029);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',2,0.0030,0.0049);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',3,0.0050,0.0079);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',4,0.0080,0.0199);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',5,0.0200,0.0499);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',6,0.0500,0.0999);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',7,0.1000,0.1499);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',8,0.1500,0.2599);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',9,0.2600,0.7999);
insert into [T_QREST_REF_QC_AUDIT_LVL] values ('42406',10,0.8000,1.0000);

--ozone
insert into T_QREST_REF_QC values ('44201','1-Point QC',0.0015,7, 0.005, 0.08);
--SO2
insert into T_QREST_REF_QC values ('42401','1-Point QC',0.0015,10, 0.005, 0.08);
insert into T_QREST_REF_QC values ('42406','1-Point QC',0.0015,10, 0.005, 0.08);

insert into T_QREST_REF_QC values ('44201','Annual PE',0.0015,15, null, null);
insert into T_QREST_REF_QC values ('42401','Annual PE',0.0015,15, null, null);
insert into T_QREST_REF_QC values ('42406','Annual PE',0.0015,15, null, null);

--PM 2.5
insert into T_QREST_REF_QC values ('81104','Flow Rate Verification',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88101','Flow Rate Verification',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88500','Flow Rate Verification',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88501','Flow Rate Verification',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88502','Flow Rate Verification',null,4.1, null, null);
--PM 10
insert into T_QREST_REF_QC values ('81102','Flow Rate Verification',null,7.1, null, null);
insert into T_QREST_REF_QC values ('85101','Flow Rate Verification',null,7.1, null, null);

--PM 2.5
insert into T_QREST_REF_QC values ('81104','Semi-Annual Flow Rate Audit',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88101','Semi-Annual Flow Rate Audit',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88500','Semi-Annual Flow Rate Audit',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88501','Semi-Annual Flow Rate Audit',null,4.1, null, null);
insert into T_QREST_REF_QC values ('88502','Semi-Annual Flow Rate Audit',null,4.1, null, null);
--PM 10
insert into T_QREST_REF_QC values ('81102','Semi-Annual Flow Rate Audit',null,7.1, null, null);
insert into T_QREST_REF_QC values ('85101','Semi-Annual Flow Rate Audit',null,7.1, null, null);