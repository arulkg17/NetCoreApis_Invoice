
USE [invoice]
GO
 
/****** Object:  Table [dbo].[Itemmaster]    Script Date: 22-02-2026 09:52:10 ******/
SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
CREATE TABLE [dbo].[Itemmaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CatCode] [varchar](5) NOT NULL,
	[ItemBarCode] [varchar](25) NOT NULL,
	[Itemcode] [varchar](10) NOT NULL,
	[Itemname] [varchar](100) NOT NULL,
	[Description] [varchar](250) NULL,
	[Uom] [varchar](3) NOT NULL,
	[Rate] [decimal](18, 2) NULL,
	[Minimumstock] [decimal](18, 2) NULL,
	[Maximumstock] [decimal](18, 2) NULL,
	[IsActive] [bit] NULL,
	[Createdby] [varchar](100) NULL,
	[Createddate] [datetime] NULL,
	[Updatedby] [varchar](100) NULL,
	[Updateddate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

DROP PROCEDURE sp_Itemmaster_Insert 

CREATE PROCEDURE sp_Itemmaster_Insert
(
    @CatCode VARCHAR(5),
    @ItemBarCode VARCHAR(25),
    @Itemcode VARCHAR(10),
    @Itemname VARCHAR(100),
    @Description VARCHAR(250),
    @Uom VARCHAR(3),
    @Rate DECIMAL(18,2),
    @Minimumstock DECIMAL(18,2),
    @Maximumstock DECIMAL(18,2),
    @IsActive BIT,
    @Createdby VARCHAR(100)
)
AS
BEGIN
    INSERT INTO Itemmaster
    (
        CatCode, 
		ItemBarCode, 
		Itemcode, 
		Itemname,
        Description,
		Uom, 
		Rate,
        Minimumstock, 
		Maximumstock,
        IsActive, 
		Createdby, 
		Createddate,
        Updatedby, 
		Updateddate
    )
	VALUES
    (
        @CatCode, 
		@ItemBarCode,
		@Itemcode, 
		@Itemname,
        @Description, 
		@Uom, 
		@Rate,
        @Minimumstock, 
		@Maximumstock,
        @IsActive, 
		@Createdby, 
		GETDATE(),
        NULL, 
		NULL
    )
END

 
Select * from Itemmaster;
 
EXEC sp_Itemmaster_Insert
	'Fruit',
	'Bar005',
	'Fruit00005',
	'Apple',
	'Red Apple',
	'Kg',
	150,20,100,1,
	'Valavan'

 select * from Itemmaster;
 
 CREATE PROCEDURE sp_Itemmaster_Update
(
    @Id INT,
    @CatCode VARCHAR(5),
    @ItemBarCode VARCHAR(25),
    @Itemcode VARCHAR(10),
    @Itemname VARCHAR(100),
    @Description VARCHAR(250),
    @Uom VARCHAR(3),
    @Rate DECIMAL(18,2),
    @Minimumstock DECIMAL(18,2),
    @Maximumstock DECIMAL(18,2),
    @IsActive BIT,
    @Updatedby VARCHAR(100)
)
AS
BEGIN
    UPDATE Itemmaster
    SET
        CatCode = @CatCode,
        ItemBarCode = @ItemBarCode,
        Itemcode = @Itemcode,
        Itemname = @Itemname,
        Description = @Description,
        Uom = @Uom,
        Rate = @Rate,
        Minimumstock = @Minimumstock,
        Maximumstock = @Maximumstock,
        IsActive = @IsActive,
        Updatedby = @Updatedby,
        Updateddate = GETDATE()
    WHERE Id = @Id
END
 
Select * From Itemmaster;
 
EXEC sp_Itemmaster_Update
	1,
	'Soap',
	'Bar001',
	'Soap000001',
	'Lux Updated',
	'Bath Soap',
	'No',
	99.99,
	30,
	120,
	1,
	'Valavan'
 
Select * From Itemmaster;

CREATE PROCEDURE sp_Itemmaster_Delete
(
    @Id INT
)
AS
BEGIN
    DELETE FROM Itemmaster
    WHERE Id = @Id
END
 
Select * From Itemmaster;
 
EXEC sp_Itemmaster_Delete 1
 
Select * From Itemmaster;


CREATE PROCEDURE sp_Itemmaster_GetById
(
    @Id INT
)
AS
BEGIN
    SELECT [Id]
      ,[CatCode]
      ,[ItemBarCode]
      ,[Itemcode]
      ,[Itemname]
      ,[Description]
      ,[Uom]
      ,[Rate]
      ,[Minimumstock]
      ,[Maximumstock]
      ,[IsActive]
      ,[Createdby]
      ,[Createddate]
      ,[Updatedby]
      ,[Updateddate]
  FROM [dbo].[Itemmaster] 
    WHERE Id = @Id
END
 
Select * From Itemmaster;
 
EXEC sp_Itemmaster_GetById 2
 
Select * From Itemmaster;

CREATE PROCEDURE sp_Itemmaster_GetALL
AS
BEGIN
    SELECT [Id]
      ,[CatCode]
      ,[ItemBarCode]
      ,[Itemcode]
      ,[Itemname]
      ,[Description]
      ,[Uom]
      ,[Rate]
      ,[Minimumstock]
      ,[Maximumstock]
      ,[IsActive]
      ,[Createdby]
      ,[Createddate]
      ,[Updatedby]
      ,[Updateddate]
  FROM [dbo].[Itemmaster] 
  ORDER BY ID ASC 
END

EXEC sp_Itemmaster_GetAll

 