ALTER PROCEDURE [dbo].[SP_AQS_REVIEW_STATUS] 
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
	DECLARE @totHrs int = 1;


	if (@edate is null)
	BEGIN
		set @edate = DATEADD(s, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @adate) + 1, 0));
	END

	set @totHrs = DATEDIFF(hh, @adate, @edate) + 1;


	select P.PAR_CODE, PP.PAR_NAME, M.MONITOR_IDX, 
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
		case when isnumeric(H.data_value)=0 and H.AQS_NULL_CODE is null then 0 else 1 end as aqs_ready, 
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
	group by P.PAR_CODE, PP.PAR_NAME, M.MONITOR_IDX;

END