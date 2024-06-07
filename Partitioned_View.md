### Partitioned View

It’s a view that combines the full results of a number of physical tables that are logically separated by a boundary. You can think of the boundary the same way you’d think of a partitioning key. It can be classified as type of sharding but only in the local database and not spanning across multiple databases (actual sharding). There is no shard map manager in this case, check constraint on the table does that job. 

Each table in a partitioned view is its own little (or large) data island. Unlike partitioning, where the partitions all come together to form a logical unit. For example, if you run ALTER TABLE on a partitioned table, you alter the entire table. If you add an index to a partitioned table, it’s on the entire table. In some cases, you can target a specific partition, but not all. For instance, if you wanted to apply different compression types on different partitions for some reason, you can do that.

With partitioned views, you add some flexibility in not needing to align all your indexes to the partitioning key, or alter all your partitions at once, but you also add some administrative overhead in making sure that you iterate over all of the tables taking part in your partitioned view.

Partitioned Views are very useful in a scenario where downtime of indexing/reindexing/reorganizing on a very large mission critical table is high. One way to avoid significant downtime during such operations is to create partitioned views by breaking a large table into multiple small tables using partitioning column. Note that by partitioning a large table into multiple small tables performance may not increase but it will help in availability of the data. 

- Individual table(s) can be added or dropped independently
- Individual table(s) can be loaded independently, even they can be loaded in parallel
- Reindex/Reorganize can be performed on each table independently
- SELECT statement can be executed on each table independently, if needed

A partitioned view is a view defined by a UNION ALL of member tables structured in the same way, but stored separately as multiple tables in either the same instance of SQL Server or in a group of autonomous instances of SQL Server servers, called federated database servers. Note that Azure SQL Database doesn't support remote tables i.e. all tables in partitioned view MUST be local to database. 

Generally, a view is said to be a partitioned view if it is of the following form:

```
SELECT <select_list1> 
FROM T1 
UNION ALL 
SELECT <select_list2> 
FROM T2 
UNION ALL 
... 
SELECT <select_listn> 
FROM Tn;
```

```
1. The Select list:
In the column list of the view definition, select all columns in the member tables
Ensure that the columns in the same ordinal position of each select list are of the same type, including collations. It is not sufficient for the columns to be implicitly convertible types, including collations. It is not sufficient for the columns to be implicitly convertible types, as is generally the case for UNION. Also, at least one column (for example <col>) must appear in all the select lists in the same ordinal position. Define <col> in a way that the member tables T1,....,Tn have CHECK CONSTRAINTS C1,....,Cn on <col>, respectively.
The constraints must be in such a way that any specified value of <col> can satisfy, at most, one of the constraints C1,....,Cn so that the constraints form a set of disjointed or non-overlapping intervals. The column <col> on which disjointed constraints are defined is called the partitioning column. 
The same column cannot be used multiple times in the select list
2. Partitioning column
The partitioning column is a part of the Primary Key of the table
It cannot be computed, identity, default, or timestamp column
If there is more than one constraint on the same column in a member table, the Database Engine ignores all the constraints and does not consider them when determining whether the view is a partitioned view. To meet conditions of the partitioned view, ensure that there is only one partitioning constraint on the partitioning column.
There are no restrictions on the updatability of the partitioning column
3. Member tables, or underlying tables T1,....,Tn
The tables can be either local tables or tables from other computers that are running SQL Server that are referenced either through a four-part name or an OPENDATASOURCE or OPENROWSET based name. If one or more of the member tables are remote, the view is called distributed partitioned view, and additional conditions apply. Note that Azure SQL Database doesn't support remote tables i.e. all tables in partitioned view MUST be local to database. 
The same table cannot appear two times in the set of tables that are being combined with the UNION ALL statement
The member tables have all PRIMARY KEY constraints on the same number of columns
The member tables cannot have indexes created on computed columns in the table
All member tables in the view have the same ANSI padding setting
```

#### Conditions for Modifying Data in Partitioned Views:
The following restrictions apply to statements that modify the data in partitioned views:

The INSERT statement supplies values for all the columns in the view, even if the underlying member tables have a DEFAULT constraint for those columns or if they allow for null values. For those member table columns that have DEFAULT definitions, the statements cannot explicitly use the keyword DEFAULT
The value being inserted into the partitioning column satisfies at least one of the underlying constraints; otherwise, the insert action will fail with a constraint violation
UPDATE statements cannot specify the DEFAULT keyword as a value in the SET clause, even if the column has a DEFAULT value defined in the corresponding member table
Columns in the view that are an identity column in one of more of the member table cannot be modified by using an INSERT or UPDATE statement
If one of the member tables contains a timestamp column, the data cannot be modified by using an INSERT or UPDATE statement
If one of the member table contains a trigger or an ON UPDATE CASCADE/SET NULL/SET DEFAULT or ON DELETE CASCADE/SET NULL/SET DEFAULT constraint, the view cannot be modified
INSERT, UPDATE, and DELETE actions agains a partitioned view are not allowed if there is a self-join with the same view or with any of the member tables in the statement
Bulk importing data into a partitioned view is unsupported by bcp or the BULK INSERT and INSERT...SELECT * FROM OPENROWSET(BULK....) statements. However, you can insert multiple rows into a partitioned view by using the INSERT statement

#### Additional Conditions for Distributed Partitioned Views (Not applicable to Azure SQL DB):
For distributed partitioned views (when one or more tables are remote), the following additional conditions apply:

A distributed transaction will be started to guarantee atomicity across all nodes affected by the update
Set the XACT_ABORT SET option to ON for INSERT, UPDATE or DELETE statements to work
Any columns in the remote table of type smallmoney that referenced in a partitioned view are mapped as money. Therefore, the corresponding columns (in the same ordinal position in the select list) in the local tables must also be of type money
Any linked server in partitioned view cannot be a loopback linked server. This is a linked server that point to the same instance of SQL Server.

#### Case Study: vizpick database
Problem Statement

A database has very large table, 4.65 billion rows, and any maintenance operation like indexing/reorganizing/update statistics takes forever to complete and when such operations are running impact on other application queries observed

Proposed Solution

Table contains the data for ~5000 stores and breaking this table into multiple tables, one for each store, will be maintenance nightmare when it comes to changes in schema. To avoid this table is divided into 10 tables and based on the Country Code+Last digit of store number (0-9) a new checksum column is created and used that checksum value as partitioned column. At present only US data is in the table so the large table will get broken into 10 small tables (~500 million rows each). Check constraint on new checksum column is added and a partitioned view is created on top of 10 tables. Any INSERT/UPDATE/DELETE/SELECT statement will be executed against a view and depending on the Country Code+StoreNumber combination database optimizer will use respective table(s) to execute the query.

Schema

10 tables, store_tag_0........store_tage_9, are created with following schema, note that the code below contains only one table. All other tables will be same except name, PK Name & Check Constraint name. 

```
/****** Object:  Table [dbo].[store_tag_0]    Script Date: 6/10/2022 3:23:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[store_tag_0](
	[country_code] [char](2) NOT NULL,
	[store_nbr] [int] NOT NULL,
	[tag_id] [int] NOT NULL,
	[version] [int] NOT NULL,
	[tag_type_id] [smallint] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[last_seen_ts] [datetime] NULL,
	[chksum_store_nbr] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[store_tag_0] ADD  DEFAULT ((0)) FOR [version]
GO

ALTER TABLE [dbo].[store_tag_0]  WITH CHECK ADD  CONSTRAINT [chk_store_0] CHECK  (([chksum_store_nbr]=(43140)))
GO

ALTER TABLE [dbo].[store_tag_0] CHECK CONSTRAINT [chk_store_0]
GO
```
Data is loaded in to these 10 tables in parallel using the following script. A powershell is used to parallelized the insert using stored procedure. 

Total Time to insert: ~2 hours 30 minutes.

```
$lastdigits = 0,1,2,3,4,5,6,7,8,9


Get-Date | write-output
$lastdigits | ForEach-Object -ThrottleLimit 10 -Parallel  {

try{

$pd = "*********"
Invoke-Sqlcmd -ServerInstance ims-vizpick-db-prod-prod-603d66ea-secondary.database.windows.net -Database vizpick_06082022 -Username rax -Password $pd `
-Query "exec  spLoadAllStoresParallel $_"

}
catch{
$errorMessage = $_
write-output $dbname : $errorMessage }
} 
Get-Date | write-output
```

```
create or alter proc spLoadAllStoresParallel
@lastdigit tinyint
as

create table #storelist (storenbr int);

with temp as (
select 0 as n
union all
select n+1 from temp
where n < 10000
)
insert into #storelist
select n from temp
where right(n,1) = @lastdigit
option (maxrecursion 0)


declare @stno int = (
				select top 1 storenbr  from #storelist
				order by storenbr
				/*store_tag
				where country_code = 'US'
				order by store_nbr*/
				)
declare @tblname sysname = N'store_tag_'+cast(@lastdigit as nvarchar)
declare @sql nvarchar(max)

while 1 = 1
begin
	set @sql = N'insert into '+@tblname+N'
				(
	country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts,chksum_store_nbr)
	select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
	checksum(country_code+right(cast(store_nbr as varchar),1))
	from store_tag
	where country_code = ''US'' and store_nbr = '+cast(@stno as nvarchar)+' option(maxdop 8)'
	--print @sql
	begin tran
	exec sp_executesql @sql
	commit tran
	delete #storelist where storenbr = @stno
	set @stno = (select top 1 storenbr  from #storelist
				order by storenbr)

	if @stno is null or @stno = ''
		break
end
```

Once the data is loaded, create the primary key (clustered index) on all the tables. I tried to run it with many combinations but best timings were achieved by running 2 PK creation in parallel with maxdop = 8

Below is a sample command to execute the PK creation (again this is for just one table, modify the names for other tables). If you want to run it in parallel open 2 query window and run Alter command from both the windows:

```
Alter table store_tag_9 add constraint pk_store_tag_9 primary key(country_code, store_nbr, tag_id, chksum_store_nbr) with(maxdop=8)
```

Execution Times:

With maxdop 8 - 2 parallel Time: 13:41 ← Best time per index

With maxdop 16 - 2 parallel Time: 14:52

With maxdop 8 - 4 parallel Time: 24:50 ← Equivalent to 2 parallel index creation

With maxdop 32 - 1 index Time: 14:07

Below is a sample command to execute the creation of non-clustered index (again this is for just one table, modify the names for other tables). If you want to run it in parallel open 2/3/4 query window and execute create index. 

```
create index idx_store_tag_tagtypeid_lastseents on store_tag_5(tag_type_id) include (last_seen_ts) with (maxdop = 8)
```

On original table with 4.65 Billion rows the execution time of Nonclustered index was 7hours 49 minutes.

Execution Times:

With maxdop 8, 2 parallel, Time: 13:05

With maxdop 8, 4 parallel, Time 24:46

```
/****** Object:  View [dbo].[store_tag_view]    Script Date: 6/10/2022 8:39:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[store_tag_view] with schemabinding
as
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_0
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_1
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_2
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_3
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_4
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_5
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_6
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_7
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_8
union all
select country_code, store_nbr, tag_id, version, tag_type_id, created_on, last_seen_ts, 
chksum_store_nbr
from dbo.store_tag_9
GO
```

Query Execution Comparison. Note that this is just one query so full testing is required to make sure that every query has conditions in where clause for store_tag_view even if there are filters defined for same columns in other tables. In the following example for query on a VIEW I have modified the alias in WHERE Clause to refer the store_tag_view columns.

Query on original store_tag table:

```
SELECT c.store_nbr, c.country_code, c.tag_id as case_tag_id, c.gtin, c.location_tag_id, c.case_status_id,c.dept_nbr, c.repl_group_nbr, c.parent_case_tag_id, ISNULL(p.total_demand, 0) as total_demand, ISNULL(p.pick_qty, 0) as pick_qty, c.case_pack_qty, c.initial_qty, l.sgln_nbr, l.legacy_sgln_nbr, l.location_name, s.last_seen_ts
FROM case_tag c
INNER JOIN store_tag s on c.country_code = s.country_code AND c.store_nbr = s.store_nbr AND c.tag_id = s.tag_id
INNER JOIN location_tag l on c.country_code = l.country_code AND c.store_nbr = l.store_nbr AND c.location_tag_id = l.tag_id
LEFT OUTER JOIN suggested_pick p on c.country_code = p.country_code AND c.store_nbr = p.store_nbr AND c.repl_group_nbr = p.repl_group_nbr
WHERE c.country_code = 'US' AND c.store_nbr = 100 AND s.last_seen_ts > GETUTCDATE() - 11
```

#### Execution Statistics:
````
Table 'store_tag'. Scan count 1, logical reads 370, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'case_tag'. Scan count 1, logical reads 11807, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'location_tag'. Scan count 1, logical reads 8, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'suggested_pick'. Scan count 1, logical reads 134, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.
```

SQL Server Execution Times: CPU time = 531 ms,  elapsed time = 616 ms.


Query on store_tag_view, country_code/store_nbr/checksum number refers to store_tag_view instead of case_tag:

```
SELECT c.store_nbr, c.country_code, c.tag_id as case_tag_id, c.gtin, c.location_tag_id, c.case_status_id,c.dept_nbr, c.repl_group_nbr, c.parent_case_tag_id, ISNULL(p.total_demand, 0) as total_demand, ISNULL(p.pick_qty, 0) as pick_qty, c.case_pack_qty, c.initial_qty, l.sgln_nbr, l.legacy_sgln_nbr, l.location_name, s.last_seen_ts
FROM case_tag c
INNER JOIN store_tag_view s on c.country_code = s.country_code AND c.store_nbr = s.store_nbr AND c.tag_id = s.tag_id
INNER JOIN location_tag l on c.country_code = l.country_code AND c.store_nbr = l.store_nbr AND c.location_tag_id = l.tag_id
LEFT OUTER JOIN suggested_pick p on c.country_code = p.country_code AND c.store_nbr = p.store_nbr AND c.repl_group_nbr = p.repl_group_nbr
WHERE s.country_code = 'US' AND s.store_nbr = 100 AND  chksum_store_nbr = checksum('US'+right(cast(100 as varchar), 1))
and s.last_seen_ts > GETUTCDATE() - 11
```

Execution Statistics:

Table 'store_tag_0'. Scan count 1, logical reads 3433, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'location_tag'. Scan count 1, logical reads 8, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'suggested_pick'. Scan count 1, logical reads 134, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

Table 'case_tag'. Scan count 1, logical reads 9772, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.


SQL Server Execution Times:

   CPU time = 250 ms,  elapsed time = 245 ms.

Above query can be written in another way to avoid mistakes on not specifying the correct aliases for country_code, store_nbr, tag_id etc. There is not much performance difference, faster by couple of milliseconds, but this clearly shows that the required rows from correct store_tag table is fetched in a separate part of the query so no chance of mixing up later with aliases. Note that if a correct column from store_tag_view is not used the query will go and touch all the 10 tables even though other tables will return zero rows but it is an additional step for optimizer to perform. 


```
with temp as(
select country_code, store_nbr, tag_id, last_seen_ts
from store_tag_view s
WHERE s.country_code = 'US' AND s.store_nbr = 100 AND  chksum_store_nbr = checksum('US'+right(cast(100 as varchar), 1))
and s.last_seen_ts > GETUTCDATE() - 11
)
SELECT c.store_nbr, c.country_code, c.tag_id as case_tag_id, c.gtin, c.location_tag_id, c.case_status_id,c.dept_nbr, c.repl_group_nbr, 
c.parent_case_tag_id, ISNULL(p.total_demand, 0) as total_demand, ISNULL(p.pick_qty, 0) as pick_qty, 
c.case_pack_qty, c.initial_qty, l.sgln_nbr, l.legacy_sgln_nbr, l.location_name, s.last_seen_ts
FROM case_tag c
INNER JOIN temp s on c.country_code = s.country_code AND c.store_nbr = s.store_nbr AND c.tag_id = s.tag_id
INNER JOIN location_tag l on c.country_code = l.country_code AND c.store_nbr = l.store_nbr AND c.location_tag_id = l.tag_id
LEFT OUTER JOIN suggested_pick p on c.country_code = p.country_code AND c.store_nbr = p.store_nbr AND c.repl_group_nbr = p.repl_group_nbr
```
