IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'EMP_READ')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT'), 'EMP_READ', 'Employee.Read', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'EMP_CRT')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT'), 'EMP_CRT', 'Employee.Create', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'EMP_UPD')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT'), 'EMP_UPD', 'Employee.Update', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'EMP_ASGNR')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT'), 'EMP_ASGNR', 'Employee.Assign.Role', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'EMP_UNASG')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'EMP_MGMT'), 'EMP_UNASG', 'Employee.Unassign.Role', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'ROLE_READ')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'ROLE_MGMT'), 'ROLE_READ', 'Role.Read', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'ROLE_UPD')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'ROLE_MGMT'), 'ROLE_UPD', 'Role.Update', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'ROLE_CRT')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'ROLE_MGMT'), 'ROLE_CRT', 'Role.Create', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'INV_READ')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'INV_MGMT'), 'INV_READ', 'Inventory.Read', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'INV_CRT')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'INV_MGMT'), 'INV_CRT', 'Inventory.Create', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'INV_UPD')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'INV_MGMT'), 'INV_UPD', 'Inventory.Update', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'PR_READ')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT'), 'PR_READ', 'PR.Read', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'PR_UPD')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT'), 'PR_UPD', 'PR.Update', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'PR_CRT')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT'), 'PR_CRT', 'PR.Create', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'PR_REJ')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT'), 'PR_REJ', 'PR.Reject', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [PermissionCode] = 'PR_ACC')
BEGIN
    INSERT INTO [dbo].[Permissions]
        ([FeatureId], [PermissionCode], [PermissionName], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ((SELECT TOP 1 [Id] FROM [dbo].[Features] WHERE [FeatureCode] = 'PR_MGMT'), 'PR_ACC', 'PR.Accept', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;
