using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AMB.Application.Services
{
    public class WastageRecordService : IWastageRecordService
    {
        private readonly IWastageRecordRepository _wastageRecordRepository;

        public WastageRecordService(IWastageRecordRepository wastageRecordRepository)
        {
            _wastageRecordRepository = wastageRecordRepository;
        }

        public async Task<WastageRecordDto> CreateWastageRecordAsync(CreateWastageRecordDto request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("At least one wastage entry item is required.");

            var wastageRecord = new WastageRecord
            {
                WastageEntryNumber = await GenerateUniqueWastageEntryNumberAsync(),
                EntryDate = request.EntryDate,
                RecordedBy = request.RecordedBy,
                GeneralNotes = request.GeneralNotes,
                WastageEntryItems = request.Items.Select(item => new WastageEntryItem
                {
                    ItemNo = item.ItemNo,
                    WastageType = item.WastageType,
                    Quantity = item.Quantity,
                    Reason = item.Reason,
                    InventoryItemId = item.InventoryItemId
                }).ToList()
            };

            var result = await _wastageRecordRepository.AddAsync(wastageRecord);

            return new WastageRecordDto
            {
                Id = result.Id,
                WastageEntryNumber = result.WastageEntryNumber,
                EntryDate = result.EntryDate,
                RecordedBy = result.RecordedBy,
                GeneralNotes = result.GeneralNotes,
                Items = result.WastageEntryItems.Select(e => new WastageEntryItemDto
                {
                    Id = e.Id,
                    ItemNo = e.ItemNo,
                    WastageType = e.WastageType,
                    Quantity = e.Quantity,
                    Reason = e.Reason,
                    InventoryItemId = e.InventoryItemId,
                    InventoryItemName = e.InventoryItem?.ItemName ?? string.Empty
                }).ToList()
            };
        }

        private async Task<string> GenerateUniqueWastageEntryNumberAsync()
        {
            // TODO: Implement unique number generation logic (e.g., based on date/time or sequence)
            return $"WSTG-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
