IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'PCS')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('PCS', 'Pieces', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'KG')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('KG', 'Kilogram', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'G')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('G', 'Gram', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'L')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('L', 'Liter', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'ML')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('ML', 'Milliliter', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'BOX')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('BOX', 'Box', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'PACK')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('PACK', 'Pack', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UnitsOfMeasure] WHERE [UoM] = 'DOZEN')
BEGIN
    INSERT INTO [dbo].[UnitsOfMeasure]
        ([UoM], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('DOZEN', 'Dozen', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;
