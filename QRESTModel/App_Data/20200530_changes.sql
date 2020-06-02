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

insert into T_QREST_REF_QC values ('44201','1-Point QC',0.0015,7, 0.005, 0.08);
insert into T_QREST_REF_QC values ('42401','1-Point QC',0.0015,10, 0.005, 0.08);
insert into T_QREST_REF_QC values ('42406','1-Point QC',0.0015,10, 0.005, 0.08);

insert into T_QREST_REF_QC values ('44201','Annual PE',0.0015,15, null, null);
insert into T_QREST_REF_QC values ('42401','Annual PE',0.0015,15, null, null);
insert into T_QREST_REF_QC values ('42406','Annual PE',0.0015,15, null, null);


alter table [dbo].[T_QREST_AQS] ADD [AQS_TRANS_TYPE] varchar(2);
ALTER TABLE [dbo].[T_QREST_QC_ASSESSMENT] ADD [AQS_IDX] uniqueidentifier;