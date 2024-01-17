ALTER TABLE vetting
ADD commentFile varbinary(max)

ALTER TABLE vetting
ADD commentFileName nvarchar(200)

------------------------------------------------------------------------------

CREATE TABLE [dbo].[QuestionTypes](
	[TypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeCode] [nchar](20) NOT NULL,
	[TypeDescription] [nchar](200) NOT NULL,
PRIMARY KEY CLUSTERED ([TypeId] ASC)
)

insert into dbo.QuestionTypes (typecode, typedescription)
values ('Management', 'Management') -- 1

insert into dbo.QuestionTypes (typecode, typedescription)
values ('SIRE', 'SIRE') -- 2

------------------------------------------------------------------------------

ALTER TABLE QuestionPoolNew
ADD QuestionTypeID int

ALTER TABLE [dbo].[QuestionPoolNew] ADD  DEFAULT ((2)) FOR [QuestionTypeID]

update QuestionPoolNew
set QuestionTypeID = 1 -- Management
where Origin = 0

update QuestionPoolNew
set QuestionTypeID = 2 -- SIRE
where QuestionTypeID is NULL

------------------------------------------------------------------------------

ALTER TABLE dbo.VIQ
ADD Id int identity(1,1) not null

ALTER TABLE dbo.VIQ
add CONSTRAINT pk_viq_ID primary key(Id)


-------------------------------------------------------------------------------------------
-- New changes about QuestionPoolNew table parent child relation , these need to be run one by one. 


 ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew]
 ADD CONSTRAINT PK_QuestionId PRIMARY KEY(questionid)
--------------------------------------------------------------------------
 ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew] ADD ParentId uniqueidentifier NULL;
-----------------------------------------------------------------------------
 ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew]
 ADD CONSTRAINT[FK_QuestionPool_Parent] FOREIGN KEY([ParentId])
 REFERENCES[AttendanceV1prod].[dbo].[QuestionPoolNew] ([questionid])
 
 ---------------------------------------------------------------------------------------
 --New changes about ordering 
 
  ALTER TABLE [AttendanceV1prod].[dbo].[VIQ] ADD ShowAfterId int NULL;
----------------------------------------------------------------------------------------
  UPDATE [AttendanceV1prod].[dbo].[VIQ]
  SET ShowAfterId = GlobalDisplayIndex -1 
  
  ------------------------------------------------------
  --Changes about User , Roles and VettingInfo 
  


CREATE TABLE [dbo].[UserQuestionnaire](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[QId] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_UserQuestionnaire_User_Q
   ON [UserQuestionnaire] (UserId, QId);

UPDATE Users SET UserId=0 WHERE UserId IS NULL
ALTER TABLE Users ALTER COLUMN UserId INTEGER NOT NULL 
ALTER TABLE Users ADD primary key (UserId)  
  
 ALTER TABLE[AttendanceV1prod].[dbo].UserQuestionnaire
 ADD CONSTRAINT[FK_UserQuestionnaire_Users] FOREIGN KEY([UserId])
 REFERENCES[AttendanceV1prod].[dbo].[Users] ([UserId])
 
   ALTER TABLE[AttendanceV1prod].[dbo].UserQuestionnaire
 ADD CONSTRAINT[FK_UserQuestionnaire_Questionnaires] FOREIGN KEY([QId])
 REFERENCES[AttendanceV1prod].[dbo].[VIQInfo] ([QId])
 
 
 ------ this below one is for backward compatibility , it will be inserting all existing userId's with existing ViqInfo Id's into newly added table  
Declare @UserId int;
Declare @QId int;

DECLARE Cur1 CURSOR FOR
    SELECT Distinct UserId From Users;

OPEN Cur1
FETCH NEXT FROM Cur1 INTO @UserId;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing UserId: ' + Cast(@UserId as Varchar);
    DECLARE Cur2 CURSOR FOR
        SELECT Distinct QId FROM VIQInfo
    OPEN Cur2;
    FETCH NEXT FROM Cur2 INTO @QId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT 'Found QID: ' + Cast(@QId as Varchar);
		INSERT INTO UserQuestionnaire
           ([UserId]
           ,[QId])
		VALUES
           (@UserId
           ,@QId)
        FETCH NEXT FROM Cur2 INTO @QId;
    END;
    CLOSE Cur2;
    DEALLOCATE Cur2;
    FETCH NEXT FROM Cur1 INTO @UserId;
END;
PRINT 'DONE';
CLOSE Cur1;
DEALLOCATE Cur1;
  
  ---------------------------------------------------------------
  --Multiple attachments changes :
  
  CREATE TABLE [dbo].[VettingAttachment](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[VETId] [int] NOT NULL,
	[ObjectId] [uniqueidentifier] NOT NULL,
	[commentFile] varbinary(max) NULL,
	[commentFileName]  nvarchar(200) NULL
) ON [PRIMARY]
GO

   ALTER TABLE [dbo].[VettingAttachment]
   ADD CONSTRAINT FK_VettingAttachment_Vetting
   FOREIGN KEY(VETId, ObjectId)
   REFERENCES dbo.Vetting (VETId, ObjectId)
   ON DELETE CASCADE
  
  
--Below one is for restore old comment files data from Vetting table to newly added VettingAttachment table
INSERT INTO [dbo].[VettingAttachment]
           ([VETId]
           ,[ObjectId]
           ,[commentFile]
           ,[commentFileName])
SELECT VETId,ObjectId,commentFile, commentFileName FROM Vetting where  commentFile is not null

-- Then we can remove columns from Vetting table 

ALTER TABLE .[dbo].[Vetting]
DROP COLUMN [commentFile]


ALTER TABLE .[dbo].[Vetting]
DROP COLUMN [commentFileName]

----------------------------------------------------------

  ALTER TABLE [dbo].[VettingInfo]
  ADD CarriedOutStatus TINYINT NULL 
  
  -----------------------------------------------------------
  
  ALTER TABLE [dbo].[VettingInfo]
  ADD Status TINYINT NULL 