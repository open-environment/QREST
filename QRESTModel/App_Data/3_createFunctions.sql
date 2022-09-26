USE [QREST]
GO


--TRIGGERS *******************************************************************************************************
--TRIGGERS *******************************************************************************************************
--TRIGGERS *******************************************************************************************************
/****** Object:  Trigger [dbo].[TRG_UPDATE_HOURLY]    Script Date: 4/1/2022 6:30:05 PM ******/
CREATE OR ALTER   TRIGGER [dbo].[TRG_UPDATE_HOURLY] ON [dbo].[T_QREST_DATA_FIVE_MIN]
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
	--4/1/2022 DOUG TIMMS change storage of timezone from poll config to site

	DECLARE @SumType varchar(4);
	DECLARE @SumTemp float;
	DECLARE @mon uniqueidentifier;
	DECLARE @sit uniqueidentifier;
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
		select @SumType=D.SUM_TYPE, @sit=C.SITE_IDX, @precision=D.ROUNDING
		from T_QREST_SITE_POLL_CONFIG C, T_QREST_SITE_POLL_CONFIG_DTL D 
		where C.POLL_CONFIG_IDX = D.POLL_CONFIG_IDX
		and C.ACT_IND=1
		and D.MONITOR_IDX = @mon;

		select @tz=S.LOCAL_TIMEZONE from T_QREST_SITES S where S.SITE_IDX=@sit;

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

--VIEWS *******************************************************************************************************
--VIEWS *******************************************************************************************************
--VIEWS *******************************************************************************************************
CREATE VIEW [dbo].[MONITOR_SNAPSHOT] as
select M.MONITOR_IDX, S.POLLING_LAST_RUN_DT,
(select max([LVL1_VAL_DT]) from T_QREST_DATA_HOURLY D where D.MONITOR_IDX = M.MONITOR_IDX) as lvl1,
(select max([LVL2_VAL_DT]) from T_QREST_DATA_HOURLY D where D.MONITOR_IDX = M.MONITOR_IDX) as lvl2
 from T_QREST_SITES S, T_QREST_MONITORS M
where S.SITE_IDX=M.SITE_IDX;

GO



CREATE VIEW [dbo].[USERLIST_DISPLAY_VIEW]
AS
SELECT u.USER_IDX, u.Email, u.FNAME, u.LNAME, u.LAST_LOGIN_DT, u.LockoutEndDateUtc, u.EmailConfirmed, u.CREATE_DT
, (select case when count(*) > 0 then '1' else '0' end from T_QREST_ROLES R, T_QREST_USER_ROLES UR where R.ROLE_IDX=UR.ROLE_IDX and UR.USER_IDX=U.USER_IDX and R.Name='GLOBAL ADMIN') as Name
, (select case when count(*) > 0 then '1' else '0' end from T_QREST_ORG_USERS OU where U.USER_IDX=OU.USER_IDX and OU.STATUS_IND='A' and OU.ACCESS_LEVEL='A') as TribalAdmin
, (select case when count(*) > 0 then '1' else '0' end from T_QREST_ORG_USERS OU where U.USER_IDX=OU.USER_IDX and OU.STATUS_IND='A' and OU.ACCESS_LEVEL in ('A','Q')) as QaReviewer
, (select case when count(*) > 0 then '1' else '0' end from T_QREST_ORG_USERS OU where U.USER_IDX=OU.USER_IDX and OU.STATUS_IND='A' and OU.ACCESS_LEVEL in ('A','Q','U')) as Operator
FROM T_QREST_USERS u 

GO



CREATE VIEW AIRNOW_LAST_HOUR as 
select S.AIRNOW_ORG, '840' + S.AIRNOW_SITE as AIRNOW_SITE, case when VAL_IND = 1 then '1' else '0' end as DATA_STATUS
,format(DATA_DTTM_UTC, 'yyyyMMddTHHmm') as DT, PM.PAR_CODE, DATA_VALUE, coalesce(H.UNIT_CODE, M.COLLECT_UNIT_CODE) as UNIT_CODE, M.POC
--, * 
from T_QREST_DATA_HOURLY H, T_QREST_MONITORS M, T_QREST_SITES S, T_QREST_REF_PAR_METHODS PM
where  M.MONITOR_IDX = H.MONITOR_IDX
and M.SITE_IDX = S.SITE_IDX
and M.PAR_METHOD_IDX = PM.PAR_METHOD_IDX
and S.AIRNOW_IND = 1
and S.AIRNOW_SITE is not null
and DATA_DTTM_UTC > GetDate()-1
and coalesce(H.UNIT_CODE, M.COLLECT_UNIT_CODE) is not null
and DATA_DTTM_UTC = 
(select max(H1.DATA_DTTM_UTC) from T_QREST_DATA_HOURLY H1, T_QREST_MONITORS M1
where M1.MONITOR_IDX = H1.MONITOR_IDX
and M1.site_IDX=M.SITE_IDX
and DATA_VALUE <> 'FEW');

GO


CREATE VIEW [dbo].[SITE_HEALTH] AS
select S.ORG_ID, S.SITE_IDX, S.SITE_ID, S.SITE_NAME, S.POLLING_ONLINE_IND, S.POLLING_LAST_RUN_DT, 
S.POLLING_NEXT_RUN_DT, S.AIRNOW_IND, S.AIRNOW_USR, S.AIRNOW_PWD, S.AIRNOW_ORG, S.AIRNOW_SITE, 
PC.LOGGER_TYPE, PC.TIME_POLL_TYPE, PC.[RAW_DURATION_CODE]
, case when PC.RAW_DURATION_CODE='1' then 
	(select max([DATA_DTTM_UTC]) from T_QREST_DATA_HOURLY H, T_QREST_MONITORS M where H.MONITOR_IDX=M.MONITOR_IDX and M.SITE_IDX = S.SITE_IDX and H.DATA_DTTM_UTC>getdate()-60) 
	when PC.RAW_DURATION_CODE='H' then
	(select max([DATA_DTTM]) from T_QREST_DATA_FIVE_MIN F, T_QREST_MONITORS M where F.MONITOR_IDX=M.MONITOR_IDX and M.SITE_IDX = S.SITE_IDX and F.DATA_DTTM>getdate()-10) 
	else null end
	as LAST_UTC_POLL
,GETUTCDATE() as CURR_UTC
from T_QREST_SITES S, T_QREST_SITE_POLL_CONFIG PC
where PC.SITE_IDX=S.SITE_IDX
and PC.ACT_IND=1
and S.POLLING_ONLINE_IND=1
GO



GO



--STORED PROCEDURES *******************************************************************************************************
--STORED PROCEDURES *******************************************************************************************************
--STORED PROCEDURES *******************************************************************************************************
CREATE PROCEDURE [dbo].[SP_VALIDATE_HOURLY] 
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
	--4/1/2022 change location of time zone to site table

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
	, S.LOCAL_TIMEZONE
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

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_AQS_REVIEW_STATUS]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_AQS_REVIEW_STATUS] 
	@siteid uniqueidentifier, 
	@adate datetime,
	@edate datetime
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given site and date range, shows the summary of data collection and validation

	--set @adate = '10/12/2019';
	--set @siteid = '563C2DE0-5323-4601-ABC4-6EEA894AF6BC';

	--CHANGE LOG
	---------------------------
	--20200324 change from UTC to local time
	--20200416 changed from hardcode month to variable date range for documents
	--20200616 add POC
	--20200727 made not AQS ready if unit is missing
	--20210123 only not AQS ready if no unit and no monitor unit
	--20210319 added overall percent complete calculation

	DECLARE @totHrs int = 1;


	if (@edate is null)
	BEGIN
		set @edate = DATEADD(s, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @adate) + 1, 0));
	END

	set @totHrs = DATEDIFF(hh, @adate, @edate) + 1;

	select PAR_CODE, PAR_NAME, MONITOR_IDX, POC, hrs, hrs_data, aqs_ready, lvl1_val_ind, lvl2_val_ind, doc_cnt
	, round(coalesce(cast(hrs_data+aqs_ready+lvl1_val_ind+lvl2_val_ind as float)/(.04*nullif(hrs,0)),0),0) as tot_pct
	from
	(
	select P.PAR_CODE, PP.PAR_NAME, M.MONITOR_IDX, M.POC,
		@totHrs as hrs,
		sum(rec) as hrs_data, 
		sum(aqs_ready) as aqs_ready, 
		cast(sum(sign(z.lvl1_val_ind)) as int) as lvl1_val_ind, 
		cast(sum(sign(lvl2_val_ind)) as int) as lvl2_val_ind
		,(select count(*) from T_QREST_ASSESS_DOCS dd 
			where (dd.MONITOR_IDX=M.MONITOR_IDX or (dd.SITE_IDX=@siteid and dd.MONITOR_IDX is null) )
			and (dd.START_DT between @adate and @edate or dd.END_DT between @adate and @edate or (dd.START_DT <= @adate and dd.END_DT > @edate))
			) as doc_cnt
	from
	(select H.MONITOR_IDX, 1 as rec, 
		case when isnumeric(H.data_value)=0 and H.AQS_NULL_CODE is null then 0 when COALESCE(H.UNIT_CODE,M.COLLECT_UNIT_CODE) is null then 0 else 1 end as aqs_ready, 
		isnull(H.lvl1_val_ind,0) as lvl1_val_ind, 
		isnull(H.lvl2_val_ind,0) as lvl2_val_ind
		from T_QREST_DATA_HOURLY H,T_QREST_MONITORS M
		where H.MONITOR_IDX = M.MONITOR_IDX
		and H.DATA_DTTM_LOCAL between @adate and @edate 
		and M.SITE_IDX = @siteid
	) Z
	, T_QREST_MONITORS M, T_QREST_REF_PAR_METHODS P, T_QREST_REF_PARAMETERS PP
	where Z.MONITOR_IDX=M.MONITOR_IDX
	and M.PAR_METHOD_IDX = P.PAR_METHOD_IDX
	and P.PAR_CODE = PP.PAR_CODE
	group by P.PAR_CODE, PP.PAR_NAME, M.MONITOR_IDX, M.POC) YYY;

END

GO


--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_RPT_DAILY]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_RPT_DAILY] 
	@siteid uniqueidentifier, 
	@mn int,
	@yr int,
	@dy int,
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. returns daily summary of air monitoring data

	--DECLARE @siteid uniqueidentifier;
	--DECLARE @mn int, @yr int, @dy int;
	--DECLARE @timetype varchar(1);
	--set @siteid = '563C2DE0-5323-4601-ABC4-6EEA894AF6BC';
	--set @mn = 10;
	--set @dy = 19;
	--set @yr = 2019;
	--set @timetype = 'L';

	IF @timetype='U'
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					P.MONITOR_IDX,
					r.PAR_CODE,
					pp.PAR_NAME,
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_UTC 
					left join [T_QREST_MONITORS] m on p.MONITOR_IDX = m.MONITOR_IDX 
					left join T_QREST_REF_PAR_METHODS r on m.PAR_METHOD_IDX = r.PAR_METHOD_IDX
					left join T_QREST_REF_PARAMETERS pp on r.PAR_CODE = pp.PAR_CODE
					where 
					SITE_IDX = @siteid
					and MONTH(HR)=@mn
					and YEAR(HR)=@yr
					and DAY(HR)=@dy
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchDay, PAR_CODE;
		END
	ELSE
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					P.MONITOR_IDX,
					r.PAR_CODE,
					pp.PAR_NAME,
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_LOCAL
					left join [T_QREST_MONITORS] m on p.MONITOR_IDX = m.MONITOR_IDX
					left join T_QREST_REF_PAR_METHODS r on m.PAR_METHOD_IDX = r.PAR_METHOD_IDX
					left join T_QREST_REF_PARAMETERS pp on r.PAR_CODE = pp.PAR_CODE
					where 					
					SITE_IDX = @siteid
					and MONTH(HR)=@mn
					and YEAR(HR)=@yr
					and DAY(HR)=@dy
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchDay, PAR_CODE;
		END

END

GO


--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_RPT_MONTHLY]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE   PROCEDURE [dbo].[SP_RPT_MONTHLY] 
	@monid uniqueidentifier, 
	@mn int,
	@yr int,
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. returns monthly summary of air monitoring data

--	DECLARE @monid uniqueidentifier;
--	DECLARE @mn int, @yr int;
--	set @monid = '4537f9ea-bb4f-488f-90b8-59d58d72c41a';
--	set @mn = 10;
--	set @yr = 2019;

	IF @timetype='U'
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_UTC and MONITOR_IDX = @monid
					  where MONTH(HR)=@mn
					  and YEAR(HR)=@yr
					  -- order by DATA_DTTM_LOCAL
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchDay;
		END
	ELSE
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_LOCAL and MONITOR_IDX = @monid
					  where MONTH(HR)=@mn
					  and YEAR(HR)=@yr
					  -- order by DATA_DTTM_LOCAL
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchDay;
		END
END

GO


--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_RPT_MONTHLY_SUMS]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_RPT_MONTHLY_SUMS] 
	@monid uniqueidentifier, 
	@mn int,
	@yr int,
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. returns monthly summary of air monitoring data

	--CHANGE LOG
	---------------------------
	--8/24/2020 Fix counting error if multiple polling configs for a monitor

	DECLARE @rounding int;
--	DECLARE @monid uniqueidentifier;
--	DECLARE @mn int, @yr int;
--	set @monid = '4537f9ea-bb4f-488f-90b8-59d58d72c41a';
--	set @mn = 10;
--	set @yr = 2019;

	select top 1 @rounding = rounding from T_QREST_SITE_POLL_CONFIG_DTL where [MONITOR_IDX]=@monid and ROUNDING is not null;
	if @rounding is null
		set @rounding = 2;

	IF @timetype='U'
		BEGIN
			select @monid as MONITOR_IDX, Z.*
			,(select count(*)*100/CNT  from [T_QREST_DATA_HOURLY] pp where pp.MONITOR_IDX=@monid 
								and ISNUMERIC(pp.DATA_VALUE)=1 
								and DATEPART(DAY, pp.DATA_DTTM_UTC) =DAY1
								and MONTH(pp.DATA_DTTM_UTC)=@mn 
								and YEAR(pp.DATA_DTTM_UTC)=@yr) as CAP
				from
			(SELECT 
				DATEPART(DAY, HR) AS DAY1, 
				MAX(cast(DATA_VALUE as float)) as MAX,
				MIN(cast(DATA_VALUE as float)) as MIN,
				round(AVG(cast(DATA_VALUE as float)),@rounding) as AVG,
				round(STDEV(cast(DATA_VALUE as float)),@rounding) as STDEV			
				,count(*) as CNT
			FROM 
				T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_UTC and MONITOR_IDX = @monid and ISNUMERIC(p.DATA_VALUE)=1
				where MONTH(HR)=@mn
				and YEAR(HR)=@yr
			GROUP BY DATEPART(DAY, HR) 
			) Z
			ORDER BY Z.DAY1;
		END
	ELSE
		BEGIN
			select @monid as MONITOR_IDX, Z.*
			,(select count(*)*100/CNT  from [T_QREST_DATA_HOURLY] pp where pp.MONITOR_IDX=@monid 
								and ISNUMERIC(pp.DATA_VALUE)=1 
								and DATEPART(DAY, pp.DATA_DTTM_LOCAL) =DAY1
								and MONTH(pp.DATA_DTTM_LOCAL)=@mn 
								and YEAR(pp.DATA_DTTM_LOCAL)=@yr) as CAP
				from
			(SELECT 
				DATEPART(DAY, HR) AS DAY1, 
				MAX(cast(DATA_VALUE as float)) as MAX,
				MIN(cast(DATA_VALUE as float)) as MIN,
				round(AVG(cast(DATA_VALUE as float)),@rounding) as AVG,
				round(STDEV(cast(DATA_VALUE as float)),@rounding) as STDEV			
				,count(*) as CNT
			FROM 
				T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_LOCAL and MONITOR_IDX = @monid and ISNUMERIC(p.DATA_VALUE)=1
				where MONTH(HR)=@mn
				and YEAR(HR)=@yr
			GROUP BY DATEPART(DAY, HR) 
			) Z
			ORDER BY Z.DAY1;
		END
END;

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_RPT_ANNUAL]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE     PROCEDURE [dbo].[SP_RPT_ANNUAL] 
	@monid uniqueidentifier, 
	@yr int,
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. returns annual summary of air monitoring data


	--DECLARE @monid uniqueidentifier;
	--DECLARE @yr int;
	--DECLARE @timetype varchar(1);
	--set @monid = '2C2A653E-51F9-4206-9EC7-5FBF768105AA';
	--set @yr = 2019;
	--set @timetype='L';

	IF @timetype='U'
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					FORMAT(HR, 'MMM', 'en-US') AS MonthDisp,
					DATEPART(MONTH, HR) AS SearchMonth,
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_UTC and MONITOR_IDX = @monid
					where YEAR(HR)=@yr
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchMonth, SearchDay;
		END
	ELSE
		BEGIN
			SELECT * FROM   
			(
				SELECT 
					FORMAT(HR, 'MMM', 'en-US') AS MonthDisp,
					DATEPART(MONTH, HR) AS SearchMonth,
					DATEPART(DAY, HR) AS SearchDay, 
					DATEPART(HOUR, HR) AS SearchHour,
					DATA_VALUE
				FROM 
					T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_LOCAL and MONITOR_IDX = @monid
					  where YEAR(HR)=@yr
			) t 
			 PIVOT (MIN(DATA_VALUE) FOR SearchHour IN 
					 ( [0],  [1],  [2],  [3],  [4],  [5], 
					   [6],  [7],  [8],  [9], [10], [11], 
					  [12], [13], [14], [15], [16], [17], 
					  [18], [19], [20], [21], [22], [23])) as pvt
			order by SearchMonth, SearchDay;
		END
END

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_RPT_ANNUAL_SUMS]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE     PROCEDURE [dbo].[SP_RPT_ANNUAL_SUMS] 
	@monid uniqueidentifier, 
	@yr int,
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. returns monthly summary of air monitoring data

	
	--CHANGE LOG
	---------------------------
	--8/24/2020 Fix counting error if multiple polling configs for a monitor

	DECLARE @rounding int;
	--DECLARE @monid uniqueidentifier;
	--DECLARE @yr int;
	--DECLARE @timetype varchar(1);
	--set @monid = '2C2A653E-51F9-4206-9EC7-5FBF768105AA';
	--set @yr = 2019;
	--set @timetype = 'L';

	select top 1 @rounding = rounding from T_QREST_SITE_POLL_CONFIG_DTL where [MONITOR_IDX]=@monid and ROUNDING is not null;
	if @rounding is null
		set @rounding = 2;

	IF @timetype='U'
		BEGIN
			select @monid as MONITOR_IDX, Z.*
			,(select count(*)*100/CNT  from [T_QREST_DATA_HOURLY] pp where pp.MONITOR_IDX=@monid 
								and ISNUMERIC(pp.DATA_VALUE)=1 
								and DATEPART(DAY, pp.DATA_DTTM_UTC) = DAY1
								and MONTH(pp.DATA_DTTM_UTC)=MONTH1 
								and YEAR(pp.DATA_DTTM_UTC)=@yr) as CAP
				from
			(SELECT 
				DATEPART(MONTH, HR) AS MONTH1, 
				DATEPART(DAY, HR) AS DAY1, 
				MAX(cast(DATA_VALUE as float)) as MAX,
				MIN(cast(DATA_VALUE as float)) as MIN,
				round(AVG(cast(DATA_VALUE as float)),@rounding) as AVG,
				round(STDEV(cast(DATA_VALUE as float)),@rounding) as STDEV			
				,count(*) as CNT
			FROM 
				T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_UTC and MONITOR_IDX = @monid and ISNUMERIC(p.DATA_VALUE)=1
				where YEAR(HR)=@yr
			GROUP BY DATEPART(MONTH, HR), DATEPART(DAY, HR) 
			) Z
			ORDER BY Z.MONTH1, Z.DAY1;
		END
	ELSE
		BEGIN
			select @monid as MONITOR_IDX, Z.*
			,(select count(*)*100/CNT  from [T_QREST_DATA_HOURLY] pp where pp.MONITOR_IDX=@monid 
								and ISNUMERIC(pp.DATA_VALUE)=1 
								and DATEPART(DAY, pp.DATA_DTTM_LOCAL)=DAY1
								and DATEPART(MONTH, pp.DATA_DTTM_LOCAL)=MONTH1 
								and YEAR(pp.DATA_DTTM_LOCAL)=@yr) as CAP
				from
			(SELECT 
				DATEPART(MONTH, HR) AS MONTH1, 
				DATEPART(DAY, HR) AS DAY1, 
				MAX(cast(DATA_VALUE as float)) as MAX,
				MIN(cast(DATA_VALUE as float)) as MIN,
				round(AVG(cast(DATA_VALUE as float)),@rounding) as AVG,
				round(STDEV(cast(DATA_VALUE as float)),@rounding) as STDEV			
				,count(*) as CNT
			FROM 
				T_SYS_HR S left join [T_QREST_DATA_HOURLY] p on S.HR=p.DATA_DTTM_LOCAL and MONITOR_IDX = @monid and ISNUMERIC(p.DATA_VALUE)=1
				where YEAR(HR)=@yr
			GROUP BY DATEPART(MONTH, HR), DATEPART(DAY, HR) 
			) Z
			ORDER BY Z.MONTH1, Z.DAY1;
		END
END;

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
	--20201117 added forcing hourly data rounded to nearest hour
	--20220404 added AQS "A" import type

	--get summary type to calculate
	select @import_typ=T.IMPORT_TYPE, @recalc_ind=T.RECALC_IND
	from T_QREST_DATA_IMPORTS T 
	where T.IMPORT_IDX = @import_idx;

	--initialize importing status
	UPDATE T_QREST_DATA_IMPORTS set SUBMISSION_STATUS='IMPORTING' where IMPORT_IDX = @import_idx;

	if (@import_typ = 'H' or @import_typ = 'H1' or @import_typ = 'A')
	BEGIN
		-- HANDLING HOURLY UPDATED RECORDS
		UPDATE H SET
			H.DATA_DTTM_UTC = dateadd(hour, datediff(hour, 0, T.DATA_DTTM_UTC), 0),
			H.DATA_DTTM_LOCAL = dateadd(hour, datediff(hour, 0, T.DATA_DTTM_LOCAL), 0),
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
		INSERT INTO T_QREST_DATA_HOURLY (DATA_HOURLY_IDX, MONITOR_IDX, DATA_DTTM_UTC, DATA_DTTM_LOCAL, DATA_VALUE, UNIT_CODE, VAL_IND, VAL_CD, MODIFY_DT, AQS_NULL_CODE, IMPORT_IDX, DATA_VALUE_NUM, AQS_QUAL_CODES)
		SELECT newid(), MONITOR_IDX, dateadd(hour, datediff(hour, 0, DATA_DTTM_UTC), 0), dateadd(hour, datediff(hour, 0, DATA_DTTM_LOCAL), 0), DATA_VALUE, UNIT_CODE, @recalc_ind ^ 1, VAL_CD, GetDate(), AQS_NULL_CODE, 
		@import_idx, DATA_VALUE_NUM, AQS_QUAL_CODES
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

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_VALIDATE_HOURLY_IMPORT]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_VALIDATE_HOURLY_IMPORT] 
	@import_id uniqueidentifier
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given import ID, sets MIN/MAX or JUMP validation


	--CHANGE LOG
	---------------------------
	--11/14/2020 add stuck value check


	--SET @import_id = '03d2a879-3bf8-460b-92cf-82c33ad62c83';

	SET NOCOUNT ON;


	--***************MIN************************
	UPDATE T_QREST_DATA_HOURLY 
	set VAL_IND=1, VAL_CD='MIN', VAL0_NOTIFY_IND=1
	FROM T_QREST_DATA_HOURLY H
	JOIN T_QREST_MONITORS M on H.MONITOR_IDX=M.MONITOR_IDX
	where IMPORT_IDX= @import_id
	and VAL_IND=0
	and VAL_CD IS NULL
	and ISNUMERIC(DATA_VALUE)=1 
	and M.ALERT_MIN_VALUE IS NOT NULL
	and DATA_VALUE_NUM<M.ALERT_MIN_VALUE;	


	--***************MAX************************
	UPDATE T_QREST_DATA_HOURLY 
	set VAL_IND=1, VAL_CD='MAX', VAL0_NOTIFY_IND=1
	FROM T_QREST_DATA_HOURLY H
	JOIN T_QREST_MONITORS M on H.MONITOR_IDX=M.MONITOR_IDX
	where IMPORT_IDX= @import_id
	and VAL_IND=0
	and VAL_CD IS NULL
	and ISNUMERIC(DATA_VALUE)=1 
	and M.ALERT_MAX_VALUE IS NOT NULL
	and DATA_VALUE_NUM>M.ALERT_MAX_VALUE;	



	--***************JUMP************************
	UPDATE H 
	set VAL_IND=1, VAL_CD='JUMP', VAL0_NOTIFY_IND=1
	FROM T_QREST_DATA_HOURLY H
	JOIN T_QREST_DATA_HOURLY H1 on H.MONITOR_IDX=H1.MONITOR_IDX and H1.DATA_DTTM_UTC=DATEADD(HOUR,-1,H.DATA_DTTM_UTC)
	JOIN T_QREST_MONITORS M on H.MONITOR_IDX=M.MONITOR_IDX
	where H.IMPORT_IDX= @import_id
	and H.VAL_IND=0
	and H.VAL_CD IS NULL
	and ISNUMERIC(H.DATA_VALUE)=1
	and ISNUMERIC(H1.DATA_VALUE)=1
	and M.ALERT_AMT_CHANGE_TYPE = 'H'
	and ((H.DATA_VALUE_NUM - H1.DATA_VALUE_NUM) > M.ALERT_AMT_CHANGE 
		or (H1.DATA_VALUE_NUM - H.DATA_VALUE_NUM) > M.ALERT_AMT_CHANGE )	 


	--***************STUCK************************
	;with cte_R as 
	(
	SELECT 
	DATA_HOURLY_IDX, T_QREST_DATA_HOURLY.MONITOR_IDX, DATA_DTTM_UTC, DATA_VALUE, IMPORT_IDX, M.ALERT_STUCK_TYPE,
	prev = LAG(DATA_VALUE,1) OVER(ORDER BY T_QREST_DATA_HOURLY.MONITOR_IDX, [DATA_DTTM_UTC]), 
	prev2 = LAG(DATA_VALUE,2) OVER(ORDER BY T_QREST_DATA_HOURLY.MONITOR_IDX, [DATA_DTTM_UTC]) 
	FROM T_QREST_DATA_HOURLY 
	JOIN T_QREST_MONITORS M on T_QREST_DATA_HOURLY.MONITOR_IDX=M.MONITOR_IDX
	where IMPORT_IDX=@import_id
	and VAL_IND=0
	and VAL_CD IS NULL
	and M.ALERT_STUCK_TYPE = 'H'
	and M.ALERT_STUCK_REC_COUNT = 3
	), cte_r2 as
	(
	select * from cte_R
	where DATA_VALUE=prev
	and prev=prev2 
	)
	update T_QREST_DATA_HOURLY set VAL_IND=1, VAL_CD='STUCK', VAL0_NOTIFY_IND=1
	from cte_r2 where T_QREST_DATA_HOURLY.DATA_HOURLY_IDX = cte_r2.DATA_HOURLY_IDX



	--***************update all other records for this import to 1***************
	UPDATE T_QREST_DATA_HOURLY set VAL_IND = 1 where IMPORT_IDX=@import_id and VAL_IND = 0;

END

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_FILL_LOST_DATA]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_FILL_LOST_DATA] 
	@sDate datetime,
	@eDate datetime,
	@monid uniqueidentifier, 
	@tz varchar(3)
AS
BEGIN   
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. fills in any data gaps with 'LIST' for hours with no data

	--CHANGE LOG
	------------------------------
	--1/27/2021 only fill past datetimes

	SET NOCOUNT ON;  

	--DECLARE @sDate datetime = '1/1/2020';
	--DECLARE @eDate datetime = '1/31/2020 23:00';
	--DECLARE @monid uniqueidentifier = 'FB0DEC3D-4275-4B89-B124-00E40FA37F8E';
	--DECLARE @tz varchar(3) = '-8';
 
	INSERT INTO T_QREST_DATA_HOURLY (DATA_HOURLY_IDX, [MONITOR_IDX], [DATA_DTTM_LOCAL], [DATA_DTTM_UTC], VAL_IND, VAL_CD)
	select NEWID(), @monid, S.HR as HR_LOCAL, DATEADD(HOUR,cast(@tz as int)*-1,S.HR) as HR_UTC, 1, 'LOST'
	from T_SYS_HR S
	where S.HR >= @sDate 
	and S.HR <= @eDate
	and S.HR < GetDate()
	and S.HR not in (select H.DATA_DTTM_LOCAL from T_QREST_DATA_HOURLY H where H.DATA_DTTM_LOCAL >= @sDate and H.DATA_DTTM_LOCAL <= @eDate and H.MONITOR_IDX = @monid);

END;

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[[SP_COUNT_LOST_DATA]]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_COUNT_LOST_DATA] 
	@monid uniqueidentifier, 
	@sdate datetime,
	@edate datetime
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given monitor and date range, counts the number of records that will be filled in with LOST

	--set @adate = '10/12/2019';
	--set @edate = '10/12/2019';
	--set @monid = '563C2DE0-5323-4601-ABC4-6EEA894AF6BC';

	--CHANGE LOG
	---------------------------

	select count(*) as CNT, min(HR) as MIN_DT, max(HR) as MAX_DT 
	from T_SYS_HR S 
	left join T_QREST_DATA_HOURLY H on S.HR = H.DATA_DTTM_LOCAL and H.MONITOR_IDX = @monid
	where S.HR >= @sDate 
	and S.HR <= @eDate
	and S.HR < GetDate()
	and DATA_HOURLY_IDX is null;

END;
	
GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[[SP_MONTHLY_STATS]]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
CREATE PROCEDURE [dbo].[SP_MONTHLY_STATS] 
	@sDate datetime,
	@eDate datetime,
	@monid uniqueidentifier
AS
BEGIN   
	--PROCEDURE DESCRIPTION
	-------------------------
	--Returns statistics broken down by month for a particualr monitor
	--CTE ranks all records by value, to find the highest and it's corresponding date (and the second highest and corresponding date
	--CTE2 groups all data by year/month and gives min/avg/stddev and record count for the month 

	--declare @sdate datetime;
	--declare @edate datetime;
	--declare @monid uniqueidentifier;
	--set @sdate = '1/1/2021';
	--set @edate = '12/31/2021 23:00';
	--set @monid = '45902582-a82f-4ec1-be85-3124065b744c';

	--CHANGE LOG
	------------------------------
	--4/29/2021  add proper rounding

	--***************** ROUNDING PRECISION DETERMINATION *********************
	declare @roundDec int;
	declare @roundDecLater int;

	select @roundDec=charindex('.',reverse(min(data_value)))-1 from T_QREST_DATA_HOURLY 
	where MONITOR_IDX=@monid
	and DATA_DTTM_LOCAL between @sdate and @edate
	and data_value_num is not null
	and data_value_num <> 0
	and NULLIF(aqs_null_code, '') is null;
	--select @roundDec = ROUNDING from T_QREST_SITE_POLL_CONFIG_DTL D, T_QREST_SITE_POLL_CONFIG C 
	--where D.POLL_CONFIG_IDX=C.POLL_CONFIG_IDX
	--and D.MONITOR_IDX = @monid 
	--and C.ACT_IND = 1;

	if (@roundDec = null)
		set @roundDec = 2;
	else if (@roundDec = -1)
		set @roundDec = 0;

	if (@roundDec = 0)
		set @roundDecLater = -1;
	else
		set @roundDecLater = @roundDec;
	--***************** END ROUNDING PRECISION DETERMINATION *********************

	WITH 
	CTE as (
		SELECT month(DATA_DTTM_LOCAL) mn, year(DATA_DTTM_LOCAL) yr
		, max_dt = DATA_DTTM_LOCAL
		, max1 = data_value
		, max1num = data_value_num
		, RN = ROW_NUMBER() OVER(PARTITION BY  month(DATA_DTTM_LOCAL),year(DATA_DTTM_LOCAL) ORDER BY data_value_num DESC)
		, PctRank = 100 - round(100. * PERCENT_RANK() OVER (PARTITION BY year(DATA_DTTM_LOCAL) ORDER BY data_value_num desc ),0)
		, Median1 = round(PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY data_value_num) OVER (PARTITION BY month(DATA_DTTM_LOCAL),year(DATA_DTTM_LOCAL)),@roundDec)
		, twen_fifth = round(PERCENTILE_CONT(0.25) WITHIN GROUP (ORDER BY data_value_num) OVER (PARTITION BY month(DATA_DTTM_LOCAL),year(DATA_DTTM_LOCAL)),@roundDec)
		, sevn_fifth = round(PERCENTILE_CONT(0.75) WITHIN GROUP (ORDER BY data_value_num) OVER (PARTITION BY month(DATA_DTTM_LOCAL),year(DATA_DTTM_LOCAL)),@roundDec)
	  FROM T_QREST_DATA_HOURLY
	  where DATA_DTTM_LOCAL between @sdate and @edate
	  and monitor_IDX=@monid	
	  and data_value_num is not null
	  and NULLIF(aqs_null_code, '') is null 
	 )
	,CTEQ as 
	 (select mn,yr,max(Median1) as median1, max(twen_fifth) as twen_fifth, max(sevn_fifth) as sevn_fifth from cte group by mn,yr)
	,CTE2 as (
		select month(hh.HR) as mn, year(hh.HR) as yr, 
		min(H.data_value_num) as min1, avg(H.data_value_num) as avg1, round(stdev(H.data_value_num),@roundDec) as stddev1
		,count(*) as tot_hrs
		,count(distinct case when NULLIF(aqs_null_code, '') is null AND data_value_num IS NOT NULL then [DATA_HOURLY_IDX] end) as hrs_data
		from T_SYS_HR hh left join T_QREST_DATA_HOURLY H on hh.HR = h.DATA_DTTM_LOCAL and monitor_IDX=@monid and NULLIF(aqs_null_code, '') is null 
		where hh.HR between @sdate and @edate	
		group by month(hh.HR), year(hh.HR)
	)
	select @monid as monid, CTE2.mn, CTE2.yr, 
	left(cast(cte2.min1 as varchar(20)), charindex('.',cast(cte2.min1 as varchar(20))) + @roundDecLater) as min1, 
	left(cast(cte2.avg1 as varchar(20)), charindex('.',cast(cte2.avg1 as varchar(20))) + @roundDecLater) as avg1, 
	stddev1, cte.max1, cte.max_dt, ctez.max1 as scnd_max, left(cast(cte.max1num-ctez.max1 as varchar(20)), charindex('.',cast(cte.max1num-ctez.max1 as varchar(20)))+@roundDecLater) as max2diff,  tot_hrs, hrs_data 
	,(select min(zzz.PctRank) from CTE zzz where zzz.max1>=cte2.avg1) as pct_yr_hrs_below_mn_mean
	,cteq.median1, cteq.twen_fifth, cteq.sevn_fifth
	from CTE2 
	left JOIN CTE on CTE2.MN=CTE.MN and CTE2.YR=CTE.YR and CTE.RN=1
	left JOIN CTE ctez on CTE2.MN=CTEz.MN and CTE2.YR=CTEz.YR and CTEz.RN=2
	left JOIN CTEQ on CTE2.MN=CTEQ.MN and CTE2.YR=CTEQ.YR
	order by CTE2.yr,CTE2.mn;

END;

GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[SP_FIVE_MIN_DATA_GAPS]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

CREATE PROCEDURE [dbo].[SP_FIVE_MIN_DATA_GAPS] 
	@siteid uniqueidentifier
AS
BEGIN   
	--PROCEDURE DESCRIPTION
	-------------------------
	--1. identifies where there are gaps in 5 minute data for a given site
	--CHANGE LOG
	------------------------------
	--4/17/2022 created

	--DECLARE @siteid uniqueidentifier
	--set @siteid = '14E3F3B4-8A76-4380-88CD-FB277292240A'

	SET NOCOUNT ON;  

	DECLARE @mon uniqueidentifier;  
	select top 1 @mon = M.monitor_idx from T_QREST_DATA_FIVE_MIN F, T_QREST_MONITORS M where M.MONITOR_IDX=F.MONITOR_IDX and M.SITE_IDX = @siteid and [DATA_DTTM] > GetDate()-30

	;with cte as
	(SELECT DATA_FIVE_IDX, 
			DATA_DTTM,
			LEAD(DATA_DTTM,1) OVER (ORDER BY DATA_DTTM) NEXT_DTTM
	FROM 
		T_QREST_DATA_FIVE_MIN where MONITOR_IDX = @mon and [DATA_DTTM]> GetDate()-30
	)
	select * from cte where datediff(mi, data_dttm, NEXT_DTTM) > 60

END;


GO



--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&[[SP_LATEST_POLLED_DATE]]&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

CREATE PROCEDURE [dbo].[SP_LATEST_POLLED_DATE] 
	@siteid uniqueidentifier, 
	@durcode varchar(1),
	@timetype varchar(1)
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given site and polling config settings, returns the the last time this site was polled

	--declare @siteid uniqueidentifier;
	--declare @durcode varchar(1);
	--declare @timetype varchar(1);
	--set @siteid = '14E3F3B4-8A76-4380-88CD-FB277292240A';
	--set @durcode = 'H';  --1=hourly  H=5 min
	--set @timetype = 'L';  --L=local U=utc



	DECLARE @lastDate datetime = null;


	if (@durcode = 'H' and @timetype='L')
	BEGIN
		select @lastDate = max([DATA_DTTM_LOCAL]) 
		from T_QREST_DATA_FIVE_MIN D, T_QREST_MONITORS M
		where M.monitor_idx=d.monitor_idx 
		and m.site_idx=@siteid;
	END
	else if (@durcode = 'H' and @timetype='U')
	BEGIN
		select @lastDate = max([DATA_DTTM]) 
		from T_QREST_DATA_FIVE_MIN D, T_QREST_MONITORS M
		where M.monitor_idx=d.monitor_idx 
		and m.site_idx=@siteid;
	END
	else if (@durcode = '1' and @timetype='L')
	BEGIN
		select @lastDate = max([DATA_DTTM_LOCAL]) 
		from T_QREST_DATA_HOURLY D, T_QREST_MONITORS M
		where M.monitor_idx=d.monitor_idx 
		and m.site_idx=@siteid;
	END
	else if (@durcode = '1' and @timetype='U')
	BEGIN
		select @lastDate = max([DATA_DTTM_UTC]) 
		from T_QREST_DATA_HOURLY D, T_QREST_MONITORS M
		where M.monitor_idx=d.monitor_idx 
		and m.site_idx=@siteid;
	END

	select @lastDate;

END

GO





--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&   TRAINING     &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&   TRAINING     &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&   TRAINING     &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&



CREATE PROCEDURE [dbo].[SP_TRAIN_COURSE_USER_PROGRESS] 
	@useridx varchar(128) 
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given user lists all available courses and the user's progress 

	--set @useridx = '8e4df1f8-0072-48d3-9bd5-bafd03c7ecfb';

	--CHANGE LOG
	---------------------------
	--5/14/2021 add course complete date

	select C.COURSE_IDX, C.COURSE_NAME, C.COURSE_DESC, case when CU.COURSE_IDX is not null then 1 else 0 end as COURSE_COMP_IND, CU.CREATE_DT
	,(select count(*) from T_QREST_TRAIN_COURSE_LESSON TCL where TCL.COURSE_IDX = C.COURSE_IDX) as COURSE_LESSON_TOT
	,(select count(*) from [T_QREST_TRAIN_LESSON_USER] TLU, T_QREST_TRAIN_COURSE_LESSON TCL where TCL.COURSE_IDX = C.COURSE_IDX and TLU.LESSON_IDX = TCL.LESSON_IDX and TLU.USER_IDX = @useridx) as COURSE_LESSON_COMP
	 from T_QREST_TRAIN_COURSE C
	 left join T_QREST_TRAIN_COURSE_USER CU on C.COURSE_IDX=CU.COURSE_IDX and CU.USER_IDX = @useridx
	where C.ACT_IND=1
	order by C.COURSE_SEQ;

END


GO




CREATE PROCEDURE [dbo].[SP_TRAIN_LESSON_USER_PROGRESS] 
	@useridx varchar(128),
	@courseidx uniqueidentifier 
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given user and course, lists all of the course's lessons and the user's progress 

	--set @useridx = '8e4df1f8-0072-48d3-9bd5-bafd03c7ecfb';
	--set @courseidx = 'fd677055-1a43-4993-a9e7-ce5e5ba9805e';

	--CHANGE LOG
	---------------------------
	select TCL.COURSE_IDX, TCL.LESSON_IDX, TCL.LESSON_SEQ, L.LESSON_TITLE, L.LESSON_DESC, case when TLU.LESSON_IDX is not null then 1 else 0 end as LESSON_COMP_IND, TLU.CREATE_DT
	,(select count(*) from T_QREST_TRAIN_LESSON_STEP_USER LSU, T_QREST_TRAIN_LESSON_STEP LS where LS.LESSON_STEP_IDX= LSU.LESSON_STEP_IDX and LS.LESSON_IDX=TCL.LESSON_IDX and LSU.USER_IDX = @useridx) as LESSON_STEPS_COMP
	from T_QREST_TRAIN_COURSE_LESSON TCL, T_QREST_TRAIN_LESSON L
	left join T_QREST_TRAIN_LESSON_USER TLU on L.LESSON_IDX = TLU.LESSON_IDX and TLU.USER_IDX = @useridx
	where TCL.LESSON_IDX = L.LESSON_IDX
	and TCL.COURSE_IDX = @courseidx
	order by TCL.LESSON_SEQ;
END

GO


CREATE PROCEDURE [dbo].[SP_TRAIN_STEP_USER_PROGRESS] 
	@useridx varchar(128),
	@lessonidx uniqueidentifier 
AS
BEGIN
	--PROCEDURE DESCRIPTION
	-------------------------
	--for a given user and lesson, lists all of the lesson's steps and the user's progress 
	--declare @useridx varchar(128);
	--declare @lessonidx uniqueidentifier;
	--set @useridx = '8e4df1f8-0072-48d3-9bd5-bafd03c7ecfb';
	--set @lessonidx = '4D325449-CE1D-4E56-BA36-41C6442AEFCE';

	--CHANGE LOG
	---------------------------
	select TLS.*, case when TLSU.LESSON_STEP_IDX is not null then 1 else 0 end as STEP_COMP_IND, TLSU.CREATE_DT
	from T_QREST_TRAIN_LESSON_STEP TLS
	left join T_QREST_TRAIN_LESSON_STEP_USER TLSU on TLS.LESSON_STEP_IDX = TLSU.LESSON_STEP_IDX and TLSU.USER_IDX = @useridx
	where TLS.LESSON_IDX = @lessonidx
	order by TLS.LESSON_STEP_SEQ;
END


GO

CREATE VIEW TRAINING_SNAPSHOT as
  		select U.USER_IDX, U.[FNAME], U.LNAME, TC.COURSE_IDX, TC.COURSE_NAME, 
		(select count(*) from T_QREST_TRAIN_COURSE_LESSON TCL2, T_QREST_TRAIN_LESSON_STEP TLS2 where TCL2.LESSON_IDX=TLS2.LESSON_IDX and TCL2.COURSE_IDX = TC.COURSE_IDX) as TotalCourseSteps
		,(select min(create_dt) 
			from T_QREST_TRAIN_LESSON_STEP_USER TLSU1, T_QREST_TRAIN_LESSON_STEP TLS1, T_QREST_TRAIN_COURSE_LESSON TCL1 
			where TLSU1.LESSON_STEP_IDX=TLS1.LESSON_STEP_IDX and TLS1.LESSON_IDX=TCL1.LESSON_IDX and TLSU1.USER_IDX=U.USER_IDX) as CourseStartDate		
		,count(*) as StepsCompleted
		,(select count(*) from T_QREST_TRAIN_COURSE_USER TCU where TCU.USER_IDX=U.USER_IDX and TCU.COURSE_IDX=TC.COURSE_IDX) as CertIssuedInd
		,(select top 1 create_dt from T_QREST_TRAIN_COURSE_USER TCU where TCU.USER_IDX=U.USER_IDX and TCU.COURSE_IDX=TC.COURSE_IDX) as CertIssuedDate
		,(select case when count(*)>1 then '(multiple)' else
		(select top 1 org_name from t_qrest_organizations o, t_qrest_org_users ou where o.org_Id=ou.org_id and ou.user_IDX=u.user_IDX 
		  order by case when ou.ACCESS_LEVEL='A' then 1 when ou.ACCESS_LEVEL='U' then 2 when ou.ACCESS_LEVEL='Q' then 3 else 4 end)
		  end
		 from t_qrest_organizations o, t_qrest_org_users ou where o.org_Id=ou.org_id and ou.user_IDX=u.user_IDX and OU.ACCESS_LEVEL='A') as ORG_NAME
		from 
		T_QREST_TRAIN_LESSON_STEP_USER TLSU,
		T_QREST_TRAIN_LESSON_STEP TLS,
		T_QREST_USERS U
	,T_QREST_TRAIN_COURSE_LESSON TCL
	,T_QREST_TRAIN_COURSE TC
		where TLSU.USER_IDX=U.USER_IDX
		and TLSU.LESSON_STEP_IDX=TLS.LESSON_STEP_IDX
		and TLS.LESSON_IDX=TCL.LESSON_IDX
		and TCL.COURSE_IDX=TC.COURSE_IDX
		group by U.USER_IDX, U.[FNAME], U.LNAME, TC.COURSE_IDX, TC.COURSE_NAME;

GO