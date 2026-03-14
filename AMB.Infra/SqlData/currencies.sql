IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'USD')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('USD', 'US Dollar', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'EUR')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('EUR', 'Euro', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'GBP')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('GBP', 'British Pound', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'LKR')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('LKR', 'Sri Lankan Rupee', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'INR')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('INR', 'Indian Rupee', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'AUD')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('AUD', 'Australian Dollar', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'CAD')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('CAD', 'Canadian Dollar', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Currencies] WHERE [CurrencyCode] = 'JPY')
BEGIN
    INSERT INTO [dbo].[Currencies]
        ([CurrencyCode], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy])
    VALUES
        ('JPY', 'Japanese Yen', 1, SYSDATETIMEOFFSET(), 'SYSTEM', SYSDATETIMEOFFSET(), NULL, NULL, NULL);
END;
