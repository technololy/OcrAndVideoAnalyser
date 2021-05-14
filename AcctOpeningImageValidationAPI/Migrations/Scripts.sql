IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AppruvResponses] (
    [Id] int NOT NULL IDENTITY,
    [BVN] nvarchar(11) NULL,
    [Email] nvarchar(max) NULL,
    [StatusOfRequest] nvarchar(max) NULL,
    [DateInserted] datetime NULL DEFAULT ((getdate())),
    [JsonResponse] nvarchar(max) NULL,
    CONSTRAINT [PK_AppruvResponses] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FacialValidations] (
    [Id] int NOT NULL IDENTITY,
    [BVN] nvarchar(11) NULL,
    [Email] nvarchar(max) NULL,
    [Accessories] nvarchar(max) NULL,
    [FacialHair] nvarchar(max) NULL,
    [Hair] nvarchar(max) NULL,
    [Emotion] nvarchar(max) NULL,
    [Smile] nvarchar(max) NULL,
    [Age] nvarchar(max) NULL,
    [HeadPose] nvarchar(max) NULL,
    [Gender] nvarchar(max) NULL,
    [Occlusion] nvarchar(max) NULL,
    [DateInserted] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_FacialValidations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [OCRResponses] (
    [Id] int NOT NULL IDENTITY,
    [BVN] nvarchar(11) NULL,
    [JsonResponse] nvarchar(max) NULL,
    [DateInserted] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_OCRResponses] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ScannedIDCardDetail] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [MiddleName] nvarchar(max) NULL,
    [IDType] nvarchar(max) NULL,
    [IDNumber] nvarchar(max) NULL,
    [IssueDate] nvarchar(max) NULL,
    [DateOfBirth] nvarchar(max) NULL,
    [ExpiryDate] nvarchar(max) NULL,
    [FormerIDNumber] nvarchar(max) NULL,
    [IDClass] nvarchar(max) NULL,
    [BVN] nvarchar(11) NULL,
    [Email] nvarchar(max) NULL,
    [BloodGroup] nvarchar(max) NULL,
    [Height] nvarchar(max) NULL,
    [IssuingAuthority] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [Occupation] nvarchar(max) NULL,
    [NextOfKin] nvarchar(max) NULL,
    [FirstIssueState] nvarchar(max) NULL,
    [Delim] nvarchar(max) NULL,
    [Gender] nvarchar(max) NULL,
    [DateInserted] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_ScannedIDCardDetail] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210213223829_InitialCreate', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [OCRUsages] (
    [Id] int NOT NULL IDENTITY,
    [Count] int NOT NULL,
    [EmailAddress] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [DateModified] datetime2 NOT NULL,
    [IsEnabled] bit NOT NULL,
    CONSTRAINT [PK_OCRUsages] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210228195258_OCRUsage', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [RequestLog] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [DateCreated] datetime NOT NULL DEFAULT ((getdate())),
    [DateModified] datetime2 NOT NULL,
    [IsEnabled] bit NOT NULL,
    CONSTRAINT [PK_RequestLog] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210311082816_requestLogTable', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OCRResponses]') AND [c].[name] = N'BVN');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [OCRResponses] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [OCRResponses] ALTER COLUMN [BVN] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210312115033_changeBVNCOlumnLength', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ScannedIDCardDetail] ADD [DocumentType] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210312135850_newColumnToScannedDetailedTable', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210325203311_synciingLocalDb', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ScannedIDCardDetail] ADD [OCRUsageId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [FacialValidations] ADD [FaceID] uniqueidentifier NULL;
GO

ALTER TABLE [FacialValidations] ADD [OcrUsageId] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [ImageScanneds] (
    [Id] int NOT NULL IDENTITY,
    [ImageURL] nvarchar(max) NULL,
    [OcrUsageId] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [DateModified] datetime2 NOT NULL,
    [IsEnabled] bit NOT NULL,
    CONSTRAINT [PK_ImageScanneds] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImageScanneds_OCRUsages_OcrUsageId] FOREIGN KEY ([OcrUsageId]) REFERENCES [OCRUsages] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SimilarFacesRecord] (
    [Id] int NOT NULL IDENTITY,
    [FaceId] uniqueidentifier NULL,
    [PersistedFaceId] uniqueidentifier NULL,
    [Confidence] float NOT NULL,
    [OCRUsageId] int NOT NULL,
    [DateInserted] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_SimilarFacesRecord] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SimilarFacesRecord_OCRUsages_OCRUsageId] FOREIGN KEY ([OCRUsageId]) REFERENCES [OCRUsages] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_ScannedIDCardDetail_OCRUsageId] ON [ScannedIDCardDetail] ([OCRUsageId]);
GO

CREATE UNIQUE INDEX [IX_FacialValidations_OcrUsageId] ON [FacialValidations] ([OcrUsageId]);
GO

CREATE INDEX [IX_ImageScanneds_OcrUsageId] ON [ImageScanneds] ([OcrUsageId]);
GO

CREATE UNIQUE INDEX [IX_SimilarFacesRecord_OCRUsageId] ON [SimilarFacesRecord] ([OCRUsageId]);
GO

ALTER TABLE [FacialValidations] ADD CONSTRAINT [FK_FacialValidations_OCRUsages_OcrUsageId] FOREIGN KEY ([OcrUsageId]) REFERENCES [OCRUsages] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [ScannedIDCardDetail] ADD CONSTRAINT [FK_ScannedIDCardDetail_OCRUsages_OCRUsageId] FOREIGN KEY ([OCRUsageId]) REFERENCES [OCRUsages] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210331000029_add New DBs And OCRUsageID To DBs', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [FacialValidations] DROP CONSTRAINT [FK_FacialValidations_OCRUsages_OcrUsageId];
GO

EXEC sp_rename N'[FacialValidations].[OcrUsageId]', N'OCRUsageId', N'COLUMN';
GO

EXEC sp_rename N'[FacialValidations].[IX_FacialValidations_OcrUsageId]', N'IX_FacialValidations_OCRUsageId', N'INDEX';
GO

ALTER TABLE [FacialValidations] ADD CONSTRAINT [FK_FacialValidations_OCRUsages_OCRUsageId] FOREIGN KEY ([OCRUsageId]) REFERENCES [OCRUsages] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210331113658_correct errors in ef', N'5.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210403141851_update 101', N'5.0.3');
GO

COMMIT;
GO

