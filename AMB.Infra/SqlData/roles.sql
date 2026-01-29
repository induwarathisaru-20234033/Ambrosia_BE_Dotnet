INSERT INTO [dbo].[Roles] 
    ([RoleCode], [RoleName], [Description], [Status], [CreatedDate], [CreatedBy], [UpdatedDate], [DeletedDate], [UpdatedBy], [DeletedBy]) 
VALUES 
    ('ADMIN', 'Super Admin', 'Full system access and configuration.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('GM', 'General Manager', 'Operational oversight and financial approvals.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('HOST', 'Host / Reception', 'Reservations, guest seating, and waitlist.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('WAITER', 'Waiter', 'Order taking, serving, and billing.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('CHEF', 'Head Chef', 'Kitchen management and menu engineering.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('COOK', 'Line Cook', 'Food preparation and KDS status updates.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('BAR', 'Bartender', 'Drink preparation and Bar Display System.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('STORE', 'Storekeeper', 'Inventory receiving, counts, and wastage.', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL),
    ('PURCH', 'Purchasing Officer', 'Purchase Orders and procurement management', 1, GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', NULL, NULL);  