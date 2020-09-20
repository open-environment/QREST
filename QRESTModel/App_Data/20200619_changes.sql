ALTER TABLE T_QREST_SITE_POLL_CONFIG ADD [CONFIG_DESC] varchar(200) NULL;

ALTER TABLE [T_QREST_DATA_IMPORTS] ADD [SUBMISSION_FILE] [varchar](max);
ALTER TABLE [T_QREST_DATA_IMPORTS] ADD [IMPORT_TYPE] [varchar](2);
ALTER TABLE [T_QREST_DATA_IMPORTS] ADD [MONITOR_IDX] [uniqueidentifier];
ALTER TABLE [T_QREST_DATA_IMPORTS] ADD [POLL_CONFIG_IDX] [uniqueidentifier];
ALTER TABLE [T_QREST_DATA_IMPORTS] ADD [RECALC_IND] [bit];

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


--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_IMPORT_DATA_FROM_TEMP]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE   PROCEDURE [dbo].[SP_IMPORT_DATA_FROM_TEMP] 
	@import_idx uniqueidentifier
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--inserts or updates RAW data from RAW TEMP IMPORT table
	--declare @import_idx uniqueidentifier
	declare @import_typ varchar(2);
	declare @recalc_ind bit;
	declare @mod_dt datetime2(0);

	--set @import_idx = '563C2DE0-5323-4601-ABC4-6EEA894AF6BC';

	--CHANGE LOG
	---------------------------
	--20200625 created
	--20200702 fix updating import_idx
	--20200703 fix table naming


	--get summary type to calculate
	select @import_typ=T.IMPORT_TYPE, @recalc_ind=T.RECALC_IND
	from T_QREST_DATA_IMPORTS T 
	where T.IMPORT_IDX = @import_idx;

	--initialize importing status
	UPDATE T_QREST_DATA_IMPORTS set SUBMISSION_STATUS='IMPORTING' where IMPORT_IDX = @import_idx;

	if (@import_typ = 'H' or @import_typ = 'H1')
	BEGIN
		-- HANDLING HOURLY UPDATED RECORDS
		UPDATE H SET
			H.DATA_DTTM_UTC = T.DATA_DTTM_UTC,
			H.[DATA_DTTM_LOCAL] = T.DATA_DTTM_LOCAL,
			H.DATA_VALUE = T.DATA_VALUE,
			H.DATA_VALUE_NUM = T.DATA_VALUE_NUM,
			H.UNIT_CODE = T.UNIT_CODE,
			H.VAL_IND = @recalc_ind ^ 1,
			H.VAL_CD = T.VAL_CD,
			H.AQS_NULL_CODE = T.AQS_NULL_CODE,
			H.AQS_QUAL_CODES = T.AQS_QUAL_CODES,
			H.MODIFY_DT = GetDATE(),
			H.IMPORT_IDX = @import_idx
		FROM
			T_QREST_DATA_HOURLY AS H
			INNER JOIN T_QREST_DATA_IMPORT_TEMP AS T ON H.DATA_HOURLY_IDX = T.DATA_ORIG_TABLE_IDX
		WHERE T.IMPORT_IDX = @import_idx 
		and T.DATA_ORIG_TABLE_IDX is not null;

		
		-- HANDLING HOURLY INSERT RECORDS
		INSERT INTO T_QREST_DATA_HOURLY ([DATA_HOURLY_IDX], MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, VAL_IND, VAL_CD, MODIFY_DT, AQS_NULL_CODE, IMPORT_IDX, DATA_VALUE_NUM, AQS_QUAL_CODES)
		SELECT newid(), MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, @recalc_ind ^ 1, VAL_CD, GetDate(), AQS_NULL_CODE, @import_idx, DATA_VALUE_NUM, AQS_QUAL_CODES
		FROM T_QREST_DATA_IMPORT_TEMP
		WHERE IMPORT_IDX = @import_idx 
		and DATA_ORIG_TABLE_IDX is null;
	END
	else if (@import_typ = 'F')
	BEGIN
		--having year of 1888 is work around for not recalculating hourly from n-min
		set @mod_Dt = case when @recalc_ind=1 then GetDate() else '1/1/1888' end;
	
		-- HANDLING FIVE MINUTE UPDATED RECORDS
		UPDATE H SET
			H.DATA_DTTM = T.DATA_DTTM_UTC,
			H.DATA_DTTM_LOCAL = T.DATA_DTTM_LOCAL,
			H.DATA_VALUE = T.DATA_VALUE,
			H.UNIT_CODE = T.UNIT_CODE,
			H.VAL_IND = @recalc_ind ^ 1,
			H.VAL_CD = T.VAL_CD,
			H.MODIFY_DT = @mod_Dt,
			H.IMPORT_IDX =  @import_idx
		FROM
			T_QREST_DATA_FIVE_MIN AS H
			INNER JOIN T_QREST_DATA_IMPORT_TEMP AS T ON H.DATA_FIVE_IDX = T.DATA_ORIG_TABLE_IDX
		WHERE T.IMPORT_IDX = @import_idx
		and T.DATA_ORIG_TABLE_IDX is not null;		

		-- HANDLING FIVE MINUTE INSERT RECORDS
		INSERT INTO T_QREST_DATA_FIVE_MIN (DATA_FIVE_IDX, MONITOR_IDX, DATA_DTTM, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, VAL_IND, VAL_CD, MODIFY_DT, IMPORT_IDX)
		SELECT newid(), MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, @recalc_ind ^ 1, VAL_CD, @mod_dt, @import_idx
		FROM T_QREST_DATA_IMPORT_TEMP
		WHERE IMPORT_IDX = @import_idx 
		and DATA_ORIG_TABLE_IDX is null;
	END		


	DELETE FROM T_QREST_DATA_IMPORT_TEMP where IMPORT_IDX = @import_idx;

	UPDATE T_QREST_DATA_IMPORTS set SUBMISSION_STATUS='IMPORTED' where IMPORT_IDX = @import_idx;

END

