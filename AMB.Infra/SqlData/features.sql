IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Employee Management', 'EMP_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'ROLE_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Role Management', 'ROLE_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'RSV_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Reservation Management', 'RSV_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'CFG_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Configurations Management', 'CFG_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'TBL_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Table Management', 'TBL_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'INV_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Inventory Management', 'INV_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT')
BEGIN
    INSERT INTO [dbo].[Features]
        ([FeatureName], [FeatureCode], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('Purchase Request Management', 'PR_MGMT', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;
