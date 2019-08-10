USE [QREST];
GO

--THIS SCRIPT POPULATES THE DATABASE WITH INITIAL DATA

--****************GENERAL APP SETTINGS  *****************************************************************************************
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_FROM','donotreply@qrest.io','The email address in the FROM line when sending emails from this application.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_SERVER','smtp.sendgrid.net','The SMTP email server used to allow this application to send emails.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_PORT','25','The port used to access the SMTP email server.',0,GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('EMAIL_SECURE_USER','smtp@change.me','If the SMTP server requires authentication, this is the SMTP server username.','',GetDate(),true);
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT],ENCRYPT_IND) VALUES ('EMAIL_SECURE_PWD','change.me','If the SMTP server requires authentication, this is the SMTP server password or API KEY.','',GetDate(),true);

INSERT INTO T_QREST_APP_SETTINGS_CUSTOM ([TERMS_AND_CONDITIONS],[ANNOUNCEMENTS]) values ('<p>The access and use of the QREST requires the creation of a user ID and password that you must maintain and keep confidential.</p>	
<p>By proceeding, you acknowledge that you fully understand and consent to all of the following:</p>	
<ul><li>Any communications or information used, transmitted, or stored on QREST may be used or disclosed for any lawful government purpose, including but not limited to, administrative purposes, penetration testing, communication security monitoring, personnel misconduct measures, law enforcement, and counterintelligence inquiries</li>	
<li>At any time, parties may for any lawful government purpose, without notice, monitor, intercept, search, and seize any authorized or unauthorized communication or information used or stored on QREST</li>	
</ul><p>&nbsp;</p>	
<p><strong>Privacy Statement</strong><br> Personal identifying information you provide will be used for the expressed purpose of registration to this site and for updating and correcting agency information as necessary. This information will not be made available for other purposes unless required by law. Your information will not be sold or otherwise transferred to an outside third party.</p>','');


--****************ONLINE HELP*****************************************************************************************
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('Users Guide',1,'<p>The QREST User''s Guide can be found here: <a href="#">User''s Guide</a></p>');
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('FAQ',2,'<p>May be provided later.</p>');
INSERT INTO T_QREST_HELP_DOCS(HELP_TITLE,SORT_SEQ,HELP_HTML) values ('Tutorial Videos',3,'<p>May be provided later.</p>');



--****************T_PRT_REF_EMAIL_TEMPLATE ************************************************************************************
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('EMAIL_CONFIRM','Sent to users to allow them to confirm their email address as part of activating their account.','Verify Your Email','Please verify your QREST account by clicking this link: <a href=''{callbackUrl}''>Verify Account</a>');
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('RESET_PASSWORD','Reset a user''s password','QREST - Reset Password','Please reset your QREST password by clicking here: <a href=''{callbackUrl}''>Reset Password</a>');
INSERT INTO T_QREST_EMAIL_TEMPLATE ([EMAIL_TEMPLATE_NAME],[EMAIL_TEMPLATE_DESC], [SUBJ], [MSG]) VALUES ('ACCESS_REQUEST','QREST Access Request','QREST - Access Request','Someone has requested access to your organization in QREST. The user {UserName} has requested to join your organization {orgName}. Please go to the dashboard in QREST to approve or deny their request.');




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




--****************REF_STATE *****************************************************************************************
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');
--INSERT INTO [T_QREST_REF_STATE] ([STATE_CD],[STATE_NAME]) VALUES ('','');


--GO


--****************REF_COLLECT_FREQ*******************************************************************************



--****************ROLES *****************************************************************************************
INSERT INTO [T_QREST_ROLES] ([ROLE_IDX], [Name]) 
  VALUES (NEWID(), 'ADMIN'); --'Global administration role for QREST, spanning all organizations.'

GO



--****************ORGANIZATIONS *****************************************************************************************



--****************ORG_EMAIL ************************************************************************************



