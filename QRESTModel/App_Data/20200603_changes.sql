alter table [dbo].[T_QREST_DATA_HOURLY] ADD [DATA_VALUE_NUM] decimal(18,9);
alter table [dbo].[T_QREST_DATA_HOURLY] ADD [AQS_QUAL_CODES] varchar(30);




GO


ALTER   TRIGGER [dbo].[TRG_UPDATE_HOURLY] ON [dbo].[T_QREST_DATA_FIVE_MIN]
AFTER INSERT, UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF;
	/**** CHANGE LOG ***/
	--11/20/2019 DOUG TIMMS fix ADEV precision
	--1/20/2020 DOUG TIMMS prevent update if HOURLY RECORD IS LVL1 or LVL2 VAL
	--                     prevent upate if modify date = 8/8/1888 (used to flag no hourly calc)
	--2/23/2020 DOUG TIMMS add calculation of TOT
	--6/4/2020 DOUG TIMMS add numeric data value storage

	DECLARE @SumType varchar(4);
	DECLARE @SumTemp float;
	DECLARE @mon uniqueidentifier;
	DECLARE @dttm datetime; --UTC
	DECLARE @sumVal varchar(20);
	DECLARE @unit varchar(3);
	DECLARE @iCount int;
	DECLARE @tz varchar(2);
	DECLARE @precision int;
	DECLARE @calcIndYr int;

	DECLARE ins_cursor CURSOR FOR 
	SELECT distinct MONITOR_IDX, dateadd(hour, datediff(hour, 0, DATA_DTTM), 0), year(MODIFY_DT) FROM INSERTED

	OPEN ins_cursor  
	FETCH NEXT FROM ins_cursor INTO @mon, @dttm, @calcIndYr

	WHILE @@FETCH_STATUS=0
	BEGIN


		--get summary type to calculate
		select @SumType=D.SUM_TYPE, @tz=C.LOCAL_TIMEZONE, @precision=D.ROUNDING
		from T_QREST_SITE_POLL_CONFIG C, T_QREST_SITE_POLL_CONFIG_DTL D 
		where C.POLL_CONFIG_IDX = D.POLL_CONFIG_IDX
		and C.ACT_IND=1
		and D.MONITOR_IDX = @mon;

		if (@SumType IS NOT NULL and @tz IS NOT NULL)
		BEGIN

			--set precision
			if (@precision is null)
				set @precision=0;

			if (@calcIndYr is null)
				set @calcIndYr=2020;

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
			else if (@SumType='TOT')
			BEGIN
				select @sumVal = cast(round(sum(cast(F.DATA_VALUE as float)),@precision) as varchar),
					   @unit=max(UNIT_CODE), 
					   @iCount=count(*)
				from T_QREST_DATA_FIVE_MIN F
				where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
				and F.MONITOR_IDX=@mon
				and ISNUMERIC(DATA_VALUE) = 1;
			END
			else if (@SumType='AAVG')
			BEGIN
				select @sumTemp = case when x>=0 and y>=0 then 0 + atan(x/y)
									  when x>=0 and y<0 then Pi() - atan(x/-y)
									  when x<0 and y<0 then Pi() + atan(-x/-y)
									  when x<0 and y>=0 then 2*PI() - atan(-x/y) end,
					   @unit=U, 
					   @iCount=CNT							   
				from
				(select avg(sin(F.DATA_VALUE * PI()/180.)) X, avg(cos(F.DATA_VALUE * PI()/180.)) Y , max(UNIT_CODE) U, count(*) CNT
					from T_QREST_DATA_FIVE_MIN F
					where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
					and F.MONITOR_IDX=@mon
					and ISNUMERIC(DATA_VALUE) = 1) Z;

				set @sumVal = cast(round(@sumTemp * 180./PI(),0) as varchar);
			END
			else if (@SumType='ADEV')
			BEGIN
				select @sumVal = cast(round((asin(epsilon)*(1+0.1547*(power(epsilon,3))))*180./PI(), @precision) as varchar),
					   @unit=U, 
					   @iCount=CNT							   
				from
				(select sqrt(1-(square(avg(sin(F.DATA_VALUE * PI()/180.)))+square(avg(cos(F.DATA_VALUE * PI()/180.))))) epsilon , max(UNIT_CODE) U, count(*) CNT
					from T_QREST_DATA_FIVE_MIN F
					where dateadd(hour, datediff(hour, 0, F.DATA_DTTM), 0)=@dttm
					and F.MONITOR_IDX=@mon
					and ISNUMERIC(DATA_VALUE) = 1) Z;
			END

			--set to few if not enough records
			if (@iCount<9)
				set @sumVal = 'FEW';
		
			--UPSERT RECORD
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
		
			MERGE T_QREST_DATA_HOURLY AS myTarget
			USING (SELECT @mon MONITOR_IDX, @dttm DATA_DTTM_UTC, @sumVal DATA_VALUE) AS mySource
			ON mySource.MONITOR_IDX = myTarget.MONITOR_IDX and mySource.DATA_DTTM_UTC = myTarget.DATA_DTTM_UTC 
			WHEN MATCHED and ISNULL(myTarget.LVL1_VAL_IND,0) = 0 and ISNULL(myTarget.LVL2_VAL_IND,0) = 0 and @calcIndYr<>1888 
			THEN 
				UPDATE SET DATA_VALUE = mySource.DATA_VALUE, VAL_IND=0, DATA_VALUE_NUM = TRY_CAST(mySource.DATA_VALUE AS DECIMAL(18, 9)), UNIT_CODE= @unit
			WHEN NOT MATCHED and @calcIndYr<>1888 
			THEN 
				INSERT (DATA_HOURLY_IDX, MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, VAL_IND
				, DATA_VALUE_NUM
				) VALUES (newid(), @mon, @dttm, DATEADD(HOUR,cast(@tz as int),@dttm), @sumVal, @unit, 0
				, TRY_CAST(mySource.DATA_VALUE AS DECIMAL(18, 9)) 
				);

		END

		FETCH NEXT FROM ins_cursor INTO @mon, @dttm, @calcIndYr
	END

	CLOSE ins_cursor	
	DEALLOCATE ins_cursor

END







GO




ALTER   PROCEDURE [dbo].[SP_VALIDATE_HOURLY] 
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. record LOST for hours with no data
	--2. set LIM for values exceeding upper/lower limit
	--3. set FEW for those with less than 9 records
	--4. set JUMP
	--5. set STUCK

	--CHANGE LOG
	------------------------------
	--11/19/2019 fix sql for JUMP
	--12/16/2019 blank out LOST if subsequent data comes in
	--6/4/2020 tweak LOST to only happen if data is missing for more than 2 hours

	SET NOCOUNT ON;  
  
	DECLARE @mon_id UNIQUEIDENTIFIER;
	DECLARE @minDate datetime2(0);
	DECLARE @tz varchar(30);
	DECLARE @min_type varchar(1);
	DECLARE @min_value float;
	DECLARE @max_type varchar(1);
	DECLARE @max_value float;
	DECLARE @amt_type varchar(1);
	DECLARE @amt_value float;
	DECLARE @stuck_type varchar(1);
	DECLARE @stuck_value int;

	DECLARE mon_cursor CURSOR FOR   
	select --S.SITE_IDX, S.SITE_ID, 
	M.MONITOR_IDX
	--, M.PAR_METHOD_IDX, M.POC, M.DURATION_CODE, M.COLLECT_UNIT_CODE, M.ALERT_MIN_TYPE, M.ALERT_MAX_VALUE, M.ALERT_AMT_CHANGE, 
	--M.ALERT_STUCK_REC_COUNT, M.ALERT_MIN_TYPE, M.ALERT_MAX_TYPE, M.ALERT_AMT_CHANGE_TYPE, M.ALERT_STUCK_TYPE, P.RAW_DURATION_CODE, P.LOCAL_TIMEZONE, PD.SUM_TYPE, PD.ROUNDING
	, (select min(DATA_DTTM_UTC) from T_QREST_DATA_HOURLY where MONITOR_IDX=M.MONITOR_IDX)
	, P.LOCAL_TIMEZONE
	, M.ALERT_MIN_TYPE, M.ALERT_MIN_VALUE
	, M.ALERT_MAX_TYPE, M.ALERT_MAX_VALUE
	, M.ALERT_AMT_CHANGE_TYPE, M.ALERT_AMT_CHANGE
	, M.ALERT_STUCK_TYPE, M.ALERT_STUCK_REC_COUNT
	from T_QREST_SITES S, T_QREST_MONITORS M, T_QREST_SITE_POLL_CONFIG P, T_QREST_SITE_POLL_CONFIG_DTL PD
	where S.SITE_IDX = M.SITE_IDX
	and S.SITE_IDX = P.SITE_IDX 
	and P.POLL_CONFIG_IDX = PD.POLL_CONFIG_IDX
	and PD.MONITOR_IDX=M.MONITOR_IDX
	and P.ACT_IND = 1
	and S.POLLING_ONLINE_IND=1
	order by S.SITE_IDX, M.MONITOR_IDX;

	OPEN mon_cursor 
	FETCH NEXT FROM mon_cursor INTO @mon_id, @minDate, @tz, @min_type, @min_value, @max_type, @max_value, @amt_type, @amt_value, @stuck_type, @stuck_value

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		if (@minDate IS NOT NULL)
		BEGIN
			--insert LOST for missing data
			INSERT INTO T_QREST_DATA_HOURLY (DATA_HOURLY_IDX, [MONITOR_IDX], [DATA_DTTM_UTC], [DATA_DTTM_LOCAL],VAL_IND,[VAL_CD])
			select NEWID(), @mon_id, S.HR, DATEADD(HOUR,cast(@tz as int),S.HR), 1, 'LOST'
			from T_SYS_HR S 
			left join T_QREST_DATA_HOURLY H on S.HR=H.DATA_DTTM_UTC and H.MONITOR_IDX=@mon_id
			where S.HR> @minDate
			and S.HR<GetDate()-0.1
			and monitor_IDX is null;

			--insert MIN for hourly values below minimum
			if (@min_type='H' AND @min_value is not null)
			BEGIN
				UPDATE T_QREST_DATA_HOURLY set VAL_IND=1, VAL_CD='MIN' 
				--select @mon_id, [DATA_DTTM_UTC], [DATA_DTTM_LOCAL] , 1, 'MIN', H.DATA_VALUE
				--from T_QREST_DATA_HOURLY H
				where MONITOR_IDX=@mon_id
				and VAL_IND=0
				and ISNUMERIC(DATA_VALUE)=1 
				and DATA_VALUE<@min_value;			 
			END

			--insert MAX for hourly values above maximum
			if (@max_type='H' AND @max_value is not null)
			BEGIN
				UPDATE T_QREST_DATA_HOURLY set VAL_IND=1, VAL_CD='MAX' 
				where MONITOR_IDX=@mon_id
				and VAL_IND=0
				and ISNUMERIC(DATA_VALUE)=1 
				and DATA_VALUE>@max_value;			 
			END

			--insert JUMP for records exceeding AMOUNT CHANGE
			if (@amt_type='H' AND @amt_value is not null AND @amt_value > 0)
			BEGIN
				UPDATE H 
				set VAL_IND=1, VAL_CD='JUMP'
				FROM T_QREST_DATA_HOURLY H
				JOIN T_QREST_DATA_HOURLY H1 on H.MONITOR_IDX=H1.MONITOR_IDX and H1.DATA_DTTM_UTC=DATEADD(HOUR,-1,H.DATA_DTTM_UTC)
				where H.MONITOR_IDX=@mon_id
				and H.VAL_IND=0
				and ISNUMERIC(H.DATA_VALUE)=1
				and ISNUMERIC(H1.DATA_VALUE)=1
				and (cast(H.DATA_VALUE as float) - cast(H1.DATA_VALUE as float) > @amt_value
				or cast(H1.DATA_VALUE as float) - cast(H.DATA_VALUE as float) > @amt_value)	 
			END

			--insert STUCK fpr records that have been STUCK 
			if (@stuck_type='H' AND @stuck_value=3)
			BEGIN
				UPDATE T_QREST_DATA_HOURLY set VAL_IND=1, VAL_CD='STUCK' 
				--select * from T_QREST_DATA_HOURLY H
				where MONITOR_IDX=@mon_id
				and VAL_IND=0
				and ISNUMERIC(DATA_VALUE)=1
				and DATA_VALUE = (select DATA_VALUE from T_QREST_DATA_HOURLY H1 
									where DATA_DTTM_UTC=DATEADD(HOUR,1,H1.DATA_DTTM_UTC)
									and ISNUMERIC(H1.DATA_VALUE)=1
									and MONITOR_IDX=H1.MONITOR_IDX)
				and DATA_VALUE = (select DATA_VALUE from T_QREST_DATA_HOURLY H2 
									where DATA_DTTM_UTC=DATEADD(HOUR,2,H2.DATA_DTTM_UTC)
									and ISNUMERIC(H2.DATA_VALUE)=1
									and MONITOR_IDX=H2.MONITOR_IDX);	 
			END	

			--blank out LOST for rows that now have numeric
			UPDATE T_QREST_DATA_HOURLY set VAL_CD = null where MONITOR_IDX=@mon_id and VAL_CD='LOST' and VAL_IND=0 and DATA_VALUE is not null;

			--finally, update all other records for this monitor to 1
			UPDATE T_QREST_DATA_HOURLY set VAL_IND = 1 where MONITOR_IDX=@mon_id and VAL_IND = 0;

		END

		FETCH NEXT FROM mon_cursor INTO @mon_id, @minDate, @tz, @min_type, @min_value, @max_type, @max_value, @amt_type, @amt_value, @stuck_type, @stuck_value
	END   
	CLOSE mon_cursor;  
	DEALLOCATE mon_cursor;  

END