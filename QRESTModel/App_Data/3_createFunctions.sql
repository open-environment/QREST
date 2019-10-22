USE [QREST]
GO
CREATE FUNCTION [dbo].[InitCap] ( @InputString varchar(4000) ) 
RETURNS VARCHAR(4000)
AS
BEGIN

DECLARE @Index          INT
DECLARE @Char           CHAR(1)
DECLARE @PrevChar       CHAR(1)
DECLARE @OutputString   VARCHAR(355)

SET @OutputString = LOWER(@InputString)
SET @Index = 1

WHILE @Index <= LEN(@InputString)
BEGIN
    SET @Char     = SUBSTRING(@InputString, @Index, 1)
    SET @PrevChar = CASE WHEN @Index = 1 THEN ' '
                         ELSE SUBSTRING(@InputString, @Index - 1, 1)
                    END

    IF @PrevChar IN (' ', ';', ':', '!', '?', ',', '.', '_', '-', '/', '&', '''', '(', CHAR(13))
    BEGIN
        IF @PrevChar != '''' OR UPPER(@Char) != 'S'
            SET @OutputString = STUFF(@OutputString, @Index, 1, UPPER(@Char))
    END

    SET @Index = @Index + 1
END

RETURN isnull(@OutputString,'')

END


GO



--TRIGGERS ***************************************************************8
CREATE OR ALTER TRIGGER [dbo].[TRG_UPDATE_HOURLY] ON [dbo].[T_QREST_DATA_FIVE_MIN]
AFTER INSERT, UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF;

	DECLARE @SumType varchar(3);
	DECLARE @mon uniqueidentifier;
	DECLARE @dttm datetime;
	DECLARE @dttm_l datetime;
	DECLARE @sumVal varchar(20);
	DECLARE @unit varchar(3);
	DECLARE @iCount int;
	DECLARE @tz varchar(30);
	DECLARE @precision int;

	DECLARE ins_cursor CURSOR FOR 
	SELECT distinct MONITOR_IDX, dateadd(hour, datediff(hour, 0, DATA_DTTM), 0) FROM INSERTED

	OPEN ins_cursor  
	FETCH NEXT FROM ins_cursor INTO @mon, @dttm

	WHILE @@FETCH_STATUS=0
	BEGIN


		--get summary type to calculate
		select @SumType=D.SUM_TYPE, @tz=T.TZ_NAME, @precision=D.ROUNDING
		from T_QREST_SITE_POLL_CONFIG C, T_QREST_SITE_POLL_CONFIG_DTL D , T_QREST_REF_TIMEZONE T
		where C.POLL_CONFIG_IDX = D.POLL_CONFIG_IDX
		and C.LOCAL_TIMEZONE = T.TZ_CODE
		and C.ACT_IND=1
		and D.MONITOR_IDX = @mon;

		--set precision
		if (@precision is null)
			set @precision=0;

		--local time
		set @dttm_l = @dttm AT TIME ZONE 'UTC' AT TIME ZONE @tz;
	
		if (@SumType='AVG')
		BEGIN
			select @sumVal = cast(round(avg(cast(F.DATA_VALUE as float)),@precision) as varchar), 
				   @unit=max(UNIT_CODE), 
				   @iCount=count(*)
			from T_QREST_DATA_FIVE_MIN F
			where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
			and F.MONITOR_IDX=@mon
			and ISNUMERIC(DATA_VALUE) = 1;
		END
		else if (@SumType='MAX')
		BEGIN
			select @sumVal = cast(round(max(cast(F.DATA_VALUE as float)),@precision) as varchar),
				   @unit=max(UNIT_CODE), 
				   @iCount=count(*)
			from T_QREST_DATA_FIVE_MIN F
			where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
			and F.MONITOR_IDX=@mon
			and ISNUMERIC(DATA_VALUE) = 1;
		END
		else if (@SumType='MIN')
		BEGIN
			select @sumVal = cast(round(min(cast(F.DATA_VALUE as float)),@precision) as varchar),
				   @unit=max(UNIT_CODE), 
				   @iCount=count(*)
			from T_QREST_DATA_FIVE_MIN F
			where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
			and F.MONITOR_IDX=@mon
			and ISNUMERIC(DATA_VALUE) = 1;
		END

		if (@iCount<9)
			set @sumVal = 'FEW';
		
		--UPSERT RECORD
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
		
		MERGE T_QREST_DATA_HOURLY AS myTarget
		USING (SELECT @mon MONITOR_IDX, @dttm DATA_DTTM_UTC, @sumVal DATA_VALUE) AS mySource
		ON mySource.MONITOR_IDX = myTarget.MONITOR_IDX and mySource.DATA_DTTM_UTC = myTarget.DATA_DTTM_UTC
		WHEN MATCHED THEN 
			UPDATE SET DATA_VALUE = mySource.DATA_VALUE
		WHEN NOT MATCHED THEN 
			INSERT (DATA_HOURLY_IDX, MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE) VALUES (newid(), @mon, @dttm, @dttm_l, @sumVal, @unit);

		FETCH NEXT FROM ins_cursor INTO @mon, @dttm
	END

	CLOSE ins_cursor	
	DEALLOCATE ins_cursor

END


GO