USE [Accounts]
GO
/****** Object:  StoredProcedure [dbo].[sp_Itemmaster_Delete]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Itemmaster_Delete]
(
    @Id INT
)
AS
BEGIN
    DELETE FROM Itemmaster
    WHERE Id = @Id
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Itemmaster_GetALL]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Itemmaster_GetALL]
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

GO
/****** Object:  StoredProcedure [dbo].[sp_Itemmaster_GetById]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Itemmaster_GetById]
(
    @Id INT
)
AS
BEGIN
    SELECT 
	    [Id],
		[CatCode],
		[ItemBarCode],
		[Itemcode],
		[Itemname],
		[Description],
		[Uom],
		[Rate],
		[Minimumstock],
		[Maximumstock],
		[IsActive],
		[Createdby],
		[Createddate],
		[Updatedby],
		[Updateddate]
    FROM Itemmaster
    WHERE Id = @Id
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Itemmaster_Insert]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Itemmaster_Insert]
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
    @IsActive BIT
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
		SYSTEM_USER,
		GETDATE(),
        NULL, 
		NULL
    )
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Itemmaster_Update]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 CREATE PROCEDURE [dbo].[sp_Itemmaster_Update]
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
    @IsActive BIT
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
        Updatedby = SYSTEM_USER,
        Updateddate = GETDATE()
    WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Product_Delete]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_Delete]
    @Id INT,
    @RowVersion ROWVERSION
AS
BEGIN
    DELETE FROM Products
    WHERE Id = @Id AND RowVersion = @RowVersion;

    SELECT @@ROWCOUNT AS AffectedRows;
END



GO
/****** Object:  StoredProcedure [dbo].[sp_Product_GetAll]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_GetAll]
AS
BEGIN
    SELECT Id, Name, Price, Stock, RowVersion
    FROM Products
    ORDER BY Id DESC;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Product_GetById]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_GetById]
    @Id INT
AS
BEGIN
    SELECT Id, Name, Price, Stock, RowVersion
    FROM Products
    WHERE Id = @Id;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Product_Insert]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_Insert]
    @Name NVARCHAR(150),
    @Price DECIMAL(10,2),
    @Stock INT
AS
BEGIN
    INSERT INTO Products(Name, Price, Stock)
    VALUES (@Name, @Price, @Stock);

    SELECT SCOPE_IDENTITY() AS Id;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Product_Update]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_Update]
    @Id INT,
    @Name NVARCHAR(150),
    @Price DECIMAL(10,2),
    @Stock INT,
    @RowVersion ROWVERSION
AS
BEGIN
    UPDATE Products
    SET Name=@Name,
        Price=@Price,
        Stock=@Stock
    WHERE Id=@Id AND RowVersion=@RowVersion;

    SELECT @@ROWCOUNT AS AffectedRows;
END

GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/27/2026 1:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customers]    Script Date: 3/27/2026 1:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Itemmaster]    Script Date: 3/27/2026 1:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Products]    Script Date: 3/27/2026 1:40:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251121220305_InitialCreate', N'8.0.22')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251127013430_InitialCreated', N'8.0.22')
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 

GO
INSERT [dbo].[Customers] ([Id], [Name], [Email], [Phone]) VALUES (1, N'Sample Aaaa', N'arulkg@gmail.com', N'6035608952')
GO
INSERT [dbo].[Customers] ([Id], [Name], [Email], [Phone]) VALUES (2, N'Sample B', N'arulkg1@gmail.com', N'6035608953')
GO
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
SET IDENTITY_INSERT [dbo].[Itemmaster] ON 

GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (1, N'Fruit', N'Bar001', N'Fruit00005', N'Apple', N'Red Apple', N'KGS', CAST(2500.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), 1, N'sa', CAST(0x0000B40601465188 AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B4190125A21B AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (2, N'Oil', N'Bar002', N'Oil00001', N'Idayam', N'Cooking Oil', N'LTR', CAST(250.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), 1, N'sa', CAST(0x0000B40601465188 AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41800E2BCF8 AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (3, N'Vegit', N'Bar003', N'Veg00001', N'Tomato', N'Cooking Item', N'KGS', CAST(50.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), 0, N'sa', CAST(0x0000B40601465188 AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41901379D22 AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (6, N'Snack', N'Bis0001', N'BIS0001', N'Biscuts1', N'Eating Items', N'NOS', CAST(50.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B4060169E0FE AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41800E29B63 AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (8, N'Oil', N'OilBar010', N'OilPeanut0', N'Oil Peanut', N'Oil Peanut', N'LTR', CAST(15.25 AS Decimal(18, 2)), CAST(15.00 AS Decimal(18, 2)), CAST(55.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B413013170E4 AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41800E2AFB5 AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (11, N'Test', N'TestBar', N'YestCode', N'Test Item', N'Test Item Desc', N'LTR', CAST(25.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B4130149D221 AS DateTime), N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41901205003 AS DateTime))
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (30, N'dfdf', N'dfdf', N'dfdfd', N'dfd', N'asdfadsf', N'LTR', CAST(56.00 AS Decimal(18, 2)), CAST(67.00 AS Decimal(18, 2)), CAST(89.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B4190137CF0F AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (31, N'hj', N'fghjh', N'hgj', N'ghjghj', N'fghjfghj', N'LTR', CAST(67.00 AS Decimal(18, 2)), CAST(67.00 AS Decimal(18, 2)), CAST(78.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B4190137EB86 AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (32, N'jk', N'jk', N'jk', N'jk', N'jkjkjk', N'NOS', CAST(67.00 AS Decimal(18, 2)), CAST(78.00 AS Decimal(18, 2)), CAST(89.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41901380646 AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (33, N'dfdf', N'dfdf', N'dfdfd', N'fdfd', N'dfdf', N'KGS', CAST(55.00 AS Decimal(18, 2)), CAST(56.00 AS Decimal(18, 2)), CAST(67.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B419013823A5 AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Itemmaster] ([Id], [CatCode], [ItemBarCode], [Itemcode], [Itemname], [Description], [Uom], [Rate], [Minimumstock], [Maximumstock], [IsActive], [Createdby], [Createddate], [Updatedby], [Updateddate]) VALUES (34, N'wewe', N'wewe', N'wewe', N'wewew', N'wewew', N'KGS', CAST(88.00 AS Decimal(18, 2)), CAST(99.00 AS Decimal(18, 2)), CAST(233.00 AS Decimal(18, 2)), 1, N'LAPTOP-BIG8QIRC\arulg', CAST(0x0000B41901384525 AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Itemmaster] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [Stock]) VALUES (1, N'Sample AAA', N'Sample product A', CAST(300.00 AS Decimal(18, 2)), 400)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [Stock]) VALUES (2, N'Sample BBB', N'Sample product BBB', CAST(1119.99 AS Decimal(18, 2)), 3500)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [Stock]) VALUES (6, N'Test', N'Test Mama', CAST(2000.00 AS Decimal(18, 2)), 3000)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [Stock]) VALUES (15, N'dfdfd', N'dfdfdf', CAST(8888.00 AS Decimal(18, 2)), 9999)
GO
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [Stock]) VALUES (20, N'fgfgfg', N'fgfgfg', CAST(23.00 AS Decimal(18, 2)), 45)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO


ALTER PROCEDURE [dbo].[sp_Itemmaster_GetPaged]
	@CatCode	VARCHAR(5) = NULL,
	@ItemName	VARCHAR(100) = NULL,
	@Uom		VARCHAR(3) = NULL,
    @PageNumber INT = 1,
    @PageSize   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- Default paging
    IF (@PageNumber <= 0) SET @PageNumber = 1;
    IF (@PageSize <= 0) SET @PageSize = 10;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Main data
    SELECT 
        Id,
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
    FROM dbo.Itemmaster
    WHERE 
        (@CatCode IS NULL OR CatCode LIKE '%' + @CatCode + '%')
        AND (@ItemName IS NULL OR ItemName LIKE  '%'+@ItemName+'%')
        AND (@Uom IS NULL OR Uom LIKE '%'+@Uom+'%')
    ORDER BY Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Total count (for pagination UI)
    SELECT 
        COUNT(1) AS TotalRecords
    FROM dbo.Itemmaster
    WHERE 
        (@CatCode IS NULL OR CatCode LIKE '%' + @CatCode + '%')
        AND (@ItemName IS NULL OR ItemName LIKE  '%'+@ItemName+'%')
        AND (@Uom IS NULL OR Uom LIKE '%'+@Uom+'%');
END



DECLARE	@return_value int

EXEC	@return_value = [dbo].[sp_Itemmaster_GetPaged]
		@CatCode = N'S',
		@PageNumber = 1,
		@PageSize = 10

SELECT	'Return Value' = @return_value
