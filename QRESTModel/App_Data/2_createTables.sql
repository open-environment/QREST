﻿USE [QREST];
GO

/******************************************************************************************/
/*******************  SEEDING SYS TABLES ***********************************************/
/******************************************************************************************/
CREATE TABLE T_SYS_HR (
	HR datetime2(0) NOT NULL,
	CONSTRAINT [PK_T_SYS_HR] PRIMARY KEY (HR)
);
GO

DECLARE @MinDate DATETIME2(0) = '01/01/2019', 
	    @MaxDate DATETIME2(0) = '12/31/2030 23:00';

insert into T_SYS_HR(HR)
SELECT TOP (DATEDIFF(HOUR, @MinDate, @MaxDate) + 1)
    Date = DATEADD(HOUR, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate)
FROM sys.all_objects a
CROSS JOIN sys.all_objects b;


/******************************************************************************************/
/*******************  IDENTITY TABLES ***********************************************/
/******************************************************************************************/

CREATE TABLE [dbo].[T_QREST_ROLES](
    [ROLE_IDX] [nvarchar](128) NOT NULL,
    [Name] [nvarchar](256) NOT NULL,
    [ROLE_DESC] [varchar](100),
CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([ROLE_IDX] ASC)
)


CREATE TABLE [dbo].[T_QREST_USERS](
    [USER_IDX] [nvarchar](128) NOT NULL,
    [Email] [nvarchar](256) NULL,
    [EmailConfirmed] [bit] NOT NULL,
    [PasswordHash] [nvarchar](max),
    [SecurityStamp] [nvarchar](max),
    [PhoneNumber] [nvarchar](max),
    [PhoneNumberConfirmed] [bit] NOT NULL,
    [TwoFactorEnabled] [bit] NOT NULL,
    [LockoutEndDateUtc] [datetime],
    [LockoutEnabled] [bit] NOT NULL,
    [AccessFailedCount] [int] NOT NULL,
    [UserName] [nvarchar](256) NOT NULL,
    [FNAME] [varchar](40) NOT NULL,
    [LNAME] [varchar](40) NOT NULL,
    [TITLE] [varchar](50),
    [NOTIFY_APP_IND] [bit],
    [NOTIFY_EMAIL_IND] [bit],
    [NOTIFY_SMS_IND] [bit],
	[LAST_LOGIN_DT] datetime2(0),
	[CREATE_USER_IDX] [nvarchar](128) NULL,
	[CREATE_DT] [datetime2](0) NULL,
	[MODIFY_USER_IDX] [nvarchar](128) NULL,
	[MODIFY_DT] [datetime2](0) NULL,
CONSTRAINT [PK_T_QREST_USERS] PRIMARY KEY CLUSTERED ([USER_IDX] ASC)
)

CREATE TABLE [dbo].[T_QREST_USER_CLAIMS](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [nvarchar](128) NOT NULL,
    [ClaimType] [nvarchar](max) NULL,
    [ClaimValue] [nvarchar](max) NULL,
CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
CONSTRAINT [FK_AspNetUserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [T_QREST_USERS] ([USER_IDX]) ON DELETE CASCADE
)


CREATE TABLE [dbo].[T_QREST_USER_LOGINS](
    [LoginProvider] [nvarchar](128) NOT NULL,
    [ProviderKey] [nvarchar](128) NOT NULL,
    [UserId] [nvarchar](128) NOT NULL,
CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
CONSTRAINT [FK_AspNetUserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [T_QREST_USERS] ([USER_IDX]) ON DELETE CASCADE
)

CREATE TABLE [dbo].[T_QREST_USER_ROLES](
    [USER_IDX] [nvarchar](128) NOT NULL,
    [ROLE_IDX] [nvarchar](128) NOT NULL,
CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([USER_IDX] ASC, [ROLE_IDX] ASC),
CONSTRAINT [FK_AspNetUserRoles_Roles_RoleId] FOREIGN KEY ([ROLE_IDX]) REFERENCES [T_QREST_ROLES] ([ROLE_IDX]) ON DELETE CASCADE,
CONSTRAINT [FK_AspNetUserRoles_Users_UserId] FOREIGN KEY ([USER_IDX]) REFERENCES [T_QREST_USERS] ([USER_IDX]) ON DELETE CASCADE
)



GO



/******************************************************************************************/
/*******************  REF DATA TABLES ***********************************************/
/******************************************************************************************/


CREATE TABLE [dbo].[T_QREST_SYS_LOG](
	[LOG_ID] [int] NOT NULL IDENTITY(1,1),
	[LOG_DT] [datetime2](0) NULL,
	[LOG_TYP] [varchar](15) NULL,
	[LOG_USERID] [varchar](128) NULL,
	[LOG_MSG] [varchar](2000) NULL,
 CONSTRAINT [PK_T_QREST_SYS_LOG] PRIMARY KEY CLUSTERED  ([LOG_ID] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_SYS_LOG_EMAIL](
	[LOG_EMAIL_ID] [int] IDENTITY(1,1) NOT NULL,
	[EMAIL_DT] [datetime2](0) NULL,
	[EMAIL_FROM] [varchar](200) NULL,
	[EMAIL_TO] [varchar](200) NULL,
	[EMAIL_CC] [varchar](200) NULL,
	[EMAIL_SUBJ] [varchar](200) NULL,
	[EMAIL_MSG] [varchar](2000) NULL,
	[EMAIL_TYPE] [varchar](15) NULL,
 CONSTRAINT [PK_T_QREST_SYS_LOG_EMAIL] PRIMARY KEY CLUSTERED  ([LOG_EMAIL_ID] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_SYS_LOG_ACTIVITY](
	[LOG_ACTIVITY_IDX] [int] IDENTITY(1,1) NOT NULL,
	[ACTIVITY_DT] [datetime2](0) NULL,
	[ACTIVITY_TYPE] [varchar](25) NOT NULL,
	[ACTIVITY_USER] [varchar](256) NULL,
	[ACTIVITY_DESC] [varchar](2000) NULL,
	[IP_ADDRESS] [varchar](100) NULL,
	[SUPPORTING_ID] [varchar](50) NULL,
 CONSTRAINT [PK_T_QREST_SYS_LOG_ACTIVITY] PRIMARY KEY CLUSTERED  ([LOG_ACTIVITY_IDX] ASC)
) ON [PRIMARY]



GO



CREATE TABLE [dbo].[T_QREST_APP_SETTINGS](
	[SETTING_IDX] [int] IDENTITY(1,1) NOT NULL,
	[SETTING_NAME] [varchar](100) NOT NULL,
	[SETTING_DESC] [varchar](500),
	[SETTING_VALUE] [varchar](200),
	[SETTING_CAT] [varchar](20),
	[ENCRYPT_IND] [bit],
	[MODIFY_USER_IDX] [varchar](128),
	[MODIFY_DT] [datetime2](0),
 CONSTRAINT [PK_T_QREST_APP_SETTINGS] PRIMARY KEY CLUSTERED  ([SETTING_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_APP_SETTINGS_CUSTOM](
	[SETTING_CUSTOM_IDX] [int] IDENTITY(1,1) NOT NULL,
	[TERMS_AND_CONDITIONS] [varchar](max),
	[ANNOUNCEMENTS] [varchar](max),
 CONSTRAINT [PK_T_QREST_APP_SETTINGS_CUSTOM] PRIMARY KEY CLUSTERED ([SETTING_CUSTOM_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_APP_TASKS](
	[TASK_IDX] [int] IDENTITY(1,1) NOT NULL,
	[TASK_NAME] [varchar](50) NOT NULL,
	[TASK_DESC] [varchar](1000) NULL,
	[FREQ_TYPE] [char](1) NULL,
	[FREQ_NUM] [int] NOT NULL,
	[LAST_RUN_DT] [datetime2](7) NOT NULL,
	[NEXT_RUN_DT] [datetime2](7) NOT NULL,
	[STATUS] [varchar](50) NULL,
	[MODIFY_USER_IDX] [varchar](128),
	[MODIFY_DT] [datetime2](0),
 CONSTRAINT [PK_T_QREST_APP_TASKS] PRIMARY KEY CLUSTERED  ([TASK_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_HELP_DOCS](
	[HELP_IDX] [int] IDENTITY(1,1) NOT NULL,
	[HELP_TITLE] [varchar](40) NOT NULL,
	[HELP_HTML] [varchar](max) NOT NULL,
	[SORT_SEQ] [int] NOT NULL,
	[HELP_CAT] varchar(50) NULL,
 CONSTRAINT [PK_T_QREST_HELP_DOCS] PRIMARY KEY CLUSTERED ([HELP_IDX] ASC)
) ON [PRIMARY]


GO



CREATE TABLE [dbo].[T_QREST_EMAIL_TEMPLATE](
	[EMAIL_TEMPLATE_ID] [int] IDENTITY(1,1) NOT NULL,
	[EMAIL_TEMPLATE_NAME] [varchar](60) NOT NULL,
	[EMAIL_TEMPLATE_DESC] [varchar](1000) NOT NULL,
	[SUBJ] [varchar](200) NULL,
	[MSG] [varchar](max) NULL,
	[MODIFY_USER_IDX] [varchar](128) NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_QREST_EMAIL_TEMPLATE] PRIMARY KEY CLUSTERED  ([EMAIL_TEMPLATE_ID] ASC)
) ON [PRIMARY]

GO



CREATE TABLE [T_QREST_REF_STATE](
	[STATE_CD] [varchar](2) NOT NULL,
	[STATE_NAME] [varchar](100) NOT NULL,
	[STATE_ABBR] [varchar](2) NOT NULL,
 CONSTRAINT [PK_T_QREST_REF_STATE] PRIMARY KEY CLUSTERED  ([STATE_CD] ASC)
);

GO


CREATE TABLE [T_QREST_REF_COUNTY](
	[STATE_CD] [varchar](2) NOT NULL,
	[COUNTY_CD] [varchar](3) NOT NULL,
	[COUNTY_NAME] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_QREST_REF_COUNTY] PRIMARY KEY CLUSTERED  ([STATE_CD] ASC, [COUNTY_CD] ASC),
 CONSTRAINT [FK_T_QREST_REF_COUNTY_S] FOREIGN KEY ([STATE_CD]) REFERENCES [T_QREST_REF_STATE] ([STATE_CD])
);

GO


CREATE TABLE [T_QREST_REF_ACCESS_LEVEL](
	[ACCESS_LEVEL] [varchar](1) NOT NULL,
	[ACCESS_LEVEL_DESC] [varchar](100) NULL,
 CONSTRAINT [PK_T_QREST_REF_ACCESS_LEVEL] PRIMARY KEY CLUSTERED  ([ACCESS_LEVEL] ASC)
);

GO


CREATE TABLE [T_QREST_REF_USER_STATUS](
	[STATUS_IND] [varchar](1) NOT NULL,
	[STATUS_IND_DESC] [varchar](20) NULL,
 CONSTRAINT [PK_T_QREST_REF_USER_STATUS] PRIMARY KEY CLUSTERED  ([STATUS_IND] ASC)
);

GO

CREATE TABLE [T_QREST_REF_AQS_AGENCY](
	[AQS_AGENCY_CODE] [varchar](4) NOT NULL,
	[AQS_AGENCY_NAME] [varchar](100) NOT NULL,
	[AQS_AGENCY_TYPE] [varchar](1) NOT NULL,
 CONSTRAINT [PK_T_QREST_REF_AQS_AGENCY] PRIMARY KEY CLUSTERED  (AQS_AGENCY_CODE ASC)
);

GO

CREATE TABLE [T_QREST_REF_REGION](
	[EPA_REGION] [int] NOT NULL,
	[EPA_REGION_NAME] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_QREST_REF_REGION] PRIMARY KEY CLUSTERED  ([EPA_REGION] ASC)
);

GO

CREATE TABLE [T_QREST_REF_TIMEZONE](
	[TZ_CODE] [varchar](4) NOT NULL,
	[TZ_NAME] [varchar](40) NOT NULL,
 CONSTRAINT [PK_T_QREST_REF_TIMEZONE] PRIMARY KEY CLUSTERED  ([TZ_CODE] ASC)
);

GO

CREATE TABLE [T_QREST_REF_DURATION] (
	[DURATION_CODE] varchar(1) NOT NULL,
	[DURATION_DESC] varchar(50) NOT NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_DURATION] PRIMARY KEY ([DURATION_CODE])
);

GO


CREATE TABLE [T_QREST_REF_COLLECT_FREQ] (
	[COLLECT_FREQ_CODE] varchar(2) NOT NULL,
	[COLLECT_FEQ_DESC] varchar(50) NOT NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_COLLECT_FREQ] PRIMARY KEY ([COLLECT_FREQ_CODE])
);

GO


CREATE TABLE [T_QREST_REF_UNITS] (
	[UNIT_CODE] varchar(3) NOT NULL,
	[UNIT_DESC] varchar(50) NOT NULL,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_UNITS] PRIMARY KEY ([UNIT_CODE])
);

GO


CREATE TABLE [T_QREST_REF_PARAMETERS] (
	[PAR_CODE] varchar(6) NOT NULL,
	[PAR_NAME] varchar(100) NOT NULL,
	[PAR_NAME_ALT] varchar(100) NULL,
	[CAS_NUM] varchar(100),
	[STD_UNIT_CODE] varchar(3),
	[SHORTLIST_IND] [bit] NOT NULL DEFAULT 0,
	[AQS_IND] [bit] NOT NULL DEFAULT 1,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_PARAMETERS] PRIMARY KEY ([PAR_CODE]),
    CONSTRAINT [FK_T_QREST_REF_PARAMETERS_U] FOREIGN KEY ([STD_UNIT_CODE]) REFERENCES [T_QREST_REF_UNITS] ([UNIT_CODE])
);


GO


CREATE TABLE [T_QREST_REF_PAR_UNITS] (
	[UNIT_CODE] varchar(3) NOT NULL,
	[PAR_CODE] varchar(6) NOT NULL,
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_PAR_UNITS] PRIMARY KEY ([UNIT_CODE],[PAR_CODE])
);

GO


CREATE TABLE [T_QREST_REF_PAR_METHODS] (
	[PAR_METHOD_IDX] UNIQUEIDENTIFIER NOT NULL,
	[PAR_CODE] varchar(6) NOT NULL,
	[METHOD_CODE] varchar(4) NOT NULL,
	[RECORDING_MODE] varchar(20),
	[COLLECTION_DESC] varchar(200),
	[ANALYSIS_DESC] varchar(200),
	[REFERENCE_METHOD_ID] varchar(200),
	[EQUIVALENT_METHOD] varchar(200),
	[STD_UNIT_CODE] varchar(3),
	[FED_MDL] float,
	[MIN_VALUE] float,
	[MAX_VALUE] float,
	[CUST_MIN_VALUE] float,
	[CUST_MAX_VALUE] float,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_PAR_METHODS] PRIMARY KEY ([PAR_METHOD_IDX]),
	CONSTRAINT [FK_T_QREST_REF_PAR_METHODS_P] FOREIGN KEY ([PAR_CODE]) REFERENCES [T_QREST_REF_PARAMETERS] ([PAR_CODE]),
	CONSTRAINT [FK_T_QREST_REF_PAR_METHODS_U] FOREIGN KEY ([STD_UNIT_CODE]) REFERENCES [T_QREST_REF_UNITS] ([UNIT_CODE])
);

GO


CREATE TABLE [T_QREST_REF_ASSESS_TYPE] (
	[ASSESSMENT_TYPE] varchar(30) NOT NULL,
	[ASSESSMENT_TYPE_DESC] varchar(max),
    CONSTRAINT [PK_T_QREST_REF_ASSESS_TYPE] PRIMARY KEY ([ASSESSMENT_TYPE])
);

GO


CREATE TABLE [T_QREST_REF_QUALIFIER] (
	[QUAL_CODE] varchar(2) NOT NULL,
	[QUAL_DESC] varchar(150),
	[QUAL_TYPE] varchar(10) NOT NULL,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_REF_QUALIFIER] PRIMARY KEY ([QUAL_CODE])
);

GO


CREATE TABLE [T_QREST_REF_QC](
	[PAR_CODE] varchar(6) NOT NULL,
	[ASSESSMENT_TYPE] varchar(30) NOT NULL,
	[AMT_DIFF_LIM] [float] NULL,
	[PCT_DIFF_LIM] [float] NULL,
	[MIN_CONC] [float] NULL,
	[MAX_CONC] [float] NULL,
 CONSTRAINT [PK_T_QREST_REF_QC] PRIMARY KEY CLUSTERED  ([PAR_CODE] ASC, [ASSESSMENT_TYPE] ASC)
);

CREATE TABLE [T_QREST_REF_QC_AUDIT_LVL](
	[PAR_CODE] varchar(6) NOT NULL,
	[AUDIT_LVL] int NOT NULL,
	[MIN_AMT] [float] NULL,
	[MAX_AMT] [float] NULL,
 CONSTRAINT [PK_T_QREST_REF_QC_AUDIT_LVL] PRIMARY KEY CLUSTERED  ([PAR_CODE] ASC, [AUDIT_LVL] ASC)
);


GO
/******************************************************************************************/
/*******************    ORGANIZATION TABLES ***********************************************/
/******************************************************************************************/
CREATE TABLE [T_QREST_ORGANIZATIONS] (
	[ORG_ID] varchar(30) NOT NULL,
	[ORG_NAME] varchar(100) NOT NULL,
	[AQS_AGENCY_CODE] varchar(4),
	[STATE_CD] varchar(2),
	[EPA_REGION] [int],
	[AQS_NAAS_UID] varchar(100),
	[AQS_NAAS_PWD] varchar(250),
	[AQS_AQS_UID] varchar(100),
	[AQS_AQS_SCREENING_GRP] varchar(100),
	[SELF_REG_IND] [bit] DEFAULT 1,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_ORGANIZATIONS] PRIMARY KEY ([ORG_ID]),
 FOREIGN KEY ([STATE_CD]) references T_QREST_REF_STATE ([STATE_CD]),
 FOREIGN KEY ([EPA_REGION]) references T_QREST_REF_REGION ([EPA_REGION]),
 FOREIGN KEY ([AQS_AGENCY_CODE]) references T_QREST_REF_AQS_AGENCY ([AQS_AGENCY_CODE])
);


GO


CREATE TABLE [dbo].[T_QREST_ORG_EMAIL_RULE](
	[ORG_ID] varchar(30) NOT NULL,
	[EMAIL_STRING] varchar(100) NOT NULL,
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
 CONSTRAINT [PK_T_QREST_ORG_EMAIL_RULE] PRIMARY KEY CLUSTERED  ([ORG_ID] ASC, [EMAIL_STRING] ASC),
 FOREIGN KEY ([ORG_ID]) references T_QREST_ORGANIZATIONS ([ORG_ID]) ON UPDATE CASCADE 	ON DELETE CASCADE
) ON [PRIMARY]

GO	


CREATE TABLE [T_QREST_ORG_USERS] (
	[ORG_USER_IDX] UNIQUEIDENTIFIER NOT NULL,
	[ORG_ID] varchar(30) NOT NULL,
	[USER_IDX] nvarchar(128) NOT NULL,
	[ACCESS_LEVEL] varchar(1) NOT NULL,
	[STATUS_IND] varchar(1) NOT NULL,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_ORG_USERS] PRIMARY KEY ([ORG_USER_IDX]),
    CONSTRAINT [FK_T_QREST_ORG_USERS_T] FOREIGN KEY ([ORG_ID]) REFERENCES [T_QREST_ORGANIZATIONS] ([ORG_ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_T_QREST_ORG_USERS_U] FOREIGN KEY ([USER_IDX]) REFERENCES [T_QREST_USERS] ([USER_IDX]) ON DELETE CASCADE
);
GO



/******************************************************************************************/
/*******************  SITE/MONITOR TABLES ***********************************************/
/******************************************************************************************/
CREATE TABLE [T_QREST_SITES] (
	[SITE_IDX] UNIQUEIDENTIFIER NOT NULL,
	[ORG_ID] varchar(30) NOT NULL,
	[SITE_ID] varchar(50) NOT NULL,
	[SITE_NAME] varchar(100) NOT NULL,
	[AQS_SITE_ID] varchar(4),
	[STATE_CD] varchar(2),
	[COUNTY_CD] varchar(3),
	[LATITUDE] decimal(18,5),
	[LONGITUDE] decimal(18,5),
	[ELEVATION] varchar(10),
	[ADDRESS] varchar(100),
	[CITY] varchar(50),
	[ZIP_CODE] varchar(10),
	[START_DT] datetime2(0),
	[END_DT] datetime2(0),
	
	[POLLING_ONLINE_IND] bit,
	[POLLING_FREQ_TYPE] [char](1),
	[POLLING_FREQ_NUM] [int],
	[POLLING_LAST_RUN_DT] [datetime2](7),
	[POLLING_NEXT_RUN_DT] [datetime2](7),

	[AIRNOW_IND] bit,
	[AIRNOW_USR] varchar(50),
	[AIRNOW_PWD] varchar(50),
	[AIRNOW_ORG] varchar(3),
	[AIRNOW_SITE] varchar(10),
	[AQS_IND] bit,
	[SITE_COMMENTS] varchar(max),
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_SITES] PRIMARY KEY ([SITE_IDX]),
    CONSTRAINT [FK_T_QREST_SITES_O] FOREIGN KEY ([ORG_ID]) REFERENCES [T_QREST_ORGANIZATIONS] ([ORG_ID]),
    CONSTRAINT [FK_T_QREST_SITES_C] FOREIGN KEY ([STATE_CD],[COUNTY_CD]) REFERENCES [T_QREST_REF_COUNTY] ([STATE_CD],[COUNTY_CD])
);


GO



CREATE TABLE [T_QREST_SITE_NOTIFY] (
	[SITE_NOTIFY_IDX] UNIQUEIDENTIFIER NOT NULL,
	[SITE_IDX] UNIQUEIDENTIFIER NOT NULL,
	[NOTIFY_USER_IDX] nvarchar(128),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_SITE_NOTIFY] PRIMARY KEY ([SITE_NOTIFY_IDX]),
    CONSTRAINT [FK_T_QREST_SITE_NOTIFY_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]) ON DELETE CASCADE,
    CONSTRAINT [FK_T_QREST_SITE_NOTIFY_U] FOREIGN KEY ([NOTIFY_USER_IDX]) REFERENCES [T_QREST_USERS] ([USER_IDX])
);


GO


CREATE TABLE [T_QREST_MONITORS] (
	[MONITOR_IDX] UNIQUEIDENTIFIER NOT NULL,
	[SITE_IDX] UNIQUEIDENTIFIER NOT NULL,
	[PAR_METHOD_IDX] UNIQUEIDENTIFIER NOT NULL,
	[POC] int NOT NULL,
	[DURATION_CODE] varchar(1),
	[COLLECT_FREQ_CODE] varchar(2),
	[COLLECT_UNIT_CODE] varchar(3),
	[ALERT_MIN_VALUE] float,
	[ALERT_MAX_VALUE] float,
	[ALERT_AMT_CHANGE] float,
	[ALERT_STUCK_REC_COUNT] int,
	[ALERT_MIN_TYPE] varchar(1),
	[ALERT_MAX_TYPE] varchar(1),
	[ALERT_AMT_CHANGE_TYPE] varchar(1),
	[ALERT_STUCK_TYPE] varchar(1),
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_MONITORS] PRIMARY KEY ([MONITOR_IDX]),
    CONSTRAINT [FK_T_QREST_MONITORS_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]),
    CONSTRAINT [FK_T_QREST_MONITORS_P] FOREIGN KEY ([PAR_METHOD_IDX]) REFERENCES [T_QREST_REF_PAR_METHODS] ([PAR_METHOD_IDX]),
    CONSTRAINT [FK_T_QREST_MONITORS_D] FOREIGN KEY ([DURATION_CODE]) REFERENCES [T_QREST_REF_DURATION] ([DURATION_CODE]),
    CONSTRAINT [FK_T_QREST_MONITORS_C] FOREIGN KEY ([COLLECT_FREQ_CODE]) REFERENCES [T_QREST_REF_COLLECT_FREQ] ([COLLECT_FREQ_CODE])
);


GO


CREATE TABLE [T_QREST_SITE_POLL_CONFIG] (
	[POLL_CONFIG_IDX] UNIQUEIDENTIFIER NOT NULL,
	[SITE_IDX] UNIQUEIDENTIFIER NOT NULL,
	[CONFIG_NAME] varchar(20) NULL,
	[CONFIG_DESC] varchar(200) NULL,
	[RAW_DURATION_CODE] varchar(1) NULL,
	[LOGGER_TYPE] varchar(20),
	[LOGGER_SOURCE] varchar(150),
	[LOGGER_PORT] int,
	[LOGGER_USERNAME] varchar(150),
	[LOGGER_PASSWORD] varchar(250),
	[DELIMITER] char(1),
	[DATE_COL] int,
	[DATE_FORMAT] varchar(10),
	[TIME_COL] int,
	[TIME_FORMAT] varchar(10),
	[LOCAL_TIMEZONE] varchar(4),
	[TIME_POLL_TYPE] varchar(1),
    [ACT_IND] bit NOT NULL,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_SITE_POLL_CONFIG] PRIMARY KEY ([POLL_CONFIG_IDX]),
    CONSTRAINT [FK_T_SITE_POLL_CONFIG_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]) ON DELETE CASCADE,
    CONSTRAINT [FK_T_SITE_POLL_CONFIG_D] FOREIGN KEY ([RAW_DURATION_CODE]) REFERENCES [T_QREST_REF_DURATION] ([DURATION_CODE])
);


GO


CREATE TABLE [T_QREST_SITE_POLL_CONFIG_DTL] (
	[POLL_CONFIG_DTL_IDX] UNIQUEIDENTIFIER NOT NULL,
	[POLL_CONFIG_IDX] UNIQUEIDENTIFIER NOT NULL,
	[MONITOR_IDX] UNIQUEIDENTIFIER NOT NULL,
	[COL] int,
	[SUM_TYPE] varchar(10),
	[ROUNDING] int,
	[ADJUST_FACTOR] float,
    CONSTRAINT [PK_T_SITE_POLL_CONFIG_DTL] PRIMARY KEY ([POLL_CONFIG_DTL_IDX]),
    CONSTRAINT [FK_T_SITE_POLL_CONFIG_DTL_P] FOREIGN KEY ([POLL_CONFIG_IDX]) REFERENCES [T_QREST_SITE_POLL_CONFIG] ([POLL_CONFIG_IDX]) ON DELETE CASCADE,
    CONSTRAINT [FK_T_SITE_POLL_CONFIG_DTL_M] FOREIGN KEY ([MONITOR_IDX]) REFERENCES [T_QREST_MONITORS] ([MONITOR_IDX]) ON DELETE CASCADE
);


GO



CREATE TABLE [T_QREST_QC_ASSESSMENT] (
	[QC_ASSESS_IDX] UNIQUEIDENTIFIER NOT NULL,
	[MONITOR_IDX] UNIQUEIDENTIFIER NOT NULL,
	[ASSESSMENT_DT] datetime2(0) NOT NULL,
	[ASSESSMENT_TYPE] varchar(30) NOT NULL,
	[ASSESSMENT_NUM] int NOT NULL,
	[UNIT_CODE] varchar(3),
	[ASSESSED_BY] varchar(128),
	[ASSESSMENT_TM] varchar(10),
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
	[ASSESSMENT_TM] varchar(10),
    [AQS_IDX] uniqueidentifier,
    CONSTRAINT [PK_T_QREST_QC_ASSESSMENT] PRIMARY KEY ([QC_ASSESS_IDX]),
    CONSTRAINT [FK_T_QREST_QC_ASSESSMENT_M] FOREIGN KEY ([MONITOR_IDX]) REFERENCES [T_QREST_MONITORS] ([MONITOR_IDX]),
    CONSTRAINT [FK_T_QREST_QC_ASSESSMENT_U] FOREIGN KEY ([UNIT_CODE]) REFERENCES [T_QREST_REF_UNITS] ([UNIT_CODE])
);

GO

CREATE TABLE [T_QREST_QC_ASSESSMENT_DTL] (
	[QC_ASSESS_DTL_IDX] UNIQUEIDENTIFIER NOT NULL,
	[QC_ASSESS_IDX] UNIQUEIDENTIFIER NOT NULL,
	[MON_CONCENTRATION] [float],
	[ASSESS_KNOWN_CONCENTRATION] [float],
	[AQS_NULL_CODE] [varchar](2) NULL,
	[COMMENTS] [varchar](100) NULL,
	[CREATE_USER_IDX] nvarchar(128),
	[CREATE_DT] datetime2(0),
	[MODIFY_USER_IDX] nvarchar(128),
	[MODIFY_DT] datetime2(0),
    CONSTRAINT [PK_T_QREST_QC_ASSESSMENT_DTL] PRIMARY KEY ([QC_ASSESS_DTL_IDX]),
    CONSTRAINT [FK_T_QREST_QC_ASSESSMENT_DTL_A] FOREIGN KEY ([QC_ASSESS_IDX]) REFERENCES [T_QREST_QC_ASSESSMENT] ([QC_ASSESS_IDX]) ON DELETE CASCADE
);

GO

/*******************************************************************************************************/
/***************************************  NOTIFICATION  ************************************************/
/*******************************************************************************************************/

CREATE TABLE [dbo].[T_QREST_USER_NOTIFICATION](
    [NOTIFICATION_IDX] [uniqueidentifier] NOT NULL,
	[USER_IDX] nvarchar(128) NOT NULL,
	[NOTIFY_DT] [datetime2](0) NOT NULL,
	[NOTIFY_TYPE] [varchar](20) NOT NULL,
	[NOTIFY_TITLE] [varchar](50),
	[NOTIFY_DESC] [varchar](2000),
	[READ_IND] bit,
	[FROM_USER_IDX] nvarchar(128),
	[CREATE_USERIDX] nvarchar(128),
	[CREATE_DT] [datetime2](0),
	[MODIFY_USERIDX] nvarchar(128),
	[MODIFY_DT] [datetime2](0),
 CONSTRAINT [PK_T_QREST_USER_NOTIFICATION] PRIMARY KEY CLUSTERED ([NOTIFICATION_IDX] ASC)
 ) on [PRIMARY]


/*******************************************************************************************************/
/***************************************  DATA  ************************************************/
/*******************************************************************************************************/


CREATE TABLE [T_QREST_DATA_FIVE_MIN] (
	[DATA_FIVE_IDX] UNIQUEIDENTIFIER NOT NULL,
	[MONITOR_IDX] UNIQUEIDENTIFIER NOT NULL,
	[DATA_DTTM] [datetime2](0),
	[DATA_DTTM_LOCAL] datetime2(0),
	[DATA_VALUE] [varchar](20),
	[UNIT_CODE] varchar(3),
	[VAL_IND] bit,
	[VAL_CD] [varchar](5),
	[MODIFY_DT] [datetime2](0),
	[IMPORT_IDX] [uniqueidentifier] NULL,
    CONSTRAINT [PK_T_QREST_DATA_FIVE_MIN] PRIMARY KEY ([DATA_FIVE_IDX])
);

GO

CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_FIVE_MIN_CE0E42157991124453CC9943039DB00F] ON [dbo].[T_QREST_DATA_FIVE_MIN] ([MONITOR_IDX]) INCLUDE ([DATA_DTTM], [DATA_VALUE], [UNIT_CODE]);
CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_FIVE_MIN_1A70FD320F5A6EA2B1924759B9C666FC] ON [dbo].[T_QREST_DATA_FIVE_MIN] ([MONITOR_IDX], [DATA_DTTM]) INCLUDE ([DATA_VALUE], [UNIT_CODE], [VAL_CD], [VAL_IND]);


CREATE TABLE [dbo].[T_QREST_DATA_HOURLY](
	[DATA_HOURLY_IDX] [uniqueidentifier] NOT NULL,
	[MONITOR_IDX] [uniqueidentifier] NOT NULL,
	[DATA_DTTM_UTC] [datetime2](0) NULL,
	[DATA_DTTM_LOCAL] [datetime2](0) NULL,
	[DATA_VALUE] [varchar](20) NULL,
    [DATA_VALUE_NUM] decimal(18,9),
	[UNIT_CODE] [varchar](3) NULL,
	[AQS_NULL_CODE] [varchar](2) NULL,
	[AQS_QUAL_CODES] varchar(30),
	[VAL_IND] [bit] NULL,
	[VAL_CD] [varchar](5) NULL,
	[VAL0_NOTIFY_IND] [bit] NOT NULL DEFAULT 0,
	[LVL1_VAL_IND] [bit] NULL,
	[LVL1_VAL_USERIDX] nvarchar(128) NULL,
	[LVL1_VAL_DT] [datetime2](0) NULL,
	[LVL2_VAL_IND] [bit] NULL,
	[LVL2_VAL_USERIDX] nvarchar(128) NULL,
	[LVL2_VAL_DT] [datetime2](0) NULL,
	[MODIFY_DT] [datetime2](0) NULL,
	[NOTES] varchar(100) NULL,
	[IMPORT_IDX] [uniqueidentifier] NULL,
 CONSTRAINT [PK_T_QREST_DATA_HOURLY] PRIMARY KEY CLUSTERED ([DATA_HOURLY_IDX] ASC)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_HOURLY_127D1F43E20002699B3BBD446C9FFD99] ON [dbo].[T_QREST_DATA_HOURLY] ([MONITOR_IDX], [DATA_DTTM_UTC]) INCLUDE ([DATA_DTTM_LOCAL], [DATA_VALUE], [LVL1_VAL_DT], [LVL2_VAL_DT]);
CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_HOURLY_2] ON [dbo].[T_QREST_DATA_HOURLY] ([MONITOR_IDX],[DATA_DTTM_LOCAL]);
CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_HOURLY_3] ON [dbo].[T_QREST_DATA_HOURLY] ([MONITOR_IDX], [VAL_IND]) INCLUDE ([DATA_DTTM_UTC], [DATA_VALUE]); 
CREATE NONCLUSTERED INDEX [nci_wi_T_QREST_DATA_HOURLY_4] ON [dbo].[T_QREST_DATA_HOURLY] ([MONITOR_IDX], [VAL_CD], [VAL_IND], [DATA_VALUE]);



CREATE TABLE [dbo].[T_QREST_DATA_HOURLY_LOG](
	[DATA_HOURLY_LOG_IDX] [uniqueidentifier] NOT NULL,
	[DATA_HOURLY_IDX] [uniqueidentifier] NOT NULL,
	[NOTES] varchar(100) NULL,
	[MODIFY_USERIDX] nvarchar(128) NULL,
	[MODIFY_DT] [datetime2](0) NULL,
 CONSTRAINT [PK_T_QREST_DATA_HOURLY_LOG] PRIMARY KEY CLUSTERED ([DATA_HOURLY_LOG_IDX] ASC),
 CONSTRAINT [FK_T_QREST_DATA_HOURLY_LOG_O] FOREIGN KEY ([DATA_HOURLY_IDX]) REFERENCES [T_QREST_DATA_HOURLY] ([DATA_HOURLY_IDX]) ON DELETE CASCADE
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[T_QREST_DATA_IMPORTS](
	[IMPORT_IDX] [uniqueidentifier] NOT NULL,
	[ORG_ID] varchar(30) NOT NULL,
	[SITE_IDX] [uniqueidentifier] NOT NULL,
	[COMMENT] [varchar](1000),
	[SUBMISSION_STATUS] [varchar](20),
	[SUBMISSION_FILE] [varchar](max),
	[IMPORT_USERIDX] nvarchar(128),
	[IMPORT_DT] [datetime2](0),
	[IMPORT_TYPE] [varchar](2),
	[MONITOR_IDX] [uniqueidentifier],
	[POLL_CONFIG_IDX] [uniqueidentifier],
	[RECALC_IND] bit,
 CONSTRAINT [PK_T_QREST_DATA_IMPORTS] PRIMARY KEY CLUSTERED ([IMPORT_IDX] ASC),
 CONSTRAINT [FK_T_QREST_DATA_IMPORTS_O] FOREIGN KEY ([ORG_ID]) REFERENCES [T_QREST_ORGANIZATIONS] ([ORG_ID]) ON DELETE CASCADE, 
 CONSTRAINT [FK_T_QREST_DATA_IMPORTS_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]) ON DELETE CASCADE
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_ASSESS_DOCS](
	[ASSESS_DOC_IDX] [uniqueidentifier] NOT NULL,
	[SITE_IDX] [uniqueidentifier] NOT NULL,
	[MONITOR_IDX] [uniqueidentifier] NULL,
	[START_DT] datetime2(0),
	[END_DT] datetime2(0),
	[DOC_CONTENT] [varbinary](max) NULL,
	[DOC_NAME] [varchar](100) NULL,
	[DOC_TYPE] [varchar](50) NULL,
	[DOC_FILE_TYPE] [varchar](75) NULL,
	[DOC_SIZE] [int] NULL,
	[DOC_COMMENT] [varchar](max) NULL,
	[DOC_AUTHOR] [varchar](100) NULL,
	[CREATE_USERIDX] nvarchar(128),
	[CREATE_DT] [datetime2](0),
	[MODIFY_USERIDX] nvarchar(128),
	[MODIFY_DT] [datetime2](0),
 CONSTRAINT [PK_T_QREST_ASSESS_DOCS] PRIMARY KEY CLUSTERED ([ASSESS_DOC_IDX] ASC),
 CONSTRAINT [FK_T_QREST_ASSESS_DOCS_M] FOREIGN KEY ([MONITOR_IDX]) REFERENCES [T_QREST_MONITORS] ([MONITOR_IDX]) ON DELETE CASCADE, 
 CONSTRAINT [FK_T_QREST_ASSESS_DOCS_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]) ON DELETE CASCADE
) ON [PRIMARY]



GO



CREATE TABLE [dbo].[T_QREST_AQS](
	[AQS_IDX] [uniqueidentifier] NOT NULL,
	[ORG_ID] varchar(30) NOT NULL,
	[SITE_IDX] [uniqueidentifier] NOT NULL,
	[AQS_SUBMISSION_NAME] [varchar](100) NULL,
	[START_DT] [datetime2](0) NOT NULL,
	[END_DT] [datetime2](0) NOT NULL,
	[AQS_CONTENT] [varbinary](max),
	[DOC_SIZE] [int],
	[COMMENT] [varchar](1000),
	[SUBMISSION_STATUS] [varchar](20),
	[AQS_CONTENT_XML] varchar(max),
	[CDX_TOKEN] [varchar](250),
    [DOWNLOAD_FILE] [varbinary](max),
	[CREATE_USERIDX] nvarchar(128),
	[CREATE_DT] [datetime2](0),
	[MODIFY_USERIDX] nvarchar(128),
	[MODIFY_DT] [datetime2](0),
	[AQS_TRANS_TYPE] varchar(2),
 CONSTRAINT [PK_T_QREST_AQS] PRIMARY KEY CLUSTERED ([AQS_IDX] ASC),
 CONSTRAINT [FK_T_QREST_AQS_O] FOREIGN KEY ([ORG_ID]) REFERENCES [T_QREST_ORGANIZATIONS] ([ORG_ID]) ON DELETE CASCADE, 
 CONSTRAINT [FK_T_QREST_AQS_S] FOREIGN KEY ([SITE_IDX]) REFERENCES [T_QREST_SITES] ([SITE_IDX]) ON DELETE CASCADE
) ON [PRIMARY]
GO





CREATE TABLE [dbo].[T_QREST_DATA_IMPORT_TEMP](
	[DATA_IMPORT_TEMP_IDX] [uniqueidentifier] NOT NULL,
    [IMPORT_USER_IDX] [nvarchar](128) NOT NULL,
	[DATA_ORIG_TABLE_IDX] [uniqueidentifier],
	[MONITOR_IDX] [uniqueidentifier] NOT NULL,
	[DATA_DTTM_UTC] [datetime2](0),
	[DATA_DTTM_LOCAL] [datetime2](0),
	[DATA_VALUE] varchar(20),
    [DATA_VALUE_NUM] decimal(18,9),
	[UNIT_CODE] varchar(3),
	[VAL_CD] varchar(5),
	[AQS_NULL_CODE] varchar(2),
	[AQS_QUAL_CODES] varchar(30),
	[IMPORT_IDX] [uniqueidentifier],
	[IMPORT_DT] [datetime2](0),
	[IMPORT_VAL_IND] [bit],
	[IMPORT_DUP_IND] [bit],
	[IMPORT_MSG] varchar(100),
 CONSTRAINT [PK_QREST_DATA_IMPORT_TEMP] PRIMARY KEY CLUSTERED ([DATA_IMPORT_TEMP_IDX] ASC),
 CONSTRAINT [FK_QREST_DATA_IMPORT_TEMP] FOREIGN KEY ([IMPORT_IDX]) REFERENCES T_QREST_DATA_IMPORTS ([IMPORT_IDX]) ON DELETE CASCADE, 
) ON [PRIMARY]

GO


/*******************************************************************************************************/
/***************************************  TRAIN_COURSE  ************************************************/
/*******************************************************************************************************/
CREATE TABLE [dbo].[T_QREST_TRAIN_COURSE](
	[COURSE_IDX] [uniqueidentifier] NOT NULL,
	[COURSE_NAME] varchar(50),
	[COURSE_DESC] varchar(2000),
	[COURSE_SEQ] int,
	[ACT_IND] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_QREST_TRAIN_COURSE] PRIMARY KEY CLUSTERED ([COURSE_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_LESSON](
	[LESSON_IDX] [uniqueidentifier] NOT NULL,
	[LESSON_TITLE] varchar(100),
	[LESSON_DESC] varchar(max),
 CONSTRAINT [PK_QREST_TRAIN_LESSON] PRIMARY KEY CLUSTERED ([LESSON_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_COURSE_LESSON](
	[COURSE_IDX] [uniqueidentifier] NOT NULL,
	[LESSON_IDX] [uniqueidentifier] NOT NULL,
	[LESSON_SEQ] int,
	[LESSON_DESC] varchar(max),
 CONSTRAINT [PK_QREST_TRAIN_C_LESSON] PRIMARY KEY CLUSTERED ([COURSE_IDX],[LESSON_IDX] ASC),
 CONSTRAINT [FK_QREST_TRAIN_C_LESSON_C] FOREIGN KEY ([COURSE_IDX]) REFERENCES T_QREST_TRAIN_COURSE ([COURSE_IDX]) ON DELETE CASCADE, 
 CONSTRAINT [FK_QREST_TRAIN_C_LESSON_L] FOREIGN KEY ([LESSON_IDX]) REFERENCES T_QREST_TRAIN_LESSON ([LESSON_IDX]) ON DELETE CASCADE 
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_LESSON_STEP](
	[LESSON_STEP_IDX] [uniqueidentifier] NOT NULL,
	[LESSON_IDX] [uniqueidentifier] NOT NULL,
	[LESSON_STEP_SEQ] int,
	[LESSON_STEP_DESC] varchar(max),
	[REQUIRED_URL] varchar(250),
	[REQUIRED_YT_VID] varchar(250),
	[REQ_CONFIRM] bit,
 CONSTRAINT [PK_QREST_TRAIN_LESSON_STEP] PRIMARY KEY CLUSTERED ([LESSON_STEP_IDX] ASC)
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_LESSON_STEP_USER](
	[LESSON_STEP_IDX] [uniqueidentifier] NOT NULL,
	[USER_IDX] nvarchar(128) NOT NULL,
	[CREATE_DT] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_QREST_TRAIN_LESSON_STEP_USER] PRIMARY KEY CLUSTERED ([LESSON_STEP_IDX], [USER_IDX] ASC),
 CONSTRAINT [FK_QREST_TRAIN_LESSON_STEP_USER_LS] FOREIGN KEY ([LESSON_STEP_IDX]) REFERENCES [T_QREST_TRAIN_LESSON_STEP] ([LESSON_STEP_IDX]) ON DELETE CASCADE
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_LESSON_USER](
	[LESSON_IDX] [uniqueidentifier] NOT NULL,
	[USER_IDX] nvarchar(128) NOT NULL,
	[CREATE_DT] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_QREST_TRAIN_LESSON_USER] PRIMARY KEY CLUSTERED ([LESSON_IDX], [USER_IDX] ASC),
 CONSTRAINT [FK_QREST_TRAIN_LESSON_USER_L] FOREIGN KEY ([LESSON_IDX]) REFERENCES [T_QREST_TRAIN_LESSON] ([LESSON_IDX]) ON DELETE CASCADE
) ON [PRIMARY]


GO


CREATE TABLE [dbo].[T_QREST_TRAIN_COURSE_USER](
	[COURSE_IDX] [uniqueidentifier] NOT NULL,
	[USER_IDX] nvarchar(128) NOT NULL,
	[CREATE_DT] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_QREST_TRAIN_COURSE_USER] PRIMARY KEY CLUSTERED ([COURSE_IDX], [USER_IDX] ASC),
 CONSTRAINT [FK_QREST_TRAIN_COURSE_USER_C] FOREIGN KEY ([COURSE_IDX]) REFERENCES [T_QREST_TRAIN_COURSE] ([COURSE_IDX]) ON DELETE CASCADE
) ON [PRIMARY]

