SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER ON;

SET XACT_ABORT ON;

SET ARITHABORT ON;
GO
------------------------------
--Security Schema
------------------------------
------------------------------
--Drop Calls
------------------------------



DROP TRIGGER IF EXISTS [Security].[vwContextInfoInsetUpdateDeleteTrigger];
GO

--Remove existing security policy
DROP SECURITY POLICY  IF EXISTS Security.EntitySelectFilter
GO
DROP SECURITY POLICY  IF EXISTS Security.EntityAssetHierarchySelectFilter
GO
DROP SECURITY POLICY  IF EXISTS Security.EntityServiceHierarchySelectFilter
GO
--rls function
DROP FUNCTION IF EXISTS [Security].[fnSecurityPredicateSelectEntity]
GO
DROP FUNCTION IF EXISTS [Security].[fnSecurityPredicateSelectEntityServiceHierarchy]
GO
DROP FUNCTION IF EXISTS [Security].[fnSecurityPredicateSelectEntityAssetHierarchy]
GO
DROP VIEW IF EXISTS [Security].[vwContextInfo];
GO
--context functions
DROP FUNCTION IF EXISTS [Security].[GetContextTable];
DROP FUNCTION IF EXISTS [Security].[GetContextBuffer];
DROP FUNCTION IF EXISTS [Security].[GetContextFieldDelimiter];
DROP FUNCTION IF EXISTS [Security].[GetContextRowDelimiter];
GO
DROP SCHEMA IF EXISTS [Security];
GO

------------------------------
--Create Calls
------------------------------


CREATE SCHEMA [Security];
GO

CREATE FUNCTION [Security].[GetContextFieldDelimiter] (
                                                      )
RETURNS CHAR
WITH SCHEMABINDING
AS
     BEGIN
         RETURN CHAR(2);   -- ',' 
     END;
GO

CREATE FUNCTION [Security].[GetContextRowDelimiter] (
                                                    )
RETURNS CHAR
WITH SCHEMABINDING
AS
     BEGIN
         RETURN CHAR(3);   -- '|'
     END;
GO

CREATE FUNCTION [Security].[GetContextBuffer] (
                                              )
RETURNS VARCHAR(128)
WITH SCHEMABINDING
AS
     BEGIN
         RETURN RTRIM(REPLACE(CONVERT(VARCHAR(128) , CONTEXT_INFO()) , CHAR(0) , CHAR(32)));
     END;
GO
GO

CREATE FUNCTION [Security].[GetContextTable] (
                                             )
RETURNS @CONTEXT TABLE (
                       [KEY]   VARCHAR(126) ,
                       [VALUE] VARCHAR(126)
                       )
WITH SCHEMABINDING
AS
     BEGIN
         DECLARE @BUFFER VARCHAR(128)= Security.GetContextBuffer();
         DECLARE @FPTR INT= CHARINDEX(Security.GetContextFieldDelimiter() , @BUFFER);
         DECLARE @RPTR INT= CHARINDEX(Security.GetContextRowDelimiter() , @BUFFER);
         WHILE @RPTR > 0
             BEGIN
                 INSERT INTO @CONTEXT
                        SELECT SUBSTRING(@BUFFER , 1 , ABS(@FPTR - 1)) , SUBSTRING(@BUFFER , @FPTR+1 , @RPTR-@FPTR-1)
                        WHERE @RPTR > NULLIF(@FPTR , 0) + 1;
                 SELECT @BUFFER = SUBSTRING(@BUFFER , @RPTR+1 , 128) , @FPTR = CHARINDEX(Security.GetContextFieldDelimiter() , @BUFFER) , @RPTR = CHARINDEX(Security.GetContextRowDelimiter() , @BUFFER);
             END;
         RETURN;
     END;
GO

CREATE VIEW [Security].[vwContextInfo]
WITH SCHEMABINDING
AS
     SELECT [KEY] , [VALUE]
     FROM [Security].[GetContextTable]();
GO

CREATE TRIGGER [Security].[vwContextInfoInsetUpdateDeleteTrigger] ON [Security].[vwContextInfo]
INSTEAD OF INSERT , UPDATE , DELETE
AS
     BEGIN
         SET NOCOUNT ON;
         DECLARE @CONTEXTTABLE TABLE (
                                     [KEY]   VARCHAR(126) ,
                                     [VALUE] VARCHAR(126)
                                     );
         INSERT INTO @CONTEXTTABLE
                SELECT *
                FROM Security.[GetContextTable]();
         IF EXISTS ( SELECT *
                     FROM inserted
                   )
            AND
            EXISTS ( SELECT *
                     FROM deleted
                   )
             BEGIN
                 --Update changed rows 
                 UPDATE @CONTEXTTABLE
                        SET [KEY] = i.[KEY] , [VALUE] = i.[VALUE]
                 FROM @CONTEXTTABLE ct INNER JOIN inserted i ON ct.[KEY] = i.[KEY]
                                       INNER JOIN deleted d ON d.[KEY] = i.[KEY]
                 WHERE i.[VALUE] <> d.[VALUE];
             END;
         ELSE
             BEGIN
                 IF EXISTS ( SELECT *
                             FROM deleted
                           )
                     BEGIN
                         --Delete removed rows
                         DELETE FROM @CONTEXTTABLE
                         WHERE [KEY] IN ( SELECT d.[KEY]
                                          FROM inserted AS i RIGHT JOIN deleted AS d ON d.[KEY] = i.[KEY]
                                          WHERE i.[KEY] IS NULL
                                        );
                     END;
                 ELSE
                     BEGIN
                         IF EXISTS ( SELECT *
                                     FROM inserted
                                   )
                             BEGIN
                                 --Insert New Rows  
                                 IF EXISTS ( SELECT *
                                             FROM @CONTEXTTABLE
                                             WHERE [KEY] IN ( SELECT i.[KEY]
                                                              FROM inserted AS i LEFT JOIN deleted AS d ON d.[KEY] = i.[KEY]
                                                              WHERE d.[KEY] IS NULL
                                                            )
                                           )
                                     BEGIN
                                         RAISERROR('Duplicate Key IN Context.' , 16 , 1);
                                     END;
                                 ELSE
                                     BEGIN
                                         INSERT INTO @CONTEXTTABLE
                                                SELECT i.[KEY] , i.[VALUE]
                                                FROM inserted AS i LEFT JOIN deleted AS d ON d.[KEY] = i.[KEY]
                                                WHERE d.[KEY] IS NULL;
                                     END;
                             END;
                     END;
             END;
         DECLARE @SUMLEN INT;
         SELECT @SUMLEN = SUM(LEN([KEY]) + LEN([VALUE]) + 2)
         FROM @CONTEXTTABLE;
         IF @SUMLEN > 128
             BEGIN
                 RAISERROR('Context_Info is full.' , 16 , 1);
             END;
         ELSE
             BEGIN
                 --SELECT * FROM @CONTEXTTABLE
                 --Convert this to varbinary(128)
                 DECLARE @BUFFER VARCHAR(255);
                 DECLARE @COUNTER INT= 1;
                 DECLARE @KEY VARCHAR(3)= '';
                 DECLARE @Row VARCHAR(255)= '';
                 WHILE @Key IS NOT NULL
                     BEGIN
                         SET @Key = NULL;
                         SELECT @Key = [KEY] , @Row = CONVERT(VARCHAR(3) , [KEY]) + Security.GetContextFieldDelimiter() + CONVERT(VARCHAR(250) , [VALUE]) + Security.GetContextRowDelimiter()
                         FROM ( SELECT [KEY] , [VALUE] , ROW_NUMBER() OVER(ORDER BY [KEY]) AS RowNum
                                FROM @CONTEXTTABLE
                              ) AS RINSERTED
                         WHERE RowNum = @COUNTER;
                         IF @Key IS NOT NULL
                             BEGIN
                                 SELECT @BUFFER = CONCAT(@BUFFER , @Row);
                                 --SELECT @BUFFER,@Row;
                             END;
                         SET @COUNTER = @COUNTER + 1;
                     END;
                 DECLARE @CONTEXT VARBINARY(128);
                 SET @BUFFER = ISNULL(@BUFFER , '');
                 SELECT @CONTEXT = CONVERT(VARBINARY(128) , @BUFFER);
                 SET CONTEXT_INFO @CONTEXT;
             END;
     END;
GO

------------------------------
--Test Data Calls
------------------------------
--DELETE Security.ContextView WHERE [Key] = '1'
--SELECT * FROM Security.ContextView 
--UPDATE Security.ContextView SET [Value] = 'OE00000002' WHERE [Key] = '5'
-- INSERT INTO Security.ContextView ([KEY],[VALUE])
--   SELECT '5', 'OE00000002'
--   UNION
--   SELECT '6', 'OE00000001'
--   UNION
--   SELECT '4' AS [KEY], 'nooruddin.kapasi@thisistheendoftheworldvalueforemailaddress1234567890.com' AS VALUE
-- INSERT INTO Security.ContextView ([KEY],[VALUE])
--   SELECT '7', 'OE00000001'
------------------------------
--Meta Schema
------------------------------
------------------------------
--Drop Calls
------------------------------
--Tag Trigger


DROP TRIGGER IF EXISTS LowercaseTags_Insteadof_Trigger_On_Tag;

DROP TRIGGER IF EXISTS Add_User_Info_AfterInsertTrigger_On_Tag;
GO

--Tag


IF OBJECT_ID(N'[Meta].[Tag]' , N'U') IS NOT NULL
    BEGIN
        ALTER TABLE Meta.Tag SET(SYSTEM_VERSIONING = OFF);
        ALTER TABLE Meta.Tag DROP PERIOD FOR SYSTEM_TIME;
    END;

DROP TABLE IF EXISTS Meta.Tag;

DROP TABLE IF EXISTS Meta.TagHistory;
GO
--TagType 


DROP TRIGGER IF EXISTS LowercaseTags_Insteadof_Trigger_On_TagType;

DROP TABLE IF EXISTS Meta.TagType;
GO
--Entity Stored Procs


DROP PROCEDURE IF EXISTS Meta.uspNewEntityCode;

DROP PROCEDURE IF EXISTS Meta.uspReparentAssetNode;

DROP PROCEDURE IF EXISTS Meta.uspReparentServiceNode;

DROP PROCEDURE IF EXISTS Meta.uspCreateEntity;

DROP PROCEDURE IF EXISTS Meta.uspUpdateEntity;

DROP PROCEDURE IF EXISTS Meta.uspDeleteEntity;
GO
--Entity
--need to remove this later
DROP VIEW IF EXISTS Meta.vwUniqueEntityAssetNodeId;
DROP VIEW IF EXISTS Meta.vwUniqueEntityServiceNodeId;

--used by all ready queries
DROP VIEW IF EXISTS  Meta.vwEntity
DROP TRIGGER IF EXISTS Meta.Add_User_Info_AfterInsertTrigger_On_Entity;
DROP TRIGGER IF EXISTS Meta.Add_User_Info_AfterInsertTrigger_On_EntityAssetHierarchy;
DROP TRIGGER IF EXISTS Meta.Add_User_Info_AfterInsertTrigger_On_EntityServiceHierarchy;

IF OBJECT_ID(N'[Meta].[EntityAssetHierarchy]' , N'U') IS NOT NULL
    BEGIN
		ALTER TABLE [Meta].[EntityAssetHierarchy] SET ( SYSTEM_VERSIONING = OFF )
        ALTER TABLE [Meta].[EntityAssetHierarchy] DROP PERIOD FOR SYSTEM_TIME;
    END;

IF ( OBJECT_ID('FK_EntityServiceHierarchy_EntityId' , 'F') IS NOT NULL )
    BEGIN
        ALTER TABLE Meta.EntityAssetHierarchy DROP CONSTRAINT FK_EntityServiceHierarchy_EntityId;
    END;

IF ( OBJECT_ID('FK_EntityAssetHierarchy_AssetParentId' , 'F') IS NOT NULL )
    BEGIN
		ALTER TABLE [Meta].[EntityAssetHierarchy] DROP CONSTRAINT [FK_EntityAssetHierarchy_AssetParentId]
    END;

DROP TABLE IF EXISTS [Meta].[EntityAssetHierarchy]
DROP TABLE IF EXISTS [Meta].[EntityAssetHierarchyHistory]
GO
IF OBJECT_ID(N'[Meta].[EntityServiceHierarchy]' , N'U') IS NOT NULL
    BEGIN
		ALTER TABLE [Meta].[EntityServiceHierarchy] SET ( SYSTEM_VERSIONING = OFF )
        ALTER TABLE [Meta].[EntityServiceHierarchy] DROP PERIOD FOR SYSTEM_TIME;
    END;

IF ( OBJECT_ID('FK_EntityServiceHierarchy_EntityId' , 'F') IS NOT NULL )
    BEGIN
        ALTER TABLE Meta.EntityServiceHierarchy DROP CONSTRAINT FK_EntityServiceHierarchy_EntityId;
    END;

IF ( OBJECT_ID('FK_EntityServiceHierarchy_ServiceParentId' , 'F') IS NOT NULL )
    BEGIN
		ALTER TABLE [Meta].[EntityServiceHierarchy] DROP CONSTRAINT [FK_EntityServiceHierarchy_ServiceParentId]
    END;

DROP TABLE IF EXISTS [Meta].[EntityServiceHierarchy]
DROP TABLE IF EXISTS [Meta].[EntityServiceHierarchyHistory]

GO
IF OBJECT_ID(N'[Meta].[Entity]' , N'U') IS NOT NULL
    BEGIN
        ALTER TABLE Meta.Entity SET(SYSTEM_VERSIONING = OFF);
        ALTER TABLE Meta.Entity DROP PERIOD FOR SYSTEM_TIME;
    END;

IF ( OBJECT_ID('FK_AssetParentId' , 'C') IS NOT NULL )
    BEGIN
        ALTER TABLE Meta.Entity DROP CONSTRAINT FK_AssetParentId;
        ALTER TABLE Meta.Entity DROP CONSTRAINT FK_ServiceParentId;
    END;

IF ( OBJECT_ID('FK_EntityType' , 'F') IS NOT NULL )
    BEGIN
        ALTER TABLE Meta.Entity DROP CONSTRAINT FK_EntityType;
    END;

DROP TABLE IF EXISTS Meta.Entity;

DROP TABLE IF EXISTS Meta.EntityHistory;
GO

DROP FUNCTION IF EXISTS Meta.CheckNullableAssestParentIdForeignKey;

DROP FUNCTION IF EXISTS Meta.CheckNullableServiceParentIdForeignKey;

DROP FUNCTION IF EXISTS Meta.CheckIsAllowedAsDescendantAssetNode;

DROP FUNCTION IF EXISTS Meta.CheckIsAllowedAsDescendantServiceNode;
GO

--EntityType


DROP TABLE IF EXISTS Meta.EntityType;
GO

--Drop scripts should come above this line


DROP SCHEMA IF EXISTS Meta;
GO

------------------------------
--Create Calls
------------------------------


CREATE SCHEMA Meta;
GO

--New Tables here
--EntityType


CREATE TABLE Meta.EntityType (
             EntityTypeId                INT IDENTITY(1 , 1) ,
             Prefix                      VARCHAR(2) NOT NULL ,
             Name                        VARCHAR(50) NOT NULL ,
             IsAllowedAsAssetNode        BIT ,
             IsAllowedAsServiceNode      BIT ,
             IsAllowedSameDescendantNode BIT ,
             MaxEntityId                 INT NOT NULL CONSTRAINT DF_MaxEntityId DEFAULT 0 ,
             CONSTRAINT PK_EntityType PRIMARY KEY CLUSTERED(EntityTypeId) ,
             CONSTRAINT UQ_EntityTypePrefix UNIQUE(Prefix) ,
             CONSTRAINT UQ_EntityTypeName UNIQUE(Name)
                             );
GO
--Entity
--these functions are only to enforce FK relation on the hierarchy id's since normal fk's wont work on nullable non unique columns


--CREATE FUNCTION Meta.CheckNullableAssestParentIdForeignKey (
--                @AssetParentId HIERARCHYID = NULL
--                                                           )
--RETURNS BIT
--AS
--     BEGIN
--         IF @AssetParentId IS NULL
--             BEGIN
--                 RETURN 1
--             END;
--         DECLARE @C BIGINT;
--         SELECT @C = CASE
--                         WHEN COUNT(EntityId) = 1
--                         THEN 1
--                         ELSE 0
--                     END
--         FROM Meta.Entity
--         WHERE AssetNodeId = @AssetParentId
--               AND
--               AssetNodeId IS NOT NULL
--               AND
--               @AssetParentId IS NOT NULL;
--         RETURN @C;
--     END;
--GO

--CREATE FUNCTION Meta.CheckNullableServiceParentIdForeignKey (
--                @ServiceParentId HIERARCHYID
--                                                            )
--RETURNS BIT
--AS
--     BEGIN
--         IF @ServiceParentId IS NULL
--             BEGIN
--                 RETURN 1
--             END;
--         DECLARE @C BIGINT;
--         SELECT @C = CASE
--                         WHEN COUNT(EntityId) = 1
--                         THEN 1
--                         ELSE 0
--                     END
--         FROM Meta.Entity
--         WHERE ServiceNodeId = @ServiceParentId
--               AND
--               ServiceNodeId IS NOT NULL
--               AND
--               @ServiceParentId IS NOT NULL;
--         RETURN @C;
--     END;
--GO

-----
-- Entity Table
-----

CREATE TABLE Meta.Entity (
             EntityId      INT IDENTITY(1 , 1) ,
             EntityTypeId  INT NOT NULL ,
             Code          VARCHAR(10) NOT NULL ,
             Name          VARCHAR(255) NOT NULL , -- max not good for performance
             UserId        VARCHAR(255) NULL ,
             RowVersion    ROWVERSION ,
             ValidFrom     DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL ,
             ValidTo       DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL ,
             CONSTRAINT FK_EntityType FOREIGN KEY(EntityTypeId) REFERENCES Meta.EntityType(EntityTypeId) ,
             CONSTRAINT PK_Entity PRIMARY KEY CLUSTERED(EntityId) ,
             CONSTRAINT UQ_Code UNIQUE(Code) ,
             PERIOD FOR SYSTEM_TIME(ValidFrom , ValidTo)
                         ) WITH(SYSTEM_VERSIONING = ON(HISTORY_TABLE = Meta.EntityHistory));

GO
             --AssetTree AS REPLICATE(' | ' , AssetNodeId.GetLevel())+Code ,
             --ServiceTree AS REPLICATE(' | ' , ServiceNodeId.GetLevel())+Code ,
             --ServiceNodeLevel AS ServiceNodeId.GetLevel() PERSISTED ,
             --ServiceParentId AS ServiceNodeId.GetAncestor ( 1
             --                                             ) PERSISTED ,

CREATE TABLE Meta.EntityAssetHierarchy (
             EntityId      INT ,
             AssetNodeId   HIERARCHYID NOT NULL ,
             AssetNodeLevel AS AssetNodeId.GetLevel() PERSISTED ,
             AssetParentId AS AssetNodeId.GetAncestor ( 1
                                                      ) PERSISTED ,
             UserId        VARCHAR(255) NULL ,
             RowVersion    ROWVERSION ,
             ValidFrom     DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL ,
             ValidTo       DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL ,
             CONSTRAINT PK_EntityAssetHierarchy_EntityId PRIMARY KEY CLUSTERED(EntityId) ,
             CONSTRAINT UQ_AssetNodeId UNIQUE(AssetNodeId) ,
             CONSTRAINT FK_EntityAssetHierarchy_EntityId FOREIGN KEY(EntityId) REFERENCES Meta.Entity(EntityId) ,
             CONSTRAINT FK_EntityAssetHierarchy_AssetParentId FOREIGN KEY(AssetParentId) REFERENCES Meta.EntityAssetHierarchy(AssetNodeId), 
             PERIOD FOR SYSTEM_TIME(ValidFrom , ValidTo)
                         ) WITH(SYSTEM_VERSIONING = ON(HISTORY_TABLE = Meta.EntityAssetHierarchyHistory));

CREATE NONCLUSTERED INDEX IX_BreadthFirst_AssetNodeId ON Meta.EntityAssetHierarchy ( AssetNodeLevel , AssetNodeId )
GO
CREATE TABLE Meta.EntityServiceHierarchy (
             EntityId      INT ,
             ServiceNodeId   HIERARCHYID NOT NULL ,
             ServiceNodeLevel AS ServiceNodeId.GetLevel() PERSISTED ,
             ServiceParentId AS ServiceNodeId.GetAncestor ( 1
                                                      ) PERSISTED ,
             UserId        VARCHAR(255) NULL ,
             RowVersion    ROWVERSION ,
             ValidFrom     DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL ,
             ValidTo       DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL ,
             CONSTRAINT PK_EntityServiceHierarchy_EntityId PRIMARY KEY CLUSTERED(EntityId) ,
             CONSTRAINT UQ_ServiceNodeId UNIQUE(ServiceNodeId) ,
             CONSTRAINT FK_EntityServiceHierarchy_EntityId FOREIGN KEY(EntityId) REFERENCES Meta.Entity(EntityId) ,
             CONSTRAINT FK_EntityServiceHierarchy_ServiceParentId FOREIGN KEY(ServiceParentId) REFERENCES Meta.EntityServiceHierarchy(ServiceNodeId), 
             PERIOD FOR SYSTEM_TIME(ValidFrom , ValidTo)
                         ) WITH(SYSTEM_VERSIONING = ON(HISTORY_TABLE = Meta.EntityServiceHierarchyHistory));

CREATE NONCLUSTERED INDEX IX_BreadthFirst_ServiceNodeId ON Meta.EntityServiceHierarchy ( ServiceNodeLevel , ServiceNodeId )
GO
CREATE VIEW Meta.vwEntity
AS
SELECT  REPLICATE(' | ' , EAH.AssetNodeLevel)+ E.Code AS AssetTree 
        , REPLICATE(' | ' , ESH.ServiceNodeLevel)+ E.Code AS  ServiceTree 
		,E.EntityId
		,E.Code
		,E.EntityTypeId
		,E.Name
		,E.ValidFrom
		,E.ValidTo
		,EAH.AssetNodeId
		,EAH.AssetNodeLevel
		,EAH.AssetParentId
		--,EAH.ValidFrom AS AssetValidFrom
		--,EAH.ValidTo AS AssetValidTo
		,ESH.ServiceNodeId
		,ESH.ServiceNodeLevel
		,ESH.ServiceParentId
		--,ESH.ValidFrom AS ServiceValidFrom
		--,ESH.ValidTo AS ServiceValidTo
		,E.UserId AS EntityUserID
		,EAH.UserId AS AssetUserId
		,ESH.UserId AS ServiceUserId
		 FROM Meta.Entity E
  LEFT JOIN Meta.EntityAssetHierarchy EAH ON E.EntityId = EAH.EntityId
  LEFT JOIN Meta.EntityServiceHierarchy ESH ON E.EntityId = ESH.EntityId

GO


CREATE TRIGGER Meta.Add_User_Info_AfterInsertTrigger_On_Entity ON Meta.Entity
AFTER INSERT , UPDATE
AS
     BEGIN
         SET NOCOUNT ON;
         IF ( ( SELECT TRIGGER_NESTLEVEL() ) > 1 )
             BEGIN
                 RETURN
             END;
         DECLARE @SecurityRuleViolationError INT= 70000;
         DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'UserId(U) not found');
         DECLARE @UserId VARCHAR(50); -- We intenationally limit to 50.
         SELECT @UserId = Value
         FROM [Security].[vwContextInfo]
         WHERE [key] = 'U';
         IF @UserId IS NULL
             BEGIN 
		   ;THROW @SecurityRuleViolationError , @msg , 1;
             END
         ELSE
             BEGIN
				UPDATE E
					SET UserId = @UserId
				FROM Meta.Entity E INNER JOIN inserted I ON E.EntityId = I.EntityId
				WHERE E.UserId <> @UserId OR (E.UserId IS NULL AND @UserId IS NOT NULL);
             END;
     END;
GO

CREATE TRIGGER Meta.Add_User_Info_AfterInsertTrigger_On_EntityAssetHierarchy ON Meta.EntityAssetHierarchy
AFTER INSERT , UPDATE
AS
     BEGIN
         SET NOCOUNT ON;
         IF ( ( SELECT TRIGGER_NESTLEVEL() ) > 1 )
             BEGIN
                 RETURN
             END;
         DECLARE @SecurityRuleViolationError INT= 70000;
         DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'UserId(U) not found');
         DECLARE @UserId VARCHAR(50); -- We intenationally limit to 50.
         SELECT @UserId = Value
         FROM [Security].[vwContextInfo]
         WHERE [key] = 'U';
         IF @UserId IS NULL
             BEGIN 
		   ;THROW @SecurityRuleViolationError , @msg , 1;
             END
         ELSE
             BEGIN
                 UPDATE E
                        SET UserId = @UserId
                 FROM Meta.EntityAssetHierarchy E INNER JOIN inserted I ON E.EntityId = I.EntityId
				 WHERE E.UserId <> @UserId OR (E.UserId IS NULL AND @UserId IS NOT NULL);
             END;
     END;
GO

CREATE TRIGGER Meta.Add_User_Info_AfterInsertTrigger_On_EntityServiceHierarchy ON Meta.EntityServiceHierarchy
AFTER INSERT , UPDATE
AS
     BEGIN
         SET NOCOUNT ON;
         IF ( ( SELECT TRIGGER_NESTLEVEL() ) > 1 )
             BEGIN
                 RETURN
             END;
         DECLARE @SecurityRuleViolationError INT= 70000;
         DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'UserId(U) not found');
         DECLARE @UserId VARCHAR(50); -- We intenationally limit to 50.
         SELECT @UserId = Value
         FROM [Security].[vwContextInfo]
         WHERE [key] = 'U';
         IF @UserId IS NULL
             BEGIN 
		   ;THROW @SecurityRuleViolationError , @msg , 1;
             END
         ELSE
             BEGIN
                 UPDATE E
                        SET UserId = @UserId
                 FROM Meta.EntityServiceHierarchy E INNER JOIN inserted I ON E.EntityId = I.EntityId
				 WHERE E.UserId <> @UserId OR (E.UserId IS NULL AND @UserId IS NOT NULL);
             END;
     END;
GO

CREATE FUNCTION Meta.CheckIsAllowedAsDescendantAssetNode (
                @TypeId       INT ,
                @ParentNodeId HIERARCHYID
                                                         )
RETURNS BIT
AS
     BEGIN
         DECLARE @RC BIT= 0;
         DECLARE @IsAllowedAsAssetNode BIT;
         DECLARE @IsAllowedSameDescendantNode BIT;
         --Get the rules for entity type
         SELECT @IsAllowedAsAssetNode = IsAllowedAsAssetNode , @IsAllowedSameDescendantNode = IsAllowedSameDescendantNode
         FROM Meta.EntityType
         WHERE EntityTypeId = @TypeId;
         IF ( @IsAllowedAsAssetNode = 0 )
             BEGIN
                 SET @RC = 0;
             END
         ELSE
             BEGIN
                 DECLARE @AncestorTypeTable TABLE (
                                                  EntityTypeId                INT ,
                                                  IsAllowedAsNode             BIT ,
                                                  IsAllowedSameDescendantNode BIT
                                                  );--, Level INT)
                 INSERT INTO @AncestorTypeTable
                        SELECT E.EntityTypeId , ET.IsAllowedAsAssetNode AS IsAllowedAsNode , ET.IsAllowedSameDescendantNode--, E.AssetNodeLevel 
                        FROM Meta.Entity AS E 
								INNER JOIN Meta.EntityType AS ET ON E.EntityTypeId = ET.EntityTypeId
								LEFT JOIN Meta.EntityAssetHierarchy EAH ON E.EntityId = EAH.EntityId
                        WHERE @ParentNodeId.IsDescendantOf ( AssetNodeId
                                                           ) = 1
                              AND
                              E.EntityTypeId <> 1;
                 --ORDER BY E.AssetNodeLevel;

                 IF NOT EXISTS ( SELECT *
                                 FROM @AncestorTypeTable
                                 WHERE IsAllowedAsNode = 0
                               )
                     BEGIN
                         -- all ancestors have allowed in asset tree true
                         SET @RC = 1;
                         -- now check if new type is allowed as descendant 
                         IF ( @IsAllowedSameDescendantNode = 0
                              AND
                              EXISTS ( SELECT *
                                       FROM @AncestorTypeTable
                                       WHERE EntityTypeId = @TypeId
                                     ) )
                             BEGIN
                                 SET @RC = 0
                             END;
                     END;
             END;
         RETURN @RC;
     END;
GO

CREATE FUNCTION Meta.CheckIsAllowedAsDescendantServiceNode (
                @TypeId       INT ,
                @ParentNodeId HIERARCHYID
                                                           )
RETURNS BIT
AS
     BEGIN
         DECLARE @RC BIT= 0;
         DECLARE @IsAllowedAsServiceNode BIT;
         DECLARE @IsAllowedSameDescendantNode BIT;
         --Get the rules for entity type
         SELECT @IsAllowedAsServiceNode = IsAllowedAsServiceNode , @IsAllowedSameDescendantNode = IsAllowedSameDescendantNode
         FROM Meta.EntityType
         WHERE EntityTypeId = @TypeId;
         IF ( @IsAllowedAsServiceNode = 0 )
             BEGIN
                 SET @RC = 0;
             END
         ELSE
             BEGIN
                 DECLARE @AncestorTypeTable TABLE (
                                                  EntityTypeId                INT ,
                                                  IsAllowedAsNode             BIT ,
                                                  IsAllowedSameDescendantNode BIT
                                                  );--, Level INT)
                 INSERT INTO @AncestorTypeTable
                        SELECT E.EntityTypeId , ET.IsAllowedAsServiceNode AS IsAllowedAsNode , ET.IsAllowedSameDescendantNode--, E.ServiceNodeLevel 
                        FROM Meta.Entity AS E 
								INNER JOIN Meta.EntityType AS ET ON E.EntityTypeId = ET.EntityTypeId
								INNER JOIN Meta.EntityServiceHierarchy ESH ON E.EntityId = ESH.EntityId
                        WHERE @ParentNodeId.IsDescendantOf ( ServiceNodeId
                                                           ) = 1
                              AND
                              E.EntityTypeId <> 1;
                 --ORDER BY E.AssetNodeLevel;

                 IF NOT EXISTS ( SELECT *
                                 FROM @AncestorTypeTable
                                 WHERE IsAllowedAsNode = 0
                               )
                     BEGIN
                         -- all ancestors have allowed in asset tree true
                         SET @RC = 1;
                         -- now check if new type is allowed as descendant 
                         IF ( @IsAllowedSameDescendantNode = 0
                              AND
                              EXISTS ( SELECT *
                                       FROM @AncestorTypeTable
                                       WHERE EntityTypeId = @TypeId
                                     ) )
                             BEGIN
                                 SET @RC = 0
                             END;
                     END;
             END;
         RETURN @RC;
     END;
GO

CREATE PROCEDURE Meta.uspNewEntityCode
       @etypeid INT ,
       @Code    VARCHAR(10) OUTPUT
AS
     BEGIN
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         BEGIN TRY
             BEGIN TRANSACTION;
             DECLARE @ret VARCHAR(10);
             UPDATE Meta.EntityType WITH(UPDLOCK , ROWLOCK)
                    SET MaxEntityId = MaxEntityId + 1
             WHERE EntityTypeId = @etypeid;
             SELECT @ret = Prefix + CONVERT([VARCHAR](8) , LTRIM(MaxEntityId))
             FROM Meta.EntityType
             WHERE EntityTypeId = @etypeid;
             --Padded Code with 0's
             --SELECT @ret = Prefix + CONVERT([varchar](8),CASE WHEN 8-LEN(ltrim(MaxEntityId)) < 0 THEN '' ELSE replicate('0',(8-LEN(ltrim(MaxEntityId)))) END+ltrim(MaxEntityId),(0)) FROM Meta.EntityType 	WHERE EntityTypeId = @etypeid;
             SET @Code = @ret;
             COMMIT TRANSACTION; -- Transaction Success!
         END TRY
         BEGIN CATCH
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN;
     END;
GO

CREATE PROCEDURE Meta.uspCreateEntity
       @TypePrefix        VARCHAR(2) ,
       @Name              VARCHAR(255) ,
       @AssetParentCode   VARCHAR(10)  = NULL ,
       @ServiceParentCode VARCHAR(10)  = NULL ,
       @Code              VARCHAR(10)  = NULL OUTPUT ,
       @TypeName		 VARCHAR(255) = NULL OUTPUT ,
       @EntityId          INT          = NULL OUTPUT ,
       @AssetNodeId       HIERARCHYID  = NULL OUTPUT ,
       @ServiceNodeId     HIERARCHYID  = NULL OUTPUT
AS
     BEGIN
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         DECLARE @BizRuleViolationError INT= 60000;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'Entity with name (%s) and AssetParentCode (%s), ServiceParentCode (%s) cannot be created.' , @Name , @AssetParentCode , @ServiceParentCode);
             DECLARE @NewChildAssetNodeId HIERARCHYID = NULL;
             DECLARE @NewChildServiceNodeId HIERARCHYID = NULL;
             DECLARE @ParentAssetNodeId HIERARCHYID;
             DECLARE @ParentServiceNodeId HIERARCHYID;
             DECLARE @LASTCHILD HIERARCHYID;
             DECLARE @NeedToGenNewAssetNodeId BIT= CASE
                                                       WHEN @AssetParentCode IS NOT NULL
                                                            AND
                                                            RTRIM(LTRIM(@AssetParentCode)) <> ''
                                                       THEN 1
                                                       ELSE 0
                                                   END;
             DECLARE @NeedToGenNewServiceNodeId BIT= CASE
                                                         WHEN @ServiceParentCode IS NOT NULL
                                                              AND
                                                              RTRIM(LTRIM(@ServiceParentCode)) <> ''
                                                         THEN 1
                                                         ELSE 0
                                                     END;
             DECLARE @TypeId INT;
             BEGIN TRANSACTION;
             SELECT TOP 1 @TypeId = EntityTypeId,@TypeName = Name
             FROM Meta.EntityType
             WHERE Prefix = @TypePrefix;

             --if first node in the table, and of type 1, then ok create the root
             IF ( @TypeId = 1
                  AND
                  @NeedToGenNewAssetNodeId = 0
                  AND
                  @NeedToGenNewServiceNodeId = 0
                  AND
                  NOT EXISTS ( SELECT TOP 1 EntityId
                               FROM Meta.Entity
                             ) )
                 BEGIN
                     --trying to insert a root element so only allow if there are no entities in the table
                     SET @NewChildAssetNodeId = '/';
                     SET @NewChildServiceNodeId = '/';
                 END;
             ELSE
                 BEGIN
                     IF ( @TypeId IS NULL
                          OR
                          @TypeId = 1 ) --else dont allow
                         BEGIN 
					   ;THROW @BizRuleViolationError , @msg , 1;
                         END
                     ELSE
                         BEGIN
                             IF ( @NeedToGenNewAssetNodeId = 1
                                  OR
                                  @NeedToGenNewServiceNodeId = 1 )-- if not a type 1
                                 BEGIN
                                     IF @NeedToGenNewAssetNodeId = 1
                                        AND
                                        @NeedToGenNewServiceNodeId = 1
                                        AND
                                        @ServiceParentCode = @AssetParentCode
                                        AND
                                        @AssetParentCode IS NOT NULL
                                         BEGIN
                                             --trying to create a top level entity, as both an asset and service node.
                                             ;THROW @BizRuleViolationError , @msg , 1;
                                         END;
                                     IF @NeedToGenNewAssetNodeId = 1
                                         BEGIN 
                                             --Generate  AssetNodeId for new node, my getting the id of the parent, and making this the lastchild
                                             --since there is no unique key hold the lock on the parent to prevent other concurrent reads from taking place 
                                             --until this transaction completes. (it still does not prevent manual inserts, or read uncommited transactions :-( )
                                             SELECT TOP 1 @ParentAssetNodeId = AssetNodeId
                                             FROM Meta.EntityAssetHierarchy EAH WITH (UPDLOCK , ROWLOCK)
											 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = EAH.EntityId
                                             WHERE Code = @AssetParentCode;
                                             IF @ParentAssetNodeId IS NULL
                                                 BEGIN 
										  ;THROW @BizRuleViolationError , @msg , 1;
                                                 END	
                                             --Find the lastchild of a parent node, and then add this node as child greater than the previous last child.
                                             --if unique constraint is not there , this will cause duplicates in concurrent scenarios.

                                             IF ( Meta.CheckIsAllowedAsDescendantAssetNode ( @TypeId , @ParentAssetNodeId
                                                                                           ) = 0 )-- not allowed in particular hierarchy
                                                 BEGIN 
									    ;THROW @BizRuleViolationError , @msg , 1;
                                                 END;
                                             SELECT @LASTCHILD = MAX(AssetNodeId)
                                             FROM Meta.EntityAssetHierarchy
                                             WHERE AssetParentId = @ParentAssetNodeId;
                                             SELECT @NewChildAssetNodeId = @ParentAssetNodeId.GetDescendant ( @LASTCHILD , NULL
                                                                                                            );
                                         END;
                                     IF @NeedToGenNewServiceNodeId = 1
                                         BEGIN 
                                             -- same principle as done in asset for service nodes
                                             SELECT TOP 1 @ParentServiceNodeId = ServiceNodeId
                                             FROM Meta.EntityServiceHierarchy ESH WITH (UPDLOCK , ROWLOCK)
											 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = ESH.EntityId
                                             WHERE Code = @ServiceParentCode;
                                             IF @ParentServiceNodeId IS NULL
                                                 BEGIN 
									    ;THROW @BizRuleViolationError , @msg , 1;
                                                 END	
                                             --Find the lastchild of a parent node, and then add this node as child greater than the previous last child.
                                             --if unique constraint is not there , this will cause duplicates in concurrent scenarios.
                                             IF ( Meta.CheckIsAllowedAsDescendantServiceNode ( @TypeId , @ParentServiceNodeId
                                                                                             ) = 0 )-- not allowed in particular hierarchy
                                                 BEGIN 
									    ;THROW @BizRuleViolationError , @msg , 1;
                                                 END;	
                                             --Find the lastchild of a parent node, and then add this node as child greater than the previous last child.
                                             --if unique constraint is not there , this will cause duplicates in concurrent scenarios.
                                             SELECT @LASTCHILD = MAX(ServiceNodeId)
                                             FROM Meta.EntityServiceHierarchy
                                             WHERE ServiceParentId = @ParentServiceNodeId;
                                             SELECT @NewChildServiceNodeId = @ParentServiceNodeId.GetDescendant ( @LASTCHILD , NULL
                                                                                                                );
                                         END;
                                 END;
                             ELSE
                                 BEGIN 
						   ;THROW @BizRuleViolationError , @msg , 1;
                                 END;
                         END;
                 END;

             --Generate a new code for this entity, and insert it into the table
             EXECUTE Meta.uspNewEntityCode @TypeId , @Code = @Code OUTPUT;
             INSERT INTO Meta.Entity ( EntityTypeId , Code , Name ) VALUES ( @TypeId , @Code , @Name );
             -- get the newly created id's and code to return back to the caller as output params	
             SELECT @EntityId = SCOPE_IDENTITY() , @AssetNodeId = @NewChildAssetNodeId , @ServiceNodeId = @NewChildServiceNodeId;

			 IF(@NewChildAssetNodeId IS NOT NULL)
				INSERT INTO Meta.EntityAssetHierarchy (EntityId, AssetNodeId) VALUES (@EntityId, @NewChildAssetNodeId);
			 IF(@NewChildServiceNodeId IS NOT NULL)
				INSERT INTO Meta.EntityServiceHierarchy (EntityId, ServiceNodeId) VALUES (@EntityId, @NewChildServiceNodeId);
             --WAITFOR DELAY '00:00:05'
             COMMIT TRANSACTION; -- Transaction Success!
             SET NOCOUNT OFF;
         END TRY
         BEGIN CATCH
             SET NOCOUNT OFF;
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN 1; -- send back a return code of 1 if all ok, otherwise 0 or error will will be thrown
     END;
GO

DROP PROCEDURE IF EXISTS Meta.uspReparentAssetNode;
GO

CREATE PROCEDURE Meta.uspReparentAssetNode
       @EntityId         INT ,
       @AssetNodeId      HIERARCHYID ,
       @NewParentAssetId HIERARCHYID
AS
     BEGIN
         SET NOCOUNT ON;
         DECLARE @NewAssetId HIERARCHYID;
         DECLARE @BizRuleViolationError INT= 60001;
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         BEGIN TRY
             DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'Could not generate new NodeId for reparenting.');
             BEGIN TRANSACTION;
             -- First, get a new hid for Asset that moves
             SET @NewAssetId = @NewParentAssetId.GetDescendant ( ( SELECT MAX(AssetNodeId)
                                                                   FROM Meta.EntityAssetHierarchy WITH (UPDLOCK , ROWLOCK)
                                                                   WHERE AssetParentId = @NewParentAssetId
                                                                 ) , NULL
                                                               );
             IF @NewAssetId IS NULL
                 BEGIN 
					;THROW @BizRuleViolationError , @msg , 1;
                 END	

             -- Next, reparent all descendants of Asset that moves

             IF @AssetNodeId IS NOT NULL
                 BEGIN
                     UPDATE Meta.EntityAssetHierarchy
                            SET AssetNodeId = AssetNodeId.GetReparentedValue ( @AssetNodeId , @NewAssetId
                                                                             )
                     WHERE AssetNodeId.IsDescendantOf ( @AssetNodeId
                                                      ) = 1;
                 END;
             ELSE
                 BEGIN
							MERGE Meta.EntityAssetHierarchy ESH
							USING Meta.Entity E
							ON ESH.EntityId = E.EntityId 
							WHEN MATCHED AND @NewAssetId IS NULL AND E.EntityId = @EntityId THEN
							  DELETE
							WHEN MATCHED AND E.EntityId = @EntityId THEN
							  UPDATE SET AssetNodeId = @NewAssetId
							WHEN NOT MATCHED BY TARGET AND E.EntityId = @EntityId THEN
							  INSERT (EntityId, AssetNodeId) VALUES (E.EntityId, @NewAssetId);
                 END;
             COMMIT TRANSACTION;
             SET NOCOUNT OFF;
         END TRY
         BEGIN CATCH
             SET NOCOUNT OFF;
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN 1; -- send back a return code of 1 if all ok, otherwise 0 or error will will be thrown
     END;
GO

DROP PROCEDURE IF EXISTS Meta.uspReparentServiceNode;
GO

CREATE PROCEDURE Meta.uspReparentServiceNode
       @EntityId           INT ,
       @ServiceNodeId      HIERARCHYID ,
       @NewParentServiceId HIERARCHYID
AS
     BEGIN
         DECLARE @NewServiceId HIERARCHYID;
         DECLARE @BizRuleViolationError INT= 60001;
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         BEGIN TRY
             DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'Could not generate new NodeId for reparenting.');
             BEGIN TRANSACTION;
             -- First, get a new hid for Asset that moves
			IF @NewParentServiceId IS NOT NULL
			 BEGIN
				 SET @NewServiceId = @NewParentServiceId.GetDescendant ( ( SELECT MAX(ServiceNodeId)
																		   FROM Meta.EntityServiceHierarchy WITH (UPDLOCK , ROWLOCK)
																		   WHERE ServiceParentId = @NewParentServiceId
																		 ) , NULL
																	   );
				 IF @NewServiceId IS NULL
					 BEGIN 
						;THROW @BizRuleViolationError , @msg , 1;
					 END	
			END
			ELSE -- unparenting a node, back to null (ie remove load from bucket and back to null)
				SET @NewServiceId = NULL
             -- Next, reparent all descendants of Service that moves
             IF @ServiceNodeId IS NOT NULL AND @NewServiceId IS NOT NULL
                 BEGIN
                     UPDATE Meta.EntityServiceHierarchy
                            SET ServiceNodeId = ServiceNodeId.GetReparentedValue ( @ServiceNodeId , @NewServiceId
                                                                                 )
                     WHERE ServiceNodeId.IsDescendantOf ( @ServiceNodeId
                                                        ) = 1;
                 END;
             ELSE
                 BEGIN
							MERGE Meta.EntityServiceHierarchy ESH
							USING Meta.Entity E
							ON ESH.EntityId = E.EntityId 
							WHEN MATCHED AND @NewServiceId IS NULL AND E.EntityId = @EntityId THEN
							  DELETE
							WHEN MATCHED AND E.EntityId = @EntityId THEN
							  UPDATE SET ServiceNodeId = @NewServiceId
							WHEN NOT MATCHED BY TARGET AND E.EntityId = @EntityId THEN
							  INSERT (EntityId, ServiceNodeId) VALUES (E.EntityId, @NewServiceId);
                 END;
             COMMIT TRANSACTION;
         END TRY
         BEGIN CATCH
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN 1; -- send back a return code of 1 if all ok, otherwise 0 or error will will be thrown
     END;
GO

DROP PROCEDURE IF EXISTS Meta.uspUpdateEntity;
GO

CREATE PROCEDURE Meta.uspUpdateEntity
       @Code                 VARCHAR(10) ,
       @NewName              VARCHAR(255) = NULL ,
       @NewAssetParentCode   VARCHAR(10)  = NULL ,
       @NewServiceParentCode VARCHAR(10)  = NULL ,
       @NewAssetNodeId       HIERARCHYID  = NULL OUTPUT ,
       @NewServiceNodeId     HIERARCHYID  = NULL OUTPUT
AS
     BEGIN
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         DECLARE @BizRuleViolationError INT= 60001;
         BEGIN TRY
             DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'Entity with code (%s) and @NewName (%s), NewAssetParentCode (%s), NewServiceParentCode (%s) cannot be updated.' , @Code , ISNULL(@NewName , '') , ISNULL(@NewAssetParentCode , '') , ISNULL(@NewServiceParentCode , ''));
             DECLARE @OldName VARCHAR(255) , @OldAssetParentId HIERARCHYID , @OldServiceParentId HIERARCHYID , @OldAssetParentCode VARCHAR(10) , @OldServiceParentCode VARCHAR(10) , @OldAssetId HIERARCHYID= NULL , @OldServiceId HIERARCHYID , @EntityId INT;
             DECLARE @TypeId INT= NULL
             DECLARE @NewParentAssetNodeId HIERARCHYID;
             DECLARE @NewParentServiceNodeId HIERARCHYID;
             BEGIN TRANSACTION;
             --read the data in and then lock it 
             SELECT @EntityId = E.EntityId 
					, @OldAssetParentId = AssetParentId 
					, @OldServiceParentId = ServiceParentId 
					, @OldAssetId = AssetNodeId 
					, @OldServiceId = ServiceNodeId 
					, @OldName = Name 
					, @TypeId = EntityTypeId
             FROM Meta.Entity E WITH (UPDLOCK , ROWLOCK)
             LEFT JOIN Meta.EntityAssetHierarchy EAH WITH (UPDLOCK , ROWLOCK) ON E.EntityId = EAH.EntityId
             LEFT JOIN Meta.EntityServiceHierarchy ESH WITH (UPDLOCK , ROWLOCK) ON E.EntityId = ESH.EntityId
             WHERE Code = @Code;
             IF @EntityId IS NULL
                 BEGIN
                     COMMIT TRANSACTION;
                     RETURN 0; -- not found
                 END;
             SELECT @OldAssetParentCode = Code
             FROM Meta.EntityAssetHierarchy EAH WITH (UPDLOCK , ROWLOCK)
			 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = EAH.EntityId
             WHERE AssetNodeId = @OldAssetParentId;
             
			 SELECT @OldServiceParentCode = Code
             FROM Meta.EntityServiceHierarchy ESH WITH (UPDLOCK , ROWLOCK)
			 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = ESH.EntityId
             WHERE ServiceNodeId = @OldServiceParentId;

             --@OldAssetParentCode
             DECLARE @NeedToReparentAssetNodeId BIT= CASE
                                                         WHEN (@OldAssetParentCode <> @NewAssetParentCode  AND @NewAssetParentCode IS NOT NULL )
															  OR (@OldAssetParentCode IS NULL AND @NewAssetParentCode IS NOT NULL )
															  

                                                         THEN 1
                                                         ELSE 0
                                                     END;
             DECLARE @NeedToReparentServiceNodeId BIT= CASE
                                                           WHEN (@OldServiceParentCode <> @NewServiceParentCode AND @NewServiceParentCode IS NOT NULL)
																OR (@OldServiceParentCode IS NULL AND @NewServiceParentCode IS NOT NULL)
                                                           THEN 1
                                                           ELSE 0
                                                       END;
             DECLARE @NeedToUpdateName BIT= CASE
                                                WHEN ISNULL(@OldName , '') <> ISNULL(@NewName , '')
                                                     AND
                                                     @NewName IS NOT NULL
                                                THEN 1
                                                ELSE 0
                                            END;

             --trying to update  root element so only allow if its a name change
             IF ( ( ( @OldAssetId = '/'
                      AND
                      @NeedToReparentAssetNodeId = 1 )
                    OR
                    ( @OldServiceId = '/'
                      AND
                      @NeedToReparentServiceNodeId = 1 ) ) )--AND @NeedToUpdateName = 0
                 BEGIN 
			  ;THROW @BizRuleViolationError , @msg , 1;
                 END
             ELSE
                 BEGIN
                     --Get the rules for entity type

                     IF @NeedToReparentAssetNodeId = 1
                        AND
                        @NeedToReparentServiceNodeId = 1
                        AND
                        @NewServiceParentCode = @NewAssetParentCode
                        AND
                        ( @NewAssetParentCode IS NOT NULL
                          OR
                          @NewServiceParentCode IS NOT NULL )
                         BEGIN
                             --trying to reparent both trees to the same entity .i.e. combine the trees
                             ;THROW @BizRuleViolationError , @msg , 1;
                         END
                     IF @NeedToReparentAssetNodeId = 1
                         BEGIN
                             SELECT @NewParentAssetNodeId = AssetNodeId 
							 FROM Meta.EntityAssetHierarchy EAH WITH (UPDLOCK , ROWLOCK)
							 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = EAH.EntityId
                             WHERE Code = @NewAssetParentCode;

                             --uspReparentAssetNode
                             --child, and new parent dont have matching flags then abort
                             IF ( Meta.CheckIsAllowedAsDescendantAssetNode ( @TypeId , @NewParentAssetNodeId
                                                                           ) = 0 )-- not allowed in particular hierarchy
                                 BEGIN 
									;THROW @BizRuleViolationError , @msg , 1;
                                 END;
                             EXEC Meta.uspReparentAssetNode @EntityId , @OldAssetId , @NewParentAssetNodeId;
                         END;
                     IF @NeedToReparentServiceNodeId = 1
                         BEGIN
                             SELECT @NewParentServiceNodeId = ServiceNodeId 
							 FROM Meta.EntityServiceHierarchy ESH WITH (UPDLOCK , ROWLOCK)
							 INNER JOIN Meta.Entity E WITH (UPDLOCK , ROWLOCK) ON E.EntityId = ESH.EntityId
                             WHERE Code = @NewServiceParentCode;

                             --uspReparentServiceNode
                             IF (@NewParentServiceNodeId IS NOT NULL AND  Meta.CheckIsAllowedAsDescendantServiceNode ( @TypeId , @NewParentServiceNodeId
                                                                             ) = 0 )-- not allowed in particular hierarchy
                                 BEGIN 
										;THROW @BizRuleViolationError , @msg , 1;
                                 END;
							
                             EXEC Meta.uspReparentServiceNode @EntityId , @OldServiceId , @NewParentServiceNodeId;
                         END;
                     IF @NeedToUpdateName = 1
                         BEGIN
                             UPDATE Meta.Entity
                                    SET Name = @NewName
                             WHERE EntityId = @EntityId;
                         END;
                 END;
             SELECT @NewAssetNodeId = AssetNodeId , @NewServiceNodeId = ServiceNodeId
             FROM Meta.Entity E
					LEFT JOIN Meta.EntityAssetHierarchy EAH ON E.EntityId = EAH.EntityId
					LEFT JOIN Meta.EntityServiceHierarchy ESH ON E.EntityId = ESH.EntityId
             WHERE E.EntityId = @EntityId;
             --WAITFOR DELAY '00:00:05'
             COMMIT TRANSACTION; -- Transaction Success!
         END TRY
         BEGIN CATCH
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN 1; -- send back a return code of 1 if all ok, otherwise 0 or error will will be thrown
     END;
GO

DROP PROCEDURE IF EXISTS Meta.uspDeleteEntity;
GO

CREATE PROCEDURE Meta.uspDeleteEntity
       @Code              VARCHAR(10) ,
       @DeleteDescendants BIT         = 0
AS
     BEGIN
         SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
         DECLARE @BizRuleViolationError INT= 60001;
         DECLARE @RC INT= 0;
         BEGIN TRY
             DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'Entity with code (%s) cannot be deleted.' , @Code);
             DECLARE @msg1 NVARCHAR(2048)= FORMATMESSAGE(N'Entity with code (%s) cannot be deleted as it has descendant nodes.' , @Code);
             BEGIN TRANSACTION;
             IF ( @Code IS NULL
                  OR
                  @Code = '' )
                 BEGIN 
			  ;THROW @BizRuleViolationError , @msg , 1;
                 END
             DECLARE @EntityId INT;
             DECLARE @AssetNodeId HIERARCHYID;
             DECLARE @ServiceNodeId HIERARCHYID;
             SELECT @EntityId = E.EntityId , @AssetNodeId = AssetNodeId , @ServiceNodeId = ServiceNodeId
             FROM Meta.Entity E WITH (XLOCK , ROWLOCK)
					LEFT JOIN Meta.EntityAssetHierarchy EAH WITH (XLOCK , ROWLOCK) ON E.EntityId = EAH.EntityId
					LEFT JOIN Meta.EntityServiceHierarchy ESH WITH (XLOCK , ROWLOCK) ON E.EntityId = ESH.EntityId
             WHERE Code = @Code;
             --cannot delete root
             IF @EntityId IS NOT NULL
                AND
                @EntityId <> 1
                 BEGIN
                     IF ( @DeleteDescendants = 1 )
                         BEGIN
							 --Delete Rules
							 --Set AssetNodeId = NULL and ServiceNodeId = NULL for all descendants in the asset tree, except for entity to be deleted 
							     
							 --Set ServiceNodeId = NULL for all descendants in the service tree, except for entity to be deleted
							 --Set AssetNodeId = NULL and ServiceNodeId = NULL for entity to be deleted
							 --delete all orphans (service and asset = null)
                             --Delete Multiple
							--From Asset Hierarchy
							 DELETE Meta.EntityServiceHierarchy
							 WHERE EntityId IN (
											SELECT EntityId FROM Meta.EntityAssetHierarchy WHERE AssetNodeId.IsDescendantOf ( @AssetNodeId ) = 1
																									AND @AssetNodeId IS NOT NULL
											    )

							 DELETE Meta.EntityAssetHierarchy
								WHERE AssetNodeId.IsDescendantOf ( @AssetNodeId ) = 1 AND @AssetNodeId IS NOT NULL

							 DELETE Meta.EntityServiceHierarchy
								WHERE ServiceNodeId.IsDescendantOf ( @ServiceNodeId ) = 1 AND @ServiceNodeId IS NOT NULL

							 -- find entityid's that dont exists in AssetHierarchy, and ServiceHierarchy and Delete them
								                            
                             --Delete the Tags for entities that will be deleted
                             DELETE Meta.Tag  WHERE EntityId IN (SELECT E.EntityId 
												FROM Meta.Entity E 
													LEFT JOIN Meta.EntityAssetHierarchy EAH ON E.EntityId = EAH.EntityId
													LEFT JOIN Meta.EntityServiceHierarchy ESH ON E.EntityId = ESH.EntityId
											 WHERE EAH.EntityId IS NULL AND ESH.EntityId IS NULL
                                               )
                             DELETE Meta.Entity WHERE EntityId IN (SELECT E2.EntityId	
												FROM Meta.Entity E2 
													LEFT JOIN Meta.EntityAssetHierarchy EAH ON E2.EntityId = EAH.EntityId
													LEFT JOIN Meta.EntityServiceHierarchy ESH ON E2.EntityId = ESH.EntityId
											 WHERE EAH.EntityId IS NULL AND ESH.EntityId IS NULL
                                               )
							 SET @RC = 1;
                         END;
                     ELSE
                         BEGIN
                             IF ( NOT EXISTS ( SELECT *
                                               FROM Meta.EntityAssetHierarchy
                                               WHERE AssetNodeId.IsDescendantOf ( @AssetNodeId
                                                                                ) = 1
                                                     AND
                                                     EntityId <> @EntityId
                                             )
                                  AND
                                  NOT EXISTS ( SELECT *
                                               FROM Meta.EntityServiceHierarchy
                                               WHERE ServiceNodeId.IsDescendantOf ( @ServiceNodeId
                                                                                  ) = 1
                                                     AND
                                                     EntityId <> @EntityId
                                             ) )
                                 BEGIN
                                     DELETE FROM Meta.Tag WHERE EntityId = @EntityId;
                                     DELETE FROM Meta.EntityAssetHierarchy WHERE EntityId = @EntityId;
                                     DELETE FROM Meta.EntityServiceHierarchy WHERE EntityId = @EntityId;
                                     DELETE FROM Meta.Entity WHERE EntityId = @EntityId;
                                     SET @RC = 1;
                                 END;
                             ELSE
                                 BEGIN 
						   ;THROW @BizRuleViolationError , @msg1 , 1;
                                 END
                         END;
                     COMMIT TRANSACTION; -- commit if success
                 END;
             ELSE
                 BEGIN
                     COMMIT TRANSACTION;
                     RETURN @RC; -- not found
                 END;
         END TRY
         BEGIN CATCH
             IF ( XACT_STATE() <> 0
                  OR
                  @@TRANCOUNT > 0 )
                 BEGIN
                     ROLLBACK TRANSACTION
                 END; -- Rollback if transaction exisits
             THROW;
         END CATCH;
         RETURN @RC; -- send back a return code of 1 if all ok, otherwise 0 or error will will be thrown
     END;
GO
--TagType


CREATE TABLE Meta.TagType (
             [Key] VARCHAR(64) NOT NULL ,
             CONSTRAINT PK_TagType PRIMARY KEY CLUSTERED([Key])
                          );
GO

CREATE TRIGGER LowercaseTags_Insteadof_Trigger_On_TagType ON Meta.TagType
FOR INSERT , UPDATE
AS
     SET NOCOUNT ON;
     UPDATE Meta.TagType
            SET Meta.TagType.[Key] = LOWER(i.[Key])
     FROM Meta.TagType INNER JOIN inserted i ON i.[Key] = Meta.TagType.[Key];
GO

CREATE TABLE Meta.Tag (
             TagId      INT IDENTITY(1 , 1) ,
             EntityId   INT NOT NULL ,
             [Key]      VARCHAR(64) NOT NULL ,
             [Value]    VARCHAR(1024) ,
             UserId     VARCHAR(255) NULL ,
             RowVersion ROWVERSION ,
             ValidFrom  DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL ,
             ValidTo    DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL ,
             PERIOD FOR SYSTEM_TIME(ValidFrom , ValidTo) ,
             CONSTRAINT PK_Tag PRIMARY KEY CLUSTERED(TagId ASC) ,
             CONSTRAINT FK_TagType FOREIGN KEY([Key]) REFERENCES Meta.TagType([Key]) ,
             CONSTRAINT FK_TagEntityId FOREIGN KEY(EntityId) REFERENCES Meta.Entity([EntityId])
                      ) WITH(SYSTEM_VERSIONING = ON(HISTORY_TABLE = Meta.TagHistory));

CREATE UNIQUE NONCLUSTERED INDEX UQ_EntityId_TagId ON Meta.Tag ( EntityId , [Key]
                                                               )
              INCLUDE ( [Value]
                      );

CREATE NONCLUSTERED INDEX IX_TagKey ON Meta.Tag ( [Key]
                                                );
GO

CREATE TRIGGER Meta.LowercaseTags_Insteadof_Trigger_On_Tag ON Meta.Tag
FOR INSERT , UPDATE
AS
	SET NOCOUNT ON;
     UPDATE Meta.Tag
            SET Meta.Tag.[Key] = LOWER(i.[Key]) , Meta.Tag.[Value] = LOWER(i.[Value])
     FROM Meta.Tag INNER JOIN inserted i ON i.EntityId = Meta.Tag.EntityId
                                            AND
                                            i.[Key] = Meta.Tag.[Key]
                                            AND
                                            i.[Value] = Meta.Tag.[Value];
GO

CREATE TRIGGER Meta.Add_User_Info_AfterInsertTrigger_On_Tag ON Meta.Tag
AFTER INSERT , UPDATE
AS
     BEGIN
         SET NOCOUNT ON;
         IF ( ( SELECT TRIGGER_NESTLEVEL() ) > 1 )
             BEGIN
                 RETURN
             END;
         DECLARE @SecurityRuleViolationError INT= 70000;
         DECLARE @msg NVARCHAR(2048)= FORMATMESSAGE(N'UserId(U) not found');
         DECLARE @UserId VARCHAR(50); -- We intenationally limit to 50.
         SELECT @UserId = Value
         FROM [Security].[vwContextInfo]
         WHERE [key] = 'U';
         IF @UserId IS NULL
             BEGIN 
		   ;THROW @SecurityRuleViolationError , @msg , 1;
             END
         ELSE
             BEGIN
                 UPDATE T
                        SET UserId = @UserId
                 FROM Meta.Tag T INNER JOIN inserted I ON T.TagId = I.TagId
				 WHERE T.UserId <> @UserId OR (T.UserId IS NULL AND @UserId IS NOT NULL);
             END;
     END;
GO
------------------------------
--Row Level Security
------------------------------
--Function Predicates for RLS

CREATE FUNCTION [Security].[fnSecurityPredicateSelectEntity](@entityId AS INT)
    RETURNS TABLE
WITH SCHEMABINDING
AS
    RETURN SELECT 1  fnSecurityPredicateSelectEntityResult FROM
                     (
                           SELECT EEA.EntityId FROM
                                                (
                                                       SELECT EA.AssetNodeId FROM Meta.Entity E 
                                                              INNER JOIN Meta.EntityAssetHierarchy EA ON EA.EntityId = E.EntityId 
                                                              INNER JOIN (SELECT [KEY],[VALUE],ROW_NUMBER() OVER (PARTITION BY [KEY] ORDER BY Rnk) KeyRank 
                                                                                    FROM (SELECT [KEY],[VALUE],1 AS Rnk FROM Security.vwContextInfo  
                                                                                                 UNION SELECT 'AR', 'OE1', 2 as Rnk UNION SELECT 'SR', 'OE1', 2 as Rnk) T
                                                                                         ) as SV on SV.[KEY]='AR' AND SV.VALUE = E.Code AND SV.KeyRank=1
                                                ) As AE
                                                INNER JOIN Meta.EntityAssetHierarchy EEA ON EEA.AssetNodeId.IsDescendantOf(AE.AssetNodeId) = 1
                                                UNION
                                                SELECT EES.EntityId FROM
                                                (
                                                       SELECT ES.ServiceNodeId FROM Meta.Entity E 
                                                              INNER JOIN Meta.EntityServiceHierarchy ES ON ES.EntityId = E.EntityId 
                                                              INNER JOIN (SELECT [KEY],[VALUE],ROW_NUMBER() OVER (PARTITION BY [KEY] ORDER BY Rnk) KeyRank 
                                                                                    FROM (SELECT [KEY],[VALUE],1 AS Rnk FROM Security.vwContextInfo  
                                                                                                 UNION SELECT 'AR', 'OE1', 2 as Rnk UNION SELECT 'SR', 'OE1', 2 as Rnk) T
                                                                                         ) SV on SV.[KEY]='SR' AND SV.VALUE = E.Code AND SV.KeyRank=1
                                                ) As SE
                                                INNER JOIN Meta.EntityServiceHierarchy EES ON EES.ServiceNodeId.IsDescendantOf(SE.ServiceNodeId) = 1
                     ) R WHERE R.EntityId=@entityId;
GO


CREATE FUNCTION [Security].[fnSecurityPredicateSelectEntityServiceHierarchy](@entityId AS INT)
    RETURNS TABLE
WITH SCHEMABINDING
AS
    RETURN SELECT 1  fnSecurityPredicateSelectEntityResult FROM
                     (          
						SELECT EES.EntityId FROM
						(
								SELECT ES.ServiceNodeId FROM Meta.Entity E 
										INNER JOIN Meta.EntityServiceHierarchy ES ON ES.EntityId = E.EntityId 
										INNER JOIN (SELECT [KEY],[VALUE],ROW_NUMBER() OVER (PARTITION BY [KEY] ORDER BY Rnk) KeyRank 
															FROM (SELECT [KEY],[VALUE],1 AS Rnk FROM Security.vwContextInfo  
																			UNION SELECT 'SR', 'OE1', 2 as Rnk) T
																	) SV on SV.[KEY]='SR' AND SV.VALUE = E.Code AND SV.KeyRank=1
						) As SE
						INNER JOIN Meta.EntityServiceHierarchy EES ON EES.ServiceNodeId.IsDescendantOf(SE.ServiceNodeId) = 1 
						WHERE EES.EntityId =@entityId
					) AS R;
                     
GO


CREATE FUNCTION [Security].[fnSecurityPredicateSelectEntityAssetHierarchy](@entityId AS INT)
    RETURNS TABLE
WITH SCHEMABINDING
AS
    RETURN SELECT 1  fnSecurityPredicateSelectEntityResult FROM
                     (          
						SELECT EES.EntityId FROM
						(
								SELECT ES.AssetNodeId FROM Meta.Entity E 
										INNER JOIN Meta.EntityAssetHierarchy ES ON ES.EntityId = E.EntityId 
										INNER JOIN (SELECT [KEY],[VALUE],ROW_NUMBER() OVER (PARTITION BY [KEY] ORDER BY Rnk) KeyRank 
															FROM (SELECT [KEY],[VALUE],1 AS Rnk FROM Security.vwContextInfo  
																			UNION SELECT 'AR', 'OE1', 2 as Rnk ) T
																	) SV on SV.[KEY]='AR' AND SV.VALUE = E.Code AND SV.KeyRank=1
						) As SE
						INNER JOIN Meta.EntityAssetHierarchy EES ON EES.AssetNodeId.IsDescendantOf(SE.AssetNodeId) = 1 
						WHERE EES.EntityId =@entityId
					) AS R;
                     
GO

--Apply Function Predicates for RLS to Entity Table
CREATE SECURITY POLICY Security.EntitySelectFilter
ADD FILTER PREDICATE Security.fnSecurityPredicateSelectEntity(EntityId) ON Meta.Entity
WITH (STATE = OFF); -- The state must be set to ON to enable the policy.
GO

CREATE SECURITY POLICY Security.EntityAssetHierarchySelectFilter
ADD FILTER PREDICATE Security.fnSecurityPredicateSelectEntityAssetHierarchy(EntityId) ON Meta.EntityAssetHierarchy
WITH (STATE = ON); -- The state must be set to ON to enable the policy.
GO

CREATE SECURITY POLICY Security.EntityServiceHierarchySelectFilter
ADD FILTER PREDICATE Security.fnSecurityPredicateSelectEntityServiceHierarchy(EntityId) ON Meta.EntityServiceHierarchy
WITH (STATE = ON); -- The state must be set to ON to enable the policy.
GO



------------------------------
--Seed Data
------------------------------
--Set up the context for the user we want to use


DELETE [Security].vwContextInfo;

INSERT INTO [Security].vwContextInfo ( [KEY] , [VALUE]
                                     )
VALUES ( 'U' , 'seed@openenergi.com'
       );

GO
--Entity Types


INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'OE' , 'Root' , 0 , 0 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'C' , 'Client' , 1 , 0 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'S' , 'Site' , 1 , 0 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'D' , 'Device' , 1 , 0 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'L' , 'Load' , 1 , 1 , 1
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'M' , 'Meter' , 1 , 0 , 1
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'G' , 'Grid' , 0 , 1 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'F' , 'Fleet' , 0 , 1 , 0
       );

INSERT INTO Meta.EntityType ( Prefix , Name , IsAllowedAsAssetNode , IsAllowedAsServiceNode , IsAllowedSameDescendantNode
                            )
VALUES ( 'B' , 'Bucket' , 0 , 1 , 0
       );
GO

-- add OE Root Entity Node


EXECUTE [Meta].[uspCreateEntity] 'OE' , 'OE';

------------------------------
--Dummy Test Data 
--for Integration Tests
------------------------------
--TagTypes


INSERT INTO Meta.TagType ( [Key]
                         )
VALUES ( 'Temperature'
       ) , ( 'Temp'
           );

INSERT INTO Meta.TagType ( [Key]
                         )
VALUES ( 'SetpointH'
       );

INSERT INTO Meta.TagType ( [Key]
                         )
VALUES ( 'SetpointL'
       );

INSERT INTO Meta.TagType ( [Key]
                         )
VALUES ( 'Dummy'
       );


--Entities


DECLARE @TypePrefix VARCHAR(2);

DECLARE @Name VARCHAR(255);

DECLARE @AssetParentCode VARCHAR(10);

DECLARE @ServiceParentCode VARCHAR(10);

SELECT @TypePrefix = 'C' , @Name = 'J Sainsbury' , @AssetParentCode = 'OE1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'C' , @Name = 'Tesco' , @AssetParentCode = 'OE1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'S' , @Name = 'Northcheam' , @AssetParentCode = 'C1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'D' , @Name = 'JACE' , @AssetParentCode = 'S1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'D' , @Name = 'JACE2' , @AssetParentCode = 'S1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'D' , @Name = 'JACE3' , @AssetParentCode = 'S1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'S' , @Name = 'Stratford' , @AssetParentCode = 'C1' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'S' , @Name = 'Tesco Express' , @AssetParentCode = 'C2' , @ServiceParentCode = NULL;

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'G' , @Name = 'National Grid' , @AssetParentCode = NULL , @ServiceParentCode = 'OE1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'F' , @Name = 'FFR' , @AssetParentCode = NULL , @ServiceParentCode = 'G1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'B' , @Name = 'Live' , @AssetParentCode = NULL , @ServiceParentCode = 'F1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'B' , @Name = 'Live2' , @AssetParentCode = NULL , @ServiceParentCode = 'F1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'B' , @Name = 'Live3' , @AssetParentCode = NULL , @ServiceParentCode = 'F1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'B' , @Name = 'NewLoads' , @AssetParentCode = NULL , @ServiceParentCode = 'F1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'L' , @Name = 'TANK 1' , @AssetParentCode = 'D1' , @ServiceParentCode = 'B1';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'L' , @Name = 'TANK 2' , @AssetParentCode = 'D1' , @ServiceParentCode = 'B2';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

SELECT @TypePrefix = 'L' , @Name = 'TANK 3' , @AssetParentCode = 'D1' , @ServiceParentCode = 'B3';

EXECUTE [Meta].[uspCreateEntity] @TypePrefix , @Name , @AssetParentCode , @ServiceParentCode;

INSERT into Meta.Tag (EntityId, [Key], Value) values (1,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (2,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (3,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (4,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (5,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (6,'dummy','32')
INSERT into Meta.Tag (EntityId, [Key], Value) values (7,'dummy',NULL)
INSERT into Meta.Tag (EntityId, [Key], Value) values (8,'dummy',NULL)

--SET TRANSACTION ISOLATION LEVEL READ COMMITTED
--BEGIN TRY
--    BEGIN TRANSACTION
--	DECLARE @TypeId INT = 1
--	DECLARE @Code VARCHAR(10)
--	EXECUTE Meta.uspNewEntityCode @TypeId, @Code = @Code OUTPUT;  
--	--WAITFOR DELAY '00:00:05'
--	INSERT INTO Meta.Entity(EntityTypeId,Code,Name,AssetNodeId,ServiceNodeId)
--		OUTPUT INSERTED.CODE
--		VALUES(@TypeId, @Code,'OE','/','/')
--	--WAITFOR DELAY '00:00:05'
--    COMMIT TRANSACTION -- Transaction Success!
--END TRY
--BEGIN CATCH
--    IF @@TRANCOUNT > 0
--        ROLLBACK TRANSACTION --RollBack in case of Error
--	DECLARE @ErrorMessage NVARCHAR(4000);
--	DECLARE @ErrorSeverity INT;
--	DECLARE @ErrorState INT;
--	SELECT @ErrorMessage=ERROR_MESSAGE(),
--      @ErrorSeverity=ERROR_SEVERITY(),
--      @ErrorState=ERROR_STATE();
--   RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
--END CATCH
--GO
--/****** Script for SelectTopNRows command from SSMS  ******/
--SELECT *  FROM Meta.EntityType;
--SELECT *  FROM Meta.TagType;
--SELECT *  FROM Meta.vwEntity;
--SELECT *  FROM Meta.EntityHistory;
--EXECUTE Meta.uspUpdateEntity 'g1' , @NewName = 'National Grid'
--EXECUTE Meta.uspUpdateEntity 'g1' ,@NewName = null, @NewAssetParentCode = 'c1',	@NewServiceParentCode = NULL

 