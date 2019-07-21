USE [QREST];
GO

--THIS SCRIPT POPULATES THE DATABASE WITH INITIAL DATA

--****************GENERAL APP SETTINGS  *****************************************************************************************
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_FROM','donotreply@qrest.io','The email address in the FROM line when sending emails from this application.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_SERVER','smtp.sendgrid.net','The SMTP email server used to allow this application to send emails.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_PORT','25','The port used to access the SMTP email server.',0,GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_SECURE_USER','smtp@change.me','If the SMTP server requires authentication, this is the SMTP server username.','',GetDate());
INSERT INTO T_QREST_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USER_IDX],[MODIFY_DT]) VALUES ('EMAIL_SECURE_PWD','change.me','If the SMTP server requires authentication, this is the SMTP server password or API KEY.','',GetDate());

INSERT INTO T_QREST_APP_SETTINGS_CUSTOM ([TERMS_AND_CONDITIONS],[ANNOUNCEMENTS]) values ('<p>The access and use of the QREST requires the creation of a user ID and password that you must maintain and keep confidential.</p>	
<p>By proceeding, you acknowledge that you fully understand and consent to all of the following:</p>	
<ul><li>Any communications or information used, transmitted, or stored on QREST may be used or disclosed for any lawful government purpose, including but not limited to, administrative purposes, penetration testing, communication security monitoring, personnel misconduct measures, law enforcement, and counterintelligence inquiries</li>	
<li>At any time, parties may for any lawful government purpose, without notice, monitor, intercept, search, and seize any authorized or unauthorized communication or information used or stored on QREST</li>	
</ul><p>&nbsp;</p>	
<p><strong>Privacy Statement</strong><br> Personal identifying information you provide will be used for the expressed purpose of registration to this site and for updating and correcting agency information as necessary. This information will not be made available for other purposes unless required by law. Your information will not be sold or otherwise transferred to an outside third party.</p>','');


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



